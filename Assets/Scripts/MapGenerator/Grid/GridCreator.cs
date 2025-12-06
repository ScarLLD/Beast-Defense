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
    [SerializeField] private int _rows = 7;
    [SerializeField] private int _columns = 8;
    [SerializeField] private float _minZ = -12;
    [SerializeField] private float _maxZ = 10;

    [Header("Width & Offsets")]
    [SerializeField] private float _gridExtraWidth = -4f;
    [SerializeField] private float _wallOffsetX = 1f;
    [SerializeField] private float _offsetY = 1.5f;

    [Header("Obstacles settings")]
    [SerializeField] private bool _isCreateObstacles;
    [SerializeField] private int _maxObstacles = 6;
    [SerializeField] private int _maxObstacleLength = 3;
    [SerializeField] private Transform _container;

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
    private float _minXScreen;
    private float _maxXScreen;

    public event Action Created;

    private void Awake()
    {
        _obstacles = new List<Obstacle>();
    }

    private void Start()
    {
        if (_boundaryMaker.TryGetScreenBottomCenter(out Vector3 bottomScreenCenter))
        {
            transform.position = bottomScreenCenter;
            _gridCenterOffset = transform.position;
        }

        _objectWidth = _cubePrefab.transform.localScale.x;
        _objectDepth = _cubePrefab.transform.localScale.z;

        SetGridWidthByScreen();
    }

    private void SetGridWidthByScreen()
    {
        if (_boundaryMaker.TryGetScreenWidthBounds(out float minX, out float maxX, _gridExtraWidth))
        {
            _minXScreen = minX;
            _maxXScreen = maxX;
        }
    }

    public bool TryCreate(out Vector3 cubeScale)
    {
        cubeScale = _cubePrefab.transform.localScale;

        float localMinX = _minXScreen + _gridCenterOffset.x;
        float localMaxX = _maxXScreen + _gridCenterOffset.x;
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
            CreateBorderWalls(localMinX, localMaxX, localMinZ);
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

    private void CreateBorderWalls(float localMinX, float localMaxX, float localMinZ)
    {
        CreateCornerWalls(localMinX, localMaxX, localMinZ);
        CreateLeftBorderWalls(localMinX, localMinZ);
        CreateRightBorderWalls(localMaxX, localMinZ);
        CreateBottomBorderWalls();
    }

    private void CreateCornerWalls(float localMinX, float localMaxX, float localMinZ)
    {
        float bottomZ = localMinZ - _objectDepth / 2f - _spacingZ - _offsetY;

        _leftCornerPosition = new Vector3(localMinX - _objectWidth / 2f - _wallOffsetX, 0f, bottomZ);
        Obstacle leftCorner = Instantiate(_obstaclePrefab, _container);

        Vector3 leftCornerPosition = _leftCornerPosition;
        leftCornerPosition.y = leftCorner.transform.localScale.y / 2;
        leftCorner.transform.position = leftCornerPosition;
        _obstacles.Add(leftCorner);

        _rightCornerPosition = new Vector3(localMaxX + _objectWidth / 2f + _wallOffsetX, 0f, bottomZ);
        Obstacle rightCorner = Instantiate(_obstaclePrefab, _container);

        Vector3 rightCornerPosition = _rightCornerPosition;
        rightCornerPosition.y = rightCorner.transform.localScale.y / 2;
        rightCorner.transform.position = rightCornerPosition;
        _obstacles.Add(rightCorner);
    }

    private void CreateLeftBorderWalls(float localMinX, float localMinZ)
    {
        float leftX = localMinX - _objectWidth / 2f - _wallOffsetX;
        Vector3 topPosition = new(leftX, 0f, localMinZ + (_objectDepth / 2f) + (_rows - 1) * (_objectDepth + _spacingZ));
        Obstacle topObstacle = Instantiate(_obstaclePrefab, _container);

        topPosition.y = topObstacle.transform.localScale.y / 2;
        topObstacle.transform.position = topPosition;
        _obstacles.Add(topObstacle);

        CreateVerticalStretchedObstacle(_leftCornerPosition, topPosition);
    }

    private void CreateRightBorderWalls(float localMaxX, float localMinZ)
    {
        float rightX = localMaxX + _objectWidth / 2f + _wallOffsetX;
        Vector3 topPosition = new(rightX, 0f, localMinZ + (_objectDepth / 2f) + (_rows - 1) * (_objectDepth + _spacingZ));
        Obstacle topObstacle = Instantiate(_obstaclePrefab, _container);

        topPosition.y = topObstacle.transform.localScale.y / 2;
        topObstacle.transform.position = topPosition;
        _obstacles.Add(topObstacle);

        CreateVerticalStretchedObstacle(_rightCornerPosition, topPosition);
    }

    private void CreateBottomBorderWalls()
    {
        int leftmostCol = -1;
        int rightmostCol = -1;

        for (int col = 0; col < _columns; col++)
        {
            if (_obstacleMap[0, col])
            {
                if (leftmostCol == -1) leftmostCol = col;
                rightmostCol = col;
            }
        }

        if (leftmostCol != -1)
        {
            Vector3 leftPos = _cellGrid[0, leftmostCol].transform.position; leftPos.y = 0f;
            Vector3 rightPos = _cellGrid[0, rightmostCol].transform.position; rightPos.y = 0f;

            CreateHorizontalStretchedObstacle(_leftCornerPosition, leftPos);
            CreateHorizontalStretchedObstacle(rightPos, _rightCornerPosition);

            if (rightmostCol - leftmostCol > 0)
                CreateHorizontalStretchedObstacle(leftPos, rightPos);
        }
        else
        {
            CreateHorizontalStretchedObstacle(_leftCornerPosition, _rightCornerPosition);
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
        spawnPoint.y = obstacle.transform.localScale.y / 2;
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

        centerPosition.y = obstacle.transform.localScale.y / 2f;
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

        centerPosition.y = obstacle.transform.localScale.y / 2f;
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