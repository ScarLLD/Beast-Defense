using UnityEngine;

public class PlayerCubeSpawner : MonoBehaviour
{
    [SerializeField] private PlayerCube _cubePrefab;
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private TargetStorage _targetStorage;

    public PlayerCube CubePrefab => _cubePrefab;

    private void Start()
    {
        if (_boundaryMaker.TryGetScreenBottomCenter(out Vector3 bottomScreenCenter))
            transform.position = bottomScreenCenter;
    }

    public void Spawn(Material material, int count, Vector3 spawnPoint)
    {
        PlayerCube cubeStack = Instantiate(_cubePrefab, transform);
        cubeStack.transform.localPosition = spawnPoint;
        cubeStack.Init(material, count, _bulletSpawner, _targetStorage);

        _cubeStorage.Add(cubeStack);
    }
}
