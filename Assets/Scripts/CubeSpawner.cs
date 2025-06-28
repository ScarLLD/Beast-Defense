using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private CubeStack _cubeStackPrefab;
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private BoundaryMaker _boundaryMaker;

    public CubeStack CubePrefab => _cubeStackPrefab;

    private void Start()
    {
        if (_boundaryMaker.TryGetScreenBottomCenter(out Vector3 bottomScreenCenter))
        {
            transform.position = bottomScreenCenter;
            Debug.Log("CubeSpawner change POS");
        }
    }

    public void Spawn(Material material, int count, Vector3 spawnPoint)
    {
        CubeStack cubeStack = Instantiate(_cubeStackPrefab, transform);
        cubeStack.transform.localPosition = spawnPoint;
        cubeStack.Init(material, count);

        _cubeStorage.Add(cubeStack);
    }
}
