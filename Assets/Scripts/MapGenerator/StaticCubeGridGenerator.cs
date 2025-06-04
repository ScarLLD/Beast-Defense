using UnityEngine;

[RequireComponent(typeof(StaticCubesHolder))]
public class StaticCubeGridGenerator : MonoBehaviour
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

    private void Awake()
    {
        _holder = GetComponent<StaticCubesHolder>();

        objectWidth = _cubePrefab.transform.localScale.x;
        objectDepth = _cubePrefab.transform.localScale.z;
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
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

                PlayerCube cube = Instantiate(_cubePrefab, transform);

                cube.Init(_bulletSpawner);
                cube.transform.localPosition = spawnPosition;

                _holder.PutCube(cube);
            }
        }
    }
}
