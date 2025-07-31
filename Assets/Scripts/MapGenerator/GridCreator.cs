using System;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private GridCell _cellPrefab;
    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;

    private float _objectWidth;
    private float _objectDepth;

    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxZ;

    public event Action Created;

    public int Rows => _rows;
    public int Columns => _columns;

    private void Start()
    {
        if (_boundaryMaker.TryGetScreenBottomCenter(out Vector3 bottomScreenCenter))
            transform.position = new Vector3(bottomScreenCenter.x, bottomScreenCenter.y + _cellPrefab.transform.localScale.y / 2, bottomScreenCenter.z);

        _objectWidth = _cubePrefab.transform.localScale.x;
        _objectDepth = _cubePrefab.transform.localScale.z;

        Create();
    }

    public void Create()
    {
        float availableSpaceX = _maxX - _minX - (_columns * _objectWidth);
        float availableSpaceZ = _maxZ - _minZ - (_rows * _objectDepth);

        if (availableSpaceX < 0 || availableSpaceZ < 0)
        {
            Debug.LogError("Недостаточно места.");
            return;
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

        Created?.Invoke();
    }
}