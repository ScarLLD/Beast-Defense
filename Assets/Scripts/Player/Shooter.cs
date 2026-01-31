using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Shooter : MonoBehaviour
{
    [SerializeField] private float _timeBetweenShoot;

    private BulletSpawner _bulletSpawner;
    private Coroutine _shootCoroutine;
    private Animator _animator;
    private Queue<SnakeSegment> _targets;
    private WaitForSeconds _sleepTime;

    private int _initialBulletCount;
    private int _bulletCount;
    private Quaternion _initialRotation;

    public int BulletCount => _bulletCount;

    public event Action BulletsCountChanged;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _targets = new Queue<SnakeSegment>();
        _sleepTime = new WaitForSeconds(_timeBetweenShoot);
    }

    public void Init(BulletSpawner bulletSpawner, int bulletCount)
    {
        _bulletSpawner = bulletSpawner;
        _initialBulletCount = bulletCount;
        _bulletCount = _initialBulletCount;
        _initialRotation = transform.rotation;
    }

    public void AddTarget(SnakeSegment snakeSegment)
    {
        if (_targets.Contains(snakeSegment) == false)
            _targets.Enqueue(snakeSegment);

        _shootCoroutine ??= StartCoroutine(Shoot());
    }

    public void SetInitialRotation()
    {
        transform.rotation = _initialRotation;
    }

    public void SetDafaultSettings()
    {
        if (_shootCoroutine != null)
        {
            StopCoroutine(_shootCoroutine);
            _shootCoroutine = null;
        }

        _targets.Clear();
        _bulletCount = _initialBulletCount;
        BulletsCountChanged?.Invoke();
        SetInitialRotation();
    }

    private IEnumerator Shoot()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_targets.Count > 0)
            {
                SnakeSegment segment = _targets.Dequeue();
                int spawnedBullet = 0;

                while (segment.TryGetCube(out Cube cube) && spawnedBullet < 4)
                {
                    spawnedBullet++;
                    transform.LookAt(segment.transform.position);
                    _bulletSpawner.SpawnBullet(transform.position, cube);
                    _bulletCount--;

                    BulletsCountChanged?.Invoke();

                    _animator.ResetTrigger("Shoot");
                    _animator.SetTrigger("Shoot");
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