using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private PlayerCube _cubePrefab;
    [SerializeField] private GridCell _cellPrefab;
    [SerializeField] private Obstracle _obstaclePrefab;
    [SerializeField] private Obstracle _stretchedObstaclePrefab;

    [SerializeField] private Vector3 _centerPosition;
    [SerializeField] private int _rows = 7;
    [SerializeField] private int _columns = 8;
    [SerializeField] private float _cellSpacingX = 1.2f;
    [SerializeField] private float _cellSpacingZ = 1.2f;
    [SerializeField] private float _cellHeightOffset = 0.01f;

    [SerializeField] private bool _isCreateObstacles;
    [SerializeField] private int _maxObstacles = 6;
    [SerializeField] private int _maxObstacleLength = 3;
    [SerializeField] private Transform _obstaclesContainer;
    [SerializeField] private Transform _walls;

    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private BoundaryMaker _boundaryMaker;

    private List<Obstracle> _obstacles;
    private GridCell[,] _cellGrid;
    private bool[,] _obstacleMap;
    private Vector3[,] _cellPositions;

    private float _objectWidth;
    private float _objectDepth;

    public event Action Created;

    private void Awake()
    {
        _obstacles = new List<Obstracle>();
    }

    private void Start()
    {
        _objectWidth = _cubePrefab.transform.localScale.x;
        _objectDepth = _cubePrefab.transform.localScale.z;
    }

    public bool TryCreate()
    {
        Terminate();

        _obstacleMap = GenerateComplexObstacleMap();
        _cellGrid = new GridCell[_rows, _columns];
        _cellPositions = new Vector3[_rows, _columns];

        float gridWidth = _columns * _objectWidth + (_columns - 1) * _cellSpacingX;
        float gridDepth = _rows * _objectDepth + (_rows - 1) * _cellSpacingZ;

        Vector3 gridStart = _centerPosition - new Vector3(gridWidth / 2f, 0f, gridDepth / 2f);

        CreateGridCells(gridStart);

        bool shouldCreateObstacles = _isCreateObstacles && UserUtils.GetIntRandomNumber(0, 2) == 1;

        if (shouldCreateObstacles)
        {
            CreateAllObstacles();
            CreateStretchedObstaclesBetweenNeighbors();

            if (_walls != null)
            {
                _walls.position = _centerPosition;
                _walls.gameObject.SetActive(true);
            }
        }

        if (_gridStorage.GridCount == 0)
        {
            return false;
        }

        _gridStorage.CreateCells(_rows, _columns);
        Created?.Invoke();

        return true;
    }

    public void Terminate()
    {
        foreach (var cell in _gridStorage.GetAllCells)
        {
            if (cell != null && cell.gameObject != null)
                Destroy(cell.gameObject);
        }

        foreach (var obstacle in _obstacles)
        {
            if (obstacle != null && obstacle.gameObject != null)
                Destroy(obstacle.gameObject);
        }

        if (_walls != null)
        {
            _walls.gameObject.SetActive(false);
        }

        _obstacles.Clear();
        _gridStorage.Clear();
    }

    private void CreateGridCells(Vector3 gridStart)
    {
        _gridStorage.Clear();

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                float x = gridStart.x + col * (_objectWidth + _cellSpacingX) + _objectWidth / 2f;
                float z = gridStart.z + row * (_objectDepth + _cellSpacingZ) + _objectDepth / 2f;
                float y = _centerPosition.y + _cellHeightOffset;

                Vector3 position = new(x, y, z);

                _cellPositions[row, col] = position;

                GridCell cell = Instantiate(_cellPrefab, transform);
                cell.transform.SetPositionAndRotation(position, Quaternion.identity);

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
        if (cell == null) return;

        Transform parent = _obstaclesContainer != null ? _obstaclesContainer : transform;
        Obstracle obstacle = Instantiate(_obstaclePrefab, parent);

        obstacle.transform.position = cell.transform.position;
        obstacle.transform.position += Vector3.up * obstacle.transform.localScale.y;

        _obstacles.Add(obstacle);
        cell.InitObstacle(obstacle);
    }

    private void CreateStretchedObstaclesBetweenNeighbors()
    {
        if (_stretchedObstaclePrefab == null) return;

        Transform parent = _obstaclesContainer != null ? _obstaclesContainer : transform;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (!_obstacleMap[row, col]) continue;

                if (col < _columns - 1 && _obstacleMap[row, col + 1])
                {
                    CreateHorizontalStretchedObstacle(
                        _cellPositions[row, col],
                        _cellPositions[row, col + 1],
                        parent
                    );
                }

                if (row < _rows - 2 && _obstacleMap[row + 1, col])
                {
                    CreateVerticalStretchedObstacle(
                        _cellPositions[row, col],
                        _cellPositions[row + 1, col],
                        parent
                    );
                }

                if (col == 0)
                {
                    CreateHorizontalStretchedObstacle(
                        _cellPositions[row, col],
                        _cellPositions[row, col] + Vector3.left * (_objectWidth + _cellSpacingX),
                        parent
                    );
                }
                else if (col == _columns - 1)
                {
                    CreateHorizontalStretchedObstacle(
                        _cellPositions[row, col],
                        _cellPositions[row, col] + Vector3.right * (_objectWidth + _cellSpacingX),
                        parent
                    );
                }

                if (row == 0)
                {
                    CreateVerticalStretchedObstacle(
                        _cellPositions[row, col],
                        _cellPositions[row, col] + Vector3.back * (_objectDepth + _cellSpacingZ),
                        parent
                    );
                }
            }
        }
    }

    private void CreateHorizontalStretchedObstacle(Vector3 startPos, Vector3 endPos, Transform parent)
    {
        Vector3 centerPosition = (startPos + endPos) / 2f;
        centerPosition.y = startPos.y + _stretchedObstaclePrefab.transform.localScale.y;

        Obstracle obstacle = Instantiate(_stretchedObstaclePrefab, parent);

        float distance = Vector3.Distance(startPos, endPos);
        Vector3 scale = obstacle.transform.localScale;
        scale.x = distance;
        obstacle.transform.localScale = scale;

        obstacle.transform.position = centerPosition;

        _obstacles.Add(obstacle);
    }

    private void CreateVerticalStretchedObstacle(Vector3 startPos, Vector3 endPos, Transform parent)
    {
        Vector3 centerPosition = (startPos + endPos) / 2f;
        centerPosition.y = startPos.y + _stretchedObstaclePrefab.transform.localScale.y;

        Obstracle obstacle = Instantiate(_stretchedObstaclePrefab, parent);

        float distance = Vector3.Distance(startPos, endPos);
        Vector3 scale = obstacle.transform.localScale;
        scale.x = distance;
        obstacle.transform.localScale = scale;

        obstacle.transform.SetPositionAndRotation(centerPosition, Quaternion.Euler(0f, 90f, 0f));

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