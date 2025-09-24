using System.Collections.Generic;
using UnityEngine;

public class CubeCreator : MonoBehaviour
{
    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private PlayerCube _cubePrefab;
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private TargetStorage _targetStorage;

    [SerializeField]
    private List<Material> _ñolors = new();

    [SerializeField]
    private List<int> _counts = new();

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public bool TryMoveToCenterScreenBottom()
    {
        if (_boundaryMaker.TryGetScreenBottomCenter(out Vector3 bottomScreenCenter))
        {
            _transform.position = new Vector3(bottomScreenCenter.x, bottomScreenCenter.y + _cubePrefab.transform.localScale.y / 2, bottomScreenCenter.z);
            return true;
        }

        return false;
    }

    public bool TryCreate()
    {
        if (_gridStorage.GridCount > 0 && TryMoveToCenterScreenBottom())
        {
            int gridCount = _gridStorage.GridCount;

            if (gridCount == 0)
                return false;

            _cubeStorage.Clear();

            for (int i = 0; i < gridCount; i++)
            {
                int count = _counts[Random.Range(0, _counts.Count)];
                Material material = _ñolors[Random.Range(0, _ñolors.Count)];

                if (_gridStorage.TryGet(i, out GridCell gridCell) && gridCell.IsOccupied == false)
                {
                    Vector3 spawnPoint = new(gridCell.transform.position.x, gridCell.transform.position.y + _cubePrefab.transform.localScale.y / 2, gridCell.transform.position.z);

                    PlayerCube playerCube = Instantiate(_cubePrefab, spawnPoint, Quaternion.identity, _transform);
                    playerCube.Init(gridCell, material, count, _bulletSpawner, _targetStorage);
                    gridCell.InitCube(playerCube);
                    _cubeStorage.Add(playerCube);
                }
            }

            return true;
        }

        Debug.Log("Íå óäàëîñü ñãåíåðèðîâàòü êóáû.");
        return false;
    }
}
