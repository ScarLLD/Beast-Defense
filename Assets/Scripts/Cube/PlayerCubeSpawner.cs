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
        else
            Debug.LogWarning("Не удалось найти центр нижней половины экрана.");
    }

    public void Spawn(Material material, int count, GridCell cell)
    {
        Vector3 spawnPoint = new(cell.transform.position.x, cell.transform.position.y + _cubePrefab.transform.localScale.y / 2, cell.transform.position.z);

        PlayerCube playerCube = Instantiate(_cubePrefab, spawnPoint, Quaternion.identity, transform);
        playerCube.Init(cell, material, count, _bulletSpawner, _targetStorage);
        cell.InitCube(playerCube);
        _cubeStorage.Add(playerCube);
    }
}
