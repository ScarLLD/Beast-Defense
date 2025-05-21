using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private TargetCollector _targetCollector;
    [SerializeField] private float _timeBetweenShoot;

    private IEnumerator coroutine;
    private WaitForSeconds _wait;

    private void Awake()
    {
        coroutine = Shoot();
    }

    private void Start()
    {
        new WaitForSeconds(_timeBetweenShoot);
        StartCoroutine(coroutine);
    }

    private IEnumerator Shoot()
    {

        if (_targetCollector.TryGetTarget(out Transform targetTransform))
        {
            Debug.Log("shoot");

            if (targetTransform.TryGetComponent<ITarget>(out ITarget target))
            {
                target.ChangeCapturedStatus();
                SpawnBullet(targetTransform);
            }
        }

        yield return _wait;
    }

    public void SpawnBullet(Transform targetTransform)
    {
        Bullet bullet = Instantiate(_bulletPrefab, transform);
        bullet.Init(targetTransform);
    }
}
