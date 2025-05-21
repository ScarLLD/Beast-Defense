using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Transform _targetTransform;
    private ObjectPool<Bullet> _pool;

    public void Init(ObjectPool<Bullet> pool, Transform target)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool), $"pool не может быть null.");

        if (target == null)
            throw new ArgumentNullException(nameof(target), $"_enemyPrefab не может быть null.");

        _pool = pool;
        _targetTransform = target;
    }

    private void Update()
    {
        if (isActiveAndEnabled == true && _targetTransform != null)
        {
            Vector3 direction = _targetTransform.position - transform.position;
            transform.Translate(_speed * Time.deltaTime * direction.normalized);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out ITarget target))
        {
            _pool.ReturnObject(this);
            Debug.Log("Returned");
        }
    }
}
