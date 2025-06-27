using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
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

    private List<Vector3> _grid = new List<Vector3>();

    public int GridCount => _grid.Count;

    private void Awake()
    {


        objectWidth = _cubePrefab.transform.localScale.x;
        objectDepth = _cubePrefab.transform.localScale.z;
    }

    //public void SpawnCubes(List<CubeStack> cubes)
    //{
    //    Queue<CubeStack> queue = new Queue<CubeStack>();

    //    foreach (var cube in cubes)
    //    {
    //        queue.Enqueue(cube);
    //    }

    //    foreach (var position in _grid)
    //    {
    //        CubeStack stack = queue.Dequeue();
    //        CubeStack cube = Instantiate(_cubePrefab, transform);
    //        cube.transform.localPosition = position;
    //        cube.Init(_bulletSpawner, stack.Count);
    //        cube.GetComponent<MeshRenderer>().material = stack.Material;

    //        _storage.Add(cube);
    //    }
    //}

    public void Create()
    {
        float availableSpaceX = maxX - minX - (columns * objectWidth);
        float availableSpaceZ = maxZ - minZ - (rows * objectDepth);

        if (availableSpaceX < 0 || availableSpaceZ < 0)
        {
            Debug.LogError("Недостаточно места.");
            return;
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

                _grid.Add(spawnPosition);
            }
        }
    }
}