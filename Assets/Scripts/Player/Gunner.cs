using System.Collections;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    [SerializeField] private BulletSpawner _spawner;
    [SerializeField] private float _timeBetweenShoot;

    public void Shoot(Transform targetTransform)
    {
        if (targetTransform.TryGetComponent(out ITarget target))
        {
            target.ChangeCapturedStatus();
            _spawner.SpawnBullet(transform.position, targetTransform);
        }
    }
}