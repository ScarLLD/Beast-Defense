using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float _timeBetweenShoot;

    private BulletSpawner _bulletSpawner;
    private Coroutine _coroutine;
    private Queue<SnakeSegment> _targets;
    private WaitForSeconds _sleepTime;

    private int _bulletCount;
    private Quaternion _initialRotation;

    public int BulletCount => _bulletCount;

    public event Action BulletsDecreased;

    private void Awake()
    {
        _targets = new Queue<SnakeSegment>();
        _sleepTime = new WaitForSeconds(_timeBetweenShoot);
    }

    public void Init(BulletSpawner bulletSpawner, int bulletCount)
    {
        _bulletSpawner = bulletSpawner;
        _bulletCount = bulletCount;
        _initialRotation = transform.rotation;
    }

    public void AddTarget(SnakeSegment snakeSegment)
    {
        if (_targets.Contains(snakeSegment) == false)
            _targets.Enqueue(snakeSegment);

        _coroutine ??= StartCoroutine(Shoot());
    }

    public void SetInitialRotation()
    {
        transform.rotation = _initialRotation;
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

                    BulletsDecreased?.Invoke();

                    yield return _sleepTime;
                }

                if (BulletCount == 0)
                    isWork = false;
            }

            SetInitialRotation();

            yield return null;
        }
    }
}