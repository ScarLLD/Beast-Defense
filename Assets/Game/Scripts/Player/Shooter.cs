using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.BulletCore;
using Game.Scripts.SnakeCore;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    public class Shooter : MonoBehaviour
    {
        [SerializeField] private float _timeBetweenShoot;
        private Animator _animator;
        private int _bulletPerTarget;

        private BulletSpawner _bulletSpawner;

        private int _initialBulletCount;
        private Quaternion _initialRotation;
        private Coroutine _shootCoroutine;
        private WaitForSeconds _sleepTime;
        private Queue<SnakeSegment> _targets;

        public int BulletCount { get; private set; }

        public event Action BulletsCountChanged;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _targets = new Queue<SnakeSegment>();
            _sleepTime = new WaitForSeconds(_timeBetweenShoot);
        }

        public void Init(BulletSpawner bulletSpawner, int bulletCount, int bulletPerTarget)
        {
            _bulletSpawner = bulletSpawner;
            _initialBulletCount = bulletCount;
            _bulletPerTarget = bulletPerTarget;
            _initialRotation = transform.rotation;
            BulletCount = _initialBulletCount;
        }

        public void AddTarget(SnakeSegment snakeSegment)
        {
            if (!_targets.Contains(snakeSegment))
                _targets.Enqueue(snakeSegment);

            _shootCoroutine ??= StartCoroutine(Shoot());
        }

        public void SetInitialRotation()
        {
            transform.rotation = _initialRotation;
        }

        public void ResetSettings()
        {
            if (_shootCoroutine != null)
            {
                StopCoroutine(_shootCoroutine);
                _shootCoroutine = null;
            }

            _targets.Clear();
            BulletCount = _initialBulletCount;
            BulletsCountChanged?.Invoke();
            SetInitialRotation();
        }

        private IEnumerator Shoot()
        {
            var isWork = true;

            while (isWork)
            {
                if (_targets.Count > 0)
                {
                    var segment = _targets.Dequeue();
                    var spawnedBullet = 0;

                    while (segment.TryGetCube(out var cube) && spawnedBullet < _bulletPerTarget)
                    {
                        spawnedBullet++;
                        transform.LookAt(segment.transform.position);
                        _bulletSpawner.SpawnBullet(transform.position, cube);
                        BulletCount--;

                        BulletsCountChanged?.Invoke();

                        _animator.ResetTrigger(nameof(Shoot));
                        _animator.SetTrigger(nameof(Shoot));
                        yield return _sleepTime;
                    }

                    if (BulletCount == 0)
                        isWork = false;
                    else if (_targets.Count == 0)
                        SetInitialRotation();
                }

                yield return null;
            }
        }
    }
}