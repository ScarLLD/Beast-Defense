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

        if (_coroutine == null)
            _coroutine = StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (_targets.Count > 0)
        {
            var target = _targets.Dequeue();

            while (target.TryGetCube(out Cube cube))
            {
                _bulletSpawner.SpawnBullet(transform.position, cube);
                _bulletCount--;

                yield return new WaitForSeconds(0.2f);
            }
        }

        _coroutine = null;
    }
}