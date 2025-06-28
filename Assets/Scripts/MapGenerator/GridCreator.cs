using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;

    private float _objectWidth;
    private float _objectDepth;

    [Header("Границы спавна")]
    [SerializeField] private float _minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;

    public event Action Created;

    private void Start()
    {
        _objectWidth = _cubePrefab.transform.localScale.x;
        _objectDepth = _cubePrefab.transform.localScale.z;

        Create();
    }

    public void Create()
    {
        float availableSpaceX = maxX - _minX - (_columns * _objectWidth);
        float availableSpaceZ = maxZ - minZ - (_rows * _objectDepth);

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
                float localZ = minZ + (_objectDepth / 2) + row * (_objectDepth + spacingZ);

                Vector3 spawnPosition = new(localX, 0f, localZ);

                _gridStorage.Add(spawnPosition);
            }
        }

        Created?.Invoke();
    }
}