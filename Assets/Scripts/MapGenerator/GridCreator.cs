using System;
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
    [SerializeField] private float _offsetX;
    [SerializeField] private float _offsetY;
    [SerializeField] private int _additionalCellsCount;

    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxZ;

    [SerializeField] private int _maxObstacles = 6;
    [SerializeField] private int _maxObstacleLength = 3;
    [SerializeField][Range(0f, 1f)] private float _obstacleChance = 0.3f;

    private float _objectWidth;
    private float _objectDepth;
    private bool[,] _obstacleMap;
    private GridCell[,] _cellGrid;
    private float _spacingX;
    private float _spacingZ;
    private Vector3 _gridCenterOffset;

    public event Action Created;

    private void Start()
    {
        if (_boundaryMaker.TryGetScreenBottomCenter(out Vector3 bottomScreenCenter))
        {
            transform.position = new Vector3(bottomScreenCenter.x, bottomScreenCenter.y, bottomScreenCenter.z);
            _gridCenterOffset = transform.position; // Сохраняем смещение
        }

        _objectWidth = _cubePrefab.transform.localScale.x;
        _objectDepth = _cubePrefab.transform.localScale.z;
    }

    public bool TryCreate(out Vector3 cubeScale)
    {
        cubeScale = _cubePrefab.transform.localScale;

        // Используем локальные координаты относительно позиции GridCreator
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

        // Создаем карту препятствий
        _obstacleMap = GenerateComplexObstacleMap();
        _cellGrid = new GridCell[_rows, _columns];

        // Сначала создаем все ячейки
        CreateGridCells(localMinX, localMinZ);

        // Затем создаем все препятствия внутри сетки
        CreateAllObstacles();

        // Создаем растянутые препятствия между соседями и на краях
        CreateStretchedObstaclesBetweenNeighbors();

        // Создаем стенки вокруг сетки
        CreateBorderWalls(localMinX, localMaxX, localMinZ, localMaxZ);

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
        CreateLeftBorderWalls(localMinX, localMinZ);
        CreateRightBorderWalls(localMaxX, localMinZ);
        CreateBottomBorderWalls(localMinX, localMinZ);
        CreateCornerWalls(localMinX, localMaxX, localMinZ);
    }

    private void CreateCornerWalls(float localMinX, float localMaxX, float localMinZ)
    {
        float bottomBorderZ = localMinZ - _objectDepth - _spacingZ + _offsetY;

        // --- Левый нижний угол ---
        Vector3 leftCornerPos = new Vector3(localMinX - _objectWidth - _spacingX + _offsetX, 0f, bottomBorderZ);
        Obstacle leftCorner = Instantiate(_obstaclePrefab, transform);
        leftCorner.transform.position = new Vector3(
            leftCornerPos.x,
            leftCorner.transform.position.y + leftCorner.transform.localScale.y / 2,
            leftCornerPos.z
        );

        // Вертикальная стенка вверх до нижнего ряда сетки
        Vector3 leftVerticalEnd = new Vector3(leftCornerPos.x, 0f, _cellGrid[0, 0].transform.position.z);
        CreateVerticalStretchedObstacle(leftCornerPos, leftVerticalEnd);

        // Горизонтальная стенка вправо до левой границы сетки
        Vector3 leftHorizontalEnd = new Vector3(_cellGrid[0, 0].transform.position.x, 0f, leftCornerPos.z);
        CreateHorizontalStretchedObstacle(leftCornerPos, leftHorizontalEnd);

        // --- Правый нижний угол ---
        Vector3 rightCornerPos = new Vector3(localMaxX + _objectWidth + _spacingX - _offsetX, 0f, bottomBorderZ);
        Obstacle rightCorner = Instantiate(_obstaclePrefab, transform);
        rightCorner.transform.position = new Vector3(
            rightCornerPos.x,
            rightCorner.transform.position.y + rightCorner.transform.localScale.y / 2,
            rightCornerPos.z
        );

        // Вертикальная стенка вверх до нижнего ряда сетки
        Vector3 rightVerticalEnd = new Vector3(rightCornerPos.x, 0f, _cellGrid[0, _columns - 1].transform.position.z);
        CreateVerticalStretchedObstacle(rightCornerPos, rightVerticalEnd);

        // Горизонтальная стенка влево до правой границы сетки
        Vector3 rightHorizontalEnd = new Vector3(_cellGrid[0, _columns - 1].transform.position.x, 0f, rightCornerPos.z);
        CreateHorizontalStretchedObstacle(rightCornerPos, rightHorizontalEnd);
    }

    private void CreateLeftBorderWalls(float localMinX, float localMinZ)
    {
        float leftBorderX = localMinX - _objectWidth - _spacingX + _offsetX;

        for (int row = 0; row < _rows + _additionalCellsCount; row++)
        {
            float localZ = localMinZ + (_objectDepth / 2) + row * (_objectDepth + _spacingZ);
            Vector3 position = new Vector3(leftBorderX, 0f, localZ);

            Obstacle obstacle = Instantiate(_obstaclePrefab, transform);
            obstacle.transform.position = new Vector3(
                position.x,
                obstacle.transform.position.y + obstacle.transform.localScale.y / 2,
                position.z
            );

            if (row < _rows - 1 + _additionalCellsCount)
            {
                float nextLocalZ = localMinZ + (_objectDepth / 2) + (row + 1) * (_objectDepth + _spacingZ);
                Vector3 nextPosition = new Vector3(leftBorderX, 0f, nextLocalZ);
                CreateVerticalStretchedObstacle(position, nextPosition);
            }
        }
    }

    private void CreateRightBorderWalls(float localMaxX, float localMinZ)
    {
        float rightBorderX = localMaxX + _objectWidth + _spacingX - _offsetX;

        for (int row = 0; row < _rows + _additionalCellsCount; row++)
        {
            float localZ = localMinZ + (_objectDepth / 2) + row * (_objectDepth + _spacingZ);
            Vector3 position = new Vector3(rightBorderX, 0f, localZ);

            Obstacle obstacle = Instantiate(_obstaclePrefab, transform);
            obstacle.transform.position = new Vector3(
                position.x,
                obstacle.transform.position.y + obstacle.transform.localScale.y / 2,
                position.z
            );

            if (row < _rows - 1 + _additionalCellsCount)
            {
                float nextLocalZ = localMinZ + (_objectDepth / 2) + (row + 1) * (_objectDepth + _spacingZ);
                Vector3 nextPosition = new Vector3(rightBorderX, 0f, nextLocalZ);
                CreateVerticalStretchedObstacle(position, nextPosition);
            }
        }
    }

    private void CreateBottomBorderWalls(float localMinX, float localMinZ)
    {
        float bottomBorderZ = localMinZ - _objectDepth - _spacingZ + _offsetY;

        for (int col = 0; col < _columns + _additionalCellsCount; col++)
        {
            float localX = localMinX + (_objectWidth / 2) + col * (_objectWidth + _spacingX);
            Vector3 position = new Vector3(localX, 0f, bottomBorderZ);

            Obstacle obstacle = Instantiate(_obstaclePrefab, transform);
            obstacle.transform.position = new Vector3(
                position.x,
                obstacle.transform.position.y + obstacle.transform.localScale.y / 2,
                position.z
            );

            if (col < _columns - 1)
            {
                float nextLocalX = localMinX + (_objectWidth / 2) + (col + 1) * (_objectWidth + _spacingX);
                Vector3 nextPosition = new Vector3(nextLocalX, 0f, bottomBorderZ);
                CreateHorizontalStretchedObstacle(position, nextPosition);
            }
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

        float distance = Vector3.Distance(position1, position2);
        Vector3 newScale = stretchedObstacle.transform.localScale;
        newScale.x = distance;
        stretchedObstacle.transform.localScale = newScale;
    }

    private void CreateVerticalStretchedObstacle(Vector3 position1, Vector3 position2)
    {
        Vector3 centerPosition = Vector3.Lerp(position1, position2, 0.5f);

        Obstacle stretchedObstacle = Instantiate(_stretchedObstaclePrefab, transform);
        stretchedObstacle.transform.position = new Vector3(
            centerPosition.x,
            stretchedObstacle.transform.position.y + stretchedObstacle.transform.localScale.y / 2,
            centerPosition.z
        );

        float distance = Vector3.Distance(position1, position2);
        Vector3 newScale = stretchedObstacle.transform.localScale;
        newScale.z = distance;
        stretchedObstacle.transform.localScale = newScale;
    }

    private void CreateAllObstacles()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
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
        gridCell.InitObstacle(obstacle);
    }

    private void CreateStretchedObstaclesBetweenNeighbors()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (_obstacleMap[row, col])
                {
                    CheckHorizontalNeighbors(row, col);

                    if (row < _rows - 1)
                        CheckVerticalNeighbors(row, col);

                    if (col == 0) // крайний левый
                    {
                        Vector3 currentPos = _cellGrid[row, col].transform.position;
                        Vector3 outsidePos = currentPos + Vector3.left * (_objectWidth + _spacingX);
                        CreateHorizontalStretchedObstacle(currentPos, outsidePos);
                    }
                    else if (col == _columns - 1) // крайний правый
                    {
                        Vector3 currentPos = _cellGrid[row, col].transform.position;
                        Vector3 outsidePos = currentPos + Vector3.right * (_objectWidth + _spacingX);
                        CreateHorizontalStretchedObstacle(currentPos, outsidePos);
                    }

                    if (row == 0) // нижний ряд
                    {
                        Vector3 currentPos = _cellGrid[row, col].transform.position;
                        Vector3 outsidePos = currentPos + Vector3.back * (_objectDepth + _spacingZ);
                        CreateVerticalStretchedObstacle(currentPos, outsidePos);
                    }
                    // верхний ряд игнорируем
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
        if (row < _rows - 1 && _obstacleMap[row + 1, col])
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

            // Не берем нижний ряд
            int row = Random.Range(1, _rows); // row = 1.._rows-1 (0-й ряд исключаем)

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
