using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private ParticleCreator _particleCreator;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _container;

    private ObjectPool<Bullet> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Bullet>(_bulletPrefab, _container);
    }

    public void SpawnBullet(Vector3 spawnPosition, Cube cube)
    {
        Bullet bullet = _pool.GetObject();
        bullet.transform.position = spawnPosition;
        bullet.Init(_particleCreator);
        bullet.InitTarget(cube);
    }
}
