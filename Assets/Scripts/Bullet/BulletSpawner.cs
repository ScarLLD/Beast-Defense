using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private ParticleCreator _particleCreator;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _container;

    private List<Bullet> _bullets;

    private ObjectPool<Bullet> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Bullet>(_bulletPrefab, _container);
        _bullets = new List<Bullet>();
    }

    public void SpawnBullet(Vector3 spawnPosition, Cube cube)
    {
        Bullet bullet = _pool.GetObject();

        if (_bullets.Contains(bullet) == false)
            _bullets.Add(bullet);

        bullet.transform.position = spawnPosition;
        bullet.Init(_particleCreator);
        bullet.InitTarget(cube);
    }

    public void Cleanup()
    {
        foreach (Bullet bullet in _bullets)
        {
            if (bullet.gameObject.activeInHierarchy == true)
                bullet.StopMove();
        }
    }
}
