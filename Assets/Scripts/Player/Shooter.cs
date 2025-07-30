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

    private int _bulletCount;

    public int BulletCount => _bulletCount;

    public event Action BulletsDecreased;

    private void Awake()
    {
        _targets = new Queue<SnakeSegment>();
    }

    public void Init(BulletSpawner bulletSpawner, int bulletCount)
    {
        _bulletSpawner = bulletSpawner;
        _bulletCount = bulletCount;
    }

    public void AddTarget(SnakeSegment snakeSegment)
    {
        _targets.Enqueue(snakeSegment);

        _coroutine ??= StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        bool isWork = true;

        Quaternion initialRotation = transform.rotation;

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

                    yield return new WaitForSeconds(0.2f);
                }

                if (BulletCount == 0)
                    isWork = false;
            }
            else
            {
                transform.rotation = initialRotation;
            }

            transform.rotation = initialRotation;

            yield return null;
        }
    }
}