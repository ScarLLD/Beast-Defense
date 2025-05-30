using System;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float _timeBetweenShoot;

    private BulletSpawner _bulletSpawner;

    public void Init(BulletSpawner bulletSpawner)
    {
        _bulletSpawner = bulletSpawner;
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