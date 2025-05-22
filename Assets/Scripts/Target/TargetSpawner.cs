using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform _container;

    private ObjectPool<Enemy> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Enemy>(_enemyPrefab, _container);
    }

    public void SpawnEnemy(Vector3 spawnPosition, Transform targetTransform)
    {
        Enemy enemy = _pool.GetObject();
        enemy.transform.position = spawnPosition;
        enemy.Init(targetTransform);
    }
}