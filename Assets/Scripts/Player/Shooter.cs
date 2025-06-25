using System;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float _timeBetweenShoot;

    private BulletSpawner _bulletSpawner;

    private int _bulletCount;

    public void Init(BulletSpawner bulletSpawner, int bulletCount)
    {
        _bulletSpawner = bulletSpawner;
        _bulletCount = bulletCount;
    }

    public void Shoot(Transform targetTransform)
    {
        if (targetTransform.TryGetComponent(out ITarget target) && target.IsCaptured == false)
        {
            target.ChangeCapturedStatus();
            _bulletSpawner.SpawnBullet(transform.position, targetTransform);
        }
    }
}