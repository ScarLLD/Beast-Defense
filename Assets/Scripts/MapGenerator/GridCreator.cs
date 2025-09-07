using System;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private PlayerCube _cubePrefab;
    [SerializeField] private GridCell _cellPrefab;
    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private BoundaryMaker _boundaryMaker;

    [SerializeField] private int _rows;
    [SerializeField] private int _columns;

    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxZ;

    private float _scaleMultiplier = 0.9f;
    private float _objectWidth;
    private float _objectDepth;

    public event Action Created;

    private void Start()
    {
        if (_boundaryMaker.TryGetScreenBottomCenter(out Vector3 bottomScreenCenter))
            transform.position = new Vector3(bottomScreenCenter.x, bottomScreenCenter.y + _cellPrefab.transform.localScale.y / 2, bottomScreenCenter.z);

        _objectWidth = _cubePrefab.transform.localScale.x;
        _objectDepth = _cubePrefab.transform.localScale.z;

        _cellPrefab.transform.localScale = new(_cubePrefab.transform.localScale.x * _scaleMultiplier, 0.01f, _cubePrefab.transform.localScale.z * _scaleMultiplier);
    }

    public bool TryCreate(out Vector3 cubeScale)
    {
        cubeScale = _cubePrefab.transform.localScale;

        float availableSpaceX = _maxX - _minX - (_columns * _objectWidth);
        float availableSpaceZ = _maxZ - _minZ - (_rows * _objectDepth);

        if (availableSpaceX < 0 || availableSpaceZ < 0)
        {
            Debug.LogError("Недостаточно места для сетки.");
            return false;
        }

        float spacingX = availableSpaceX / (_columns - 1);
        float spacingZ = availableSpaceZ / (_rows - 1);

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                float localX = _minX + (_objectWidth / 2) + col * (_objectWidth + spacingX);
                float localZ = _minZ + (_objectDepth / 2) + row * (_objectDepth + spacingZ);

                Vector3 spawnPosition = new(localX, 0f, localZ);
                GridCell gridCell = Instantiate(_cellPrefab, transform);
                gridCell.transform.localPosition = spawnPosition;

                _gridStorage.Add(gridCell);
            }
        }

        if (_gridStorage.GridCount == 0)
        {
            Debug.Log("Не удалось сгенерировать сетку.");
            return false;
        }

        _gridStorage.CreateCells(_rows, _columns);

        Created?.Invoke();
        return true;
    }
}