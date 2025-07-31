using UnityEngine;

public class PlayerCubeSpawner : MonoBehaviour
{
    [SerializeField] private PlayerCube _cubePrefab;
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private TargetStorage _targetStorage;

    private void Start()
    {
        if (_boundaryMaker.TryGetScreenBottomCenter(out Vector3 bottomScreenCenter))
            transform.position = new Vector3(bottomScreenCenter.x, bottomScreenCenter.y + _cubePrefab.transform.localScale.y / 2, bottomScreenCenter.z);
    }

    public void Spawn(Material material, int count, Vector3 spawnPoint)
    {
        PlayerCube playerCube = Instantiate(_cubePrefab, transform);
        playerCube.transform.localPosition = spawnPoint;
        playerCube.Init(material, count, _bulletSpawner, _targetStorage);
        _cubeStorage.Add(playerCube);
    }
}
