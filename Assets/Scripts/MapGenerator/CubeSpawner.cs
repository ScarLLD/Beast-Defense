using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private Transform _container;

    private ObjectPool<Cube> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Cube>(_cubePrefab, _container);
    }

    public void SpawnCube(Vector3 spawnPosition, Transform targetTransform)
    {
        //Cube cube = _pool.GetObject();
        //cube.transform.position = spawnPosition;
        //cube.Init(_pool, targetTransform);
    }
}
