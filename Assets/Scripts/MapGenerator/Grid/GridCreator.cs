using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridCreator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private PlayerCube _cubePrefab;
    [SerializeField] private GridCell _cellPrefab;
    [SerializeField] private Obstacle _obstaclePrefab;
    [SerializeField] private Obstacle _stretchedObstaclePrefab;

    [Header("Grid settings")]
    [SerializeField] private Vector3 _centerPosition;
    [SerializeField] private int _rows = 7;
    [SerializeField] private int _columns = 8;
    [SerializeField] private float _minZ = -12;
    [SerializeField] private float _maxZ = 10;
    [SerializeField] private float _minX = -10f;
    [SerializeField] private float _maxX = 10f;

    [Header("Obstacles settings")]
    [SerializeField] private bool _isCreateObstacles;
    [SerializeField] private int _maxObstacles = 6;
    [SerializeField] private int _maxObstacleLength = 3;
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _walls;

    [Header("Other settings")]
    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private BoundaryMaker _boundaryMaker;

    private List<Obstacle> _obstacles;
    private GridCell[,] _cellGrid;
    private bool[,] _obstacleMap;

    private float _objectWidth;
    private float _objectDepth;
    private float _spacingX;
    private float _spacingZ;
    private Vector3 _gridCenterOffset;
    private Vector3 _leftCornerPosition;
    private Vector3 _rightCornerPosition;

    public event Action Created;

    private void Awake()
    {
        _obstacles = new List<Obstacle>();
    }

    private void Start()
    {
        _objectWidth = _cubePrefab.transform.localScale.x;
        _objectDepth = _cubePrefab.transform.localScale.z;
    }

    public bool TryCreate(out Vector3 cubeScale)
    {
        cubeScale = _cubePrefab.transform.localScale;

        float localMinX = _minX + _gridCenterOffset.x;
        float localMaxX = _maxX + _gridCenterOffset.x;
        float localMinZ = _minZ + _gridCenterOffset.z;
        float localMaxZ = _maxZ + _gridCenterOffset.z;

        float availableSpaceX = localMaxX - localMinX - (_columns * _objectWidth);
        float availableSpaceZ = localMaxZ - localMinZ - (_rows * _objectDepth);

        if (availableSpaceX < 0 || availableSpaceZ < 0)
        {
            Debug.LogError("Недостаточно места для сетки.");
            return false;
        }

        _spacingX = availableSpaceX / (_columns - 1);
        _spacingZ = availableSpaceZ / (_rows - 1);

        _obstacleMap = GenerateComplexObstacleMap();
        _cellGrid = new GridCell[_rows, _columns];

        CreateGridCells(localMinX, localMinZ);

        bool _isCreateObstaclesSucces = false;

        if (_isCreateObstacles)
            _isCreateObstaclesSucces = UserUtils.GetIntRandomNumber(0, 2) == 1;

        if (_isCreateObstaclesSucces)
        {
            CreateAllObstacles();
            CreateStretchedObstaclesBetweenNeighbors();
            _walls.gameObject.SetActive(true);

        }

        if (_gridStorage.GridCount == 0)
        {
            Debug.Log("Не удалось сгенерировать сетку.");
            return false;
        }

        _gridStorage.CreateCells(_rows, _columns);
        transform.position = _centerPosition;

        Created?.Invoke();
        return true;
    }

    public void Terminate()
    {
        foreach (var obstacle in _obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        _obstacles.Clear();
    }

    private void CreateGridCells(float localMinX, float localMinZ)
    {
        _gridStorage.Clear();

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                float x = localMinX + (_objectWidth / 2f) + col * (_objectWidth + _spacingX);
                float z = localMinZ + (_objectDepth / 2f) + row * (_objectDepth + _spacingZ);

                GridCell cell = Instantiate(_cellPrefab, transform);
                cell.transform.position = new Vector3(x, _gridCenterOffset.y + cell.transform.localScale.y, z);

                _cellGrid[row, col] = cell;
                _gridStorage.Add(cell);
            }
        }
    }

    private void CreateAllObstacles()
    {
        foreach (var obs in _obstacles)
        {
            if (obs != null) Destroy(obs.gameObject);
        }
        _obstacles.Clear();

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (row == _rows - 1) continue;
                if (_obstacleMap[row, col])
                    CreateSingleObstacle(row, col);
            }
        }
    }

    private void CreateSingleObstacle(int row, int col)
    {
        GridCell cell = _cellGrid[row, col];
        Obstacle obstacle = Instantiate(_obstaclePrefab, _container);

        Vector3 spawnPoint = cell.transform.position;
        spawnPoint.y += obstacle.transform.localScale.y;
        obstacle.transform.position = spawnPoint;

        _obstacles.Add(obstacle);

        var initMethod = cell.GetType().GetMethod("InitObstacle");
        initMethod?.Invoke(cell, new object[] { obstacle });
    }

    private void CreateStretchedObstaclesBetweenNeighbors()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (row == _rows - 1) continue;
                if (_obstacleMap[row, col])
                {
                    CheckHorizontalNeighbors(row, col);
                    if (row < _rows - 2) CheckVerticalNeighbors(row, col);

                    if (col == 0)
                        CreateHorizontalStretchedObstacle(_cellGrid[row, col].transform.position,
                                                          _cellGrid[row, col].transform.position + Vector3.left * (_objectWidth + _spacingX));
                    else if (col == _columns - 1)
                        CreateHorizontalStretchedObstacle(_cellGrid[row, col].transform.position,
                                                          _cellGrid[row, col].transform.position + Vector3.right * (_objectWidth + _spacingX));

                    if (row == 0)
                        CreateVerticalStretchedObstacle(_cellGrid[row, col].transform.position,
                                                        _cellGrid[row, col].transform.position + Vector3.back * (_objectDepth + _spacingZ));
                }
            }
        }
    }

    private void CheckHorizontalNeighbors(int row, int col)
    {
        if (col < _columns - 1 && _obstacleMap[row, col + 1])
            CreateHorizontalStretchedObstacle(_cellGrid[row, col].transform.position, _cellGrid[row, col + 1].transform.position);
    }

    private void CheckVerticalNeighbors(int row, int col)
    {
        if (row < _rows - 2 && _obstacleMap[row + 1, col])
            CreateVerticalStretchedObstacle(_cellGrid[row, col].transform.position, _cellGrid[row + 1, col].transform.position);
    }

    private void CreateHorizontalStretchedObstacle(Vector3 startPos, Vector3 endPos)
    {
        Vector3 centerPosition = Vector3.Lerp(startPos, endPos, 0.5f);

        Obstacle obstacle = Instantiate(_stretchedObstaclePrefab, _container);

        Vector3 obstacleScale = obstacle.transform.localScale;
        obstacleScale.x = Vector3.Distance(startPos, endPos);
        obstacle.transform.localScale = obstacleScale;

        centerPosition.y += obstacle.transform.localScale.y;
        obstacle.transform.position = centerPosition;

        _obstacles.Add(obstacle);
    }

    private void CreateVerticalStretchedObstacle(Vector3 startPos, Vector3 endPos)
    {
        Vector3 centerPosition = Vector3.Lerp(startPos, endPos, 0.5f);

        Obstacle obstacle = Instantiate(_stretchedObstaclePrefab, _container);

        obstacle.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        Vector3 obstacleScale = obstacle.transform.localScale;
        obstacleScale.x = Vector3.Distance(startPos, endPos);
        obstacle.transform.localScale = obstacleScale;

        centerPosition.y += obstacle.transform.localScale.y;
        obstacle.transform.position = centerPosition;

        _obstacles.Add(obstacle);
    }

    private bool[,] GenerateComplexObstacleMap()
    {
        bool[,] map = new bool[_rows, _columns];
        int total = 0;
        int groups = Random.Range(1, Mathf.Min(4, _rows / 2 + 1));

        for (int g = 0; g < groups; g++)
        {
            if (total >= _maxObstacles) break;

            int row = Random.Range(1, _rows - 1);
            int startCol = Random.Range(0, _columns / 2 - 1);
            int length = Random.Range(1, Mathf.Min(_maxObstacleLength + 1, _columns / 2 - startCol + 1));

            for (int i = 0; i < length; i++)
            {
                if (startCol + i < _columns / 2 && total < _maxObstacles)
                {
                    map[row, startCol + i] = true;
                    map[row, _columns - 1 - (startCol + i)] = true;
                    total += 2;
                }
            }
        }

        return map;
    }
}