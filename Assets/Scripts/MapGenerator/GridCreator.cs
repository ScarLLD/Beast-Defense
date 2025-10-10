using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private PlayerCube _cubePrefab;
    [SerializeField] private GridCell _cellPrefab;
    [SerializeField] private Obstacle _obstaclePrefab;
    [SerializeField] private Obstacle _stretchedObstaclePrefab;
    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private BoundaryMaker _boundaryMaker;

    [SerializeField] private int _rows;
    [SerializeField] private int _columns;

    [Header("Grid position settings")]
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxZ;

    [Header("Obstacles settings")]
    [SerializeField] private bool CreateObstacles;
    [SerializeField] private int _maxObstacles = 6;
    [SerializeField] private int _maxObstacleLength = 3;
    [SerializeField] private float _offsetX;
    [SerializeField] private float _offsetY;

    private List<Obstacle> _obstacles;
    private float _objectWidth;
    private float _objectDepth;
    private bool[,] _obstacleMap;
    private GridCell[,] _cellGrid;
    private float _spacingX;
    private float _spacingZ;
    private Vector3 _gridCenterOffset;
    private Vector3 _leftCornerPos;
    private Vector3 _rightCornerPos;

    public event Action Created;

    private void Awake()
    {
        _obstacles = new();
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

        if (CreateObstacles)
        {
            CreateAllObstacles();
            CreateStretchedObstaclesBetweenNeighbors();
            CreateBorderWalls(localMinX, localMaxX, localMinZ, localMaxZ);
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

    private void CreateGridCells(float localMinX, float localMinZ)
    {
        _gridStorage.Clear();

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                float localX = localMinX + (_objectWidth / 2) + col * (_objectWidth + _spacingX);
                float localZ = localMinZ + (_objectDepth / 2) + row * (_objectDepth + _spacingZ);

                GridCell gridCell = Instantiate(_cellPrefab, transform);
                Vector3 spawnPosition = new(localX, _gridCenterOffset.y + gridCell.transform.localScale.y, localZ);
                gridCell.transform.position = spawnPosition;

                _cellGrid[row, col] = gridCell;
                _gridStorage.Add(gridCell);
            }
        }
    }

    private void CreateBorderWalls(float localMinX, float localMaxX, float localMinZ, float localMaxZ)
    {
        // Сначала создаем угловые препятствия и запоминаем их позиции
        CreateCornerWalls(localMinX, localMaxX, localMinZ);

        // Затем создаем стены, которые будут соединяться с угловыми препятствиями
        CreateLeftBorderWalls(localMinX, localMinZ, localMaxZ);
        CreateRightBorderWalls(localMaxX, localMinZ, localMaxZ);
        CreateBottomBorderWalls(localMinX, localMaxX, localMinZ);
    }

    private void CreateCornerWalls(float localMinX, float localMaxX, float localMinZ)
    {
        float bottomBorderZ = localMinZ - _objectDepth - _spacingZ + _offsetY;

        // Левый нижний угол
        _leftCornerPos = new Vector3(localMinX - _objectWidth - _spacingX + _offsetX, 0f, bottomBorderZ);
        Obstacle leftCorner = Instantiate(_obstaclePrefab, transform);
        leftCorner.transform.position = new Vector3(
            _leftCornerPos.x,
            leftCorner.transform.position.y + leftCorner.transform.localScale.y / 2,
            _leftCornerPos.z
        );
        _obstacles.Add(leftCorner);

        // Правый нижний угол
        _rightCornerPos = new Vector3(localMaxX + _objectWidth + _spacingX - _offsetX, 0f, bottomBorderZ);
        Obstacle rightCorner = Instantiate(_obstaclePrefab, transform);
        rightCorner.transform.position = new Vector3(
            _rightCornerPos.x,
            rightCorner.transform.position.y + rightCorner.transform.localScale.y / 2,
            _rightCornerPos.z
        );
        _obstacles.Add(rightCorner);
    }

    private void CreateLeftBorderWalls(float localMinX, float localMinZ, float localMaxZ)
    {
        float leftBorderX = localMinX - _objectWidth - _spacingX + _offsetX;

        // Верхний угол левой стены
        Vector3 topPos = new Vector3(leftBorderX, 0f, localMinZ + (_objectDepth / 2) + (_rows - 1) * (_objectDepth + _spacingZ));
        Obstacle topObstacle = Instantiate(_obstaclePrefab, transform);
        topObstacle.transform.position = new Vector3(
            topPos.x,
            topObstacle.transform.position.y + topObstacle.transform.localScale.y / 2,
            topPos.z
        );
        _obstacles.Add(topObstacle);

        // Создаем растянутое препятствие от нижнего углового препятствия до верхнего
        CreateVerticalStretchedObstacle(_leftCornerPos, topPos);
    }

    private void CreateRightBorderWalls(float localMaxX, float localMinZ, float localMaxZ)
    {
        float rightBorderX = localMaxX + _objectWidth + _spacingX - _offsetX;

        // Верхний угол правой стены
        Vector3 topPos = new Vector3(rightBorderX, 0f, localMinZ + (_objectDepth / 2) + (_rows - 1) * (_objectDepth + _spacingZ));
        Obstacle topObstacle = Instantiate(_obstaclePrefab, transform);
        topObstacle.transform.position = new Vector3(
            topPos.x,
            topObstacle.transform.position.y + topObstacle.transform.localScale.y / 2,
            topPos.z
        );
        _obstacles.Add(topObstacle);

        // Создаем растянутое препятствие от нижнего углового препятствия до верхнего
        CreateVerticalStretchedObstacle(_rightCornerPos, topPos);
    }

    private void CreateBottomBorderWalls(float localMinX, float localMaxX, float localMinZ)
    {
        float bottomBorderZ = localMinZ - _objectDepth - _spacingZ + _offsetY;

        // Находим крайние препятствия в нижнем ряду сетки
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
            // Есть препятствия в нижнем ряду - соединяем углы с ними
            Vector3 leftObstaclePos = _cellGrid[0, leftmostCol].transform.position;
            leftObstaclePos.y = 0f;

            Vector3 rightObstaclePos = _cellGrid[0, rightmostCol].transform.position;
            rightObstaclePos.y = 0f;

            // От левого угла до левого препятствия
            CreateHorizontalStretchedObstacle(_leftCornerPos, leftObstaclePos);

            // От правого препятствия до правого угла
            CreateHorizontalStretchedObstacle(rightObstaclePos, _rightCornerPos);

            // Между препятствиями (если они не соседние)
            if (rightmostCol - leftmostCol > 0)
            {
                CreateHorizontalStretchedObstacle(leftObstaclePos, rightObstaclePos);
            }
        }
        else
        {
            // Нет препятствий в нижнем ряду - просто соединяем углы
            CreateHorizontalStretchedObstacle(_leftCornerPos, _rightCornerPos);
        }
    }

    private void CreateHorizontalStretchedObstacle(Vector3 position1, Vector3 position2)
    {
        Vector3 centerPosition = Vector3.Lerp(position1, position2, 0.5f);

        Obstacle stretchedObstacle = Instantiate(_stretchedObstaclePrefab, transform);
        stretchedObstacle.transform.position = new Vector3(
            centerPosition.x,
            stretchedObstacle.transform.position.y + stretchedObstacle.transform.localScale.y / 2,
            centerPosition.z
        );

        _obstacles.Add(stretchedObstacle);

        float distance = Vector3.Distance(position1, position2);
        Vector3 newScale = stretchedObstacle.transform.localScale;
        newScale.x = distance;
        stretchedObstacle.transform.localScale = newScale;
    }

    private void CreateVerticalStretchedObstacle(Vector3 position1, Vector3 position2)
    {
        Vector3 centerPosition = Vector3.Lerp(position1, position2, 0.5f);
        centerPosition.y += _stretchedObstaclePrefab.transform.localScale.y / 2;

        Obstacle stretchedObstacle = Instantiate(_stretchedObstaclePrefab, centerPosition, Quaternion.identity);

        Quaternion newRotation = Quaternion.Euler(0f, 90f, 0f);
        stretchedObstacle.transform.rotation = newRotation;

        _obstacles.Add(stretchedObstacle);

        float distance = Vector3.Distance(position1, position2);
        Vector3 newScale = stretchedObstacle.transform.localScale;
        newScale.x = distance;
        stretchedObstacle.transform.localScale = newScale;
    }

    private void CreateAllObstacles()
    {
        if (_obstacles.Count > 0)
        {
            foreach (var obstacle in _obstacles)
            {
                if (obstacle != null)
                    Destroy(obstacle.gameObject);
            }

            _obstacles.Clear();
        }

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (row == _rows - 1)
                    continue;

                if (_obstacleMap[row, col])
                {
                    CreateSingleObstacle(row, col);
                }
            }
        }
    }

    private void CreateSingleObstacle(int row, int col)
    {
        GridCell gridCell = _cellGrid[row, col];
        Obstacle obstacle = Instantiate(_obstaclePrefab, transform);
        obstacle.transform.position = new Vector3(
            gridCell.transform.position.x,
            obstacle.transform.position.y + obstacle.transform.localScale.y / 2,
            gridCell.transform.position.z
        );
        _obstacles.Add(obstacle);

        // Если в GridCell есть метод InitObstacle, вызываем его
        var initMethod = gridCell.GetType().GetMethod("InitObstacle");
        if (initMethod != null)
        {
            initMethod.Invoke(gridCell, new object[] { obstacle });
        }
    }

    private void CreateStretchedObstaclesBetweenNeighbors()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (row == _rows - 1)
                    continue;

                if (_obstacleMap[row, col])
                {
                    CheckHorizontalNeighbors(row, col);

                    if (row < _rows - 2)
                        CheckVerticalNeighbors(row, col);

                    if (col == 0)
                    {
                        Vector3 currentPos = _cellGrid[row, col].transform.position;
                        Vector3 outsidePos = currentPos + Vector3.left * (_objectWidth + _spacingX);
                        CreateHorizontalStretchedObstacle(currentPos, outsidePos);
                    }
                    else if (col == _columns - 1)
                    {
                        Vector3 currentPos = _cellGrid[row, col].transform.position;
                        Vector3 outsidePos = currentPos + Vector3.right * (_objectWidth + _spacingX);
                        CreateHorizontalStretchedObstacle(currentPos, outsidePos);
                    }

                    if (row == 0)
                    {
                        Vector3 currentPos = _cellGrid[row, col].transform.position;
                        Vector3 outsidePos = currentPos + Vector3.back * (_objectDepth + _spacingZ);
                        CreateVerticalStretchedObstacle(currentPos, outsidePos);
                    }
                }
            }
        }
    }

    private void CheckHorizontalNeighbors(int row, int col)
    {
        if (col < _columns - 1 && _obstacleMap[row, col + 1])
        {
            CreateHorizontalStretchedObstacle(
                _cellGrid[row, col].transform.position,
                _cellGrid[row, col + 1].transform.position
            );
        }
    }

    private void CheckVerticalNeighbors(int row, int col)
    {
        if (row < _rows - 2 && _obstacleMap[row + 1, col])
        {
            CreateVerticalStretchedObstacle(
                _cellGrid[row, col].transform.position,
                _cellGrid[row + 1, col].transform.position
            );
        }
    }

    private bool[,] GenerateComplexObstacleMap()
    {
        bool[,] obstacleMap = new bool[_rows, _columns];
        int totalObstacles = 0;

        int obstacleGroups = Random.Range(1, Mathf.Min(4, _rows / 2 + 1));

        for (int group = 0; group < obstacleGroups; group++)
        {
            if (totalObstacles >= _maxObstacles)
                break;

            int row = Random.Range(1, _rows - 1);

            int startCol = Random.Range(0, _columns / 2 - 1);
            int obstacleLength = Random.Range(1, Mathf.Min(_maxObstacleLength + 1, _columns / 2 - startCol + 1));

            for (int i = 0; i < obstacleLength; i++)
            {
                if (startCol + i < _columns / 2 && totalObstacles < _maxObstacles)
                {
                    obstacleMap[row, startCol + i] = true;
                    obstacleMap[row, _columns - 1 - (startCol + i)] = true;
                    totalObstacles += 2;
                }
            }
        }

        return obstacleMap;
    }
}