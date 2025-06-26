using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StaticCubesHolder))]
public class GridAndCubesGenerator : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private PlayerCube _cubePrefab;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private int rows;
    [SerializeField] private int columns;

    private float objectWidth;
    private float objectDepth;

    [Header("Границы спавна")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;

    private StaticCubesHolder _holder;
    private List<Vector3> _positions;

    private void Awake()
    {
        _positions = new List<Vector3>();

        _holder = GetComponent<StaticCubesHolder>();

        objectWidth = _cubePrefab.transform.localScale.x;
        objectDepth = _cubePrefab.transform.localScale.z;
    }

    public void SpawnCubes(List<CustomCube> cubes)
    {
        Queue<CustomCube> queue = new Queue<CustomCube>();

        foreach (var cube in cubes)
        {
            queue.Enqueue(cube);
        }

        foreach (var position in _positions)
        {
            CustomCube customCube = queue.Dequeue();
            PlayerCube cube = Instantiate(_cubePrefab, transform);
            cube.transform.localPosition = position;
            cube.Init(_bulletSpawner, customCube.Count);
            cube.GetComponent<MeshRenderer>().material = customCube.Material;

            _holder.PutCube(cube);
        }
    }

    public bool TryGetGridPositions(out List<Vector3> positions)
    {
        positions = new List<Vector3>();

        float availableSpaceX = maxX - minX - (columns * objectWidth);
        float availableSpaceZ = maxZ - minZ - (rows * objectDepth);

        if (availableSpaceX < 0 || availableSpaceZ < 0)
        {
            Debug.LogError("Недостаточно места.");
            return false;
        }

        float spacingX = availableSpaceX / (columns - 1);
        float spacingZ = availableSpaceZ / (rows - 1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float localX = minX + (objectWidth / 2) + col * (objectWidth + spacingX);
                float localZ = minZ + (objectDepth / 2) + row * (objectDepth + spacingZ);

                Vector3 spawnPosition = new(localX, 0f, localZ);

                positions.Add(spawnPosition);
            }
        }

        _positions = positions;

        return positions.Count > 0;
    }
}