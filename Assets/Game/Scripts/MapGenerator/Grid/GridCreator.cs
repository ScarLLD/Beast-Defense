using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core;
using Game.Scripts.CubeCore;
using Game.Scripts.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.MapGenerator.Grid
{
    public class GridCreator : MonoBehaviour
    {
        [SerializeField] private PlayerCube _cubePrefab;
        [SerializeField] private GridCell _cellPrefab;
        [SerializeField] private Obstacle _obstaclePrefab;
        [SerializeField] private Obstacle _stretchedObstaclePrefab;

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
        private GridCell[,] _cellGrid;
        private Vector3[,] _cellPositions;
        private float _objectDepth;

        private float _objectWidth;
        private bool[,] _obstacleMap;

        private List<Obstacle> _obstacles;

        private void Awake()
        {
            _obstacles = new List<Obstacle>();
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

            var gridWidth = _columns * _objectWidth + (_columns - 1) * _cellSpacingX;
            var gridDepth = _rows * _objectDepth + (_rows - 1) * _cellSpacingZ;

            var gridStart = _centerPosition - new Vector3(gridWidth / 2f, 0f, gridDepth / 2f);

            CreateGridCells(gridStart);

            var shouldCreateObstacles = _isCreateObstacles && UserUtils.GetIntRandomNumber(0, 2) == 1;

            if (shouldCreateObstacles)
            {
                CreateAllObstacles();
                CreateStretchedObstaclesBetweenNeighbors();

                if (_walls)
                {
                    _walls.position = _centerPosition;
                    _walls.gameObject.SetActive(true);
                }
            }

            if (_gridStorage.GridCount == 0) return false;

            _gridStorage.CreateCells(_rows, _columns);

            return true;
        }

        public void Terminate()
        {
            foreach (var cell in _gridStorage.GetAllCells.Where(cell => cell && cell.gameObject))
                Destroy(cell.gameObject);

            foreach (var obstacle in _obstacles.Where(obstacle => obstacle && obstacle.gameObject))
                Destroy(obstacle.gameObject);

            if (_walls) _walls.gameObject.SetActive(false);

            _obstacles.Clear();
            _gridStorage.Clear();
        }

        private void CreateGridCells(Vector3 gridStart)
        {
            _gridStorage.Clear();

            for (var row = 0; row < _rows; row++)
            for (var col = 0; col < _columns; col++)
            {
                var x = gridStart.x + col * (_objectWidth + _cellSpacingX) + _objectWidth / 2f;
                var z = gridStart.z + row * (_objectDepth + _cellSpacingZ) + _objectDepth / 2f;
                var y = _centerPosition.y + _cellHeightOffset;

                Vector3 position = new(x, y, z);

                _cellPositions[row, col] = position;

                var cell = Instantiate(_cellPrefab, transform);
                cell.transform.SetPositionAndRotation(position, Quaternion.identity);

                _cellGrid[row, col] = cell;
                _gridStorage.Add(cell);
            }
        }

        private void CreateAllObstacles()
        {
            foreach (var obs in _obstacles.Where(obs => obs)) Destroy(obs.gameObject);

            _obstacles.Clear();

            for (var row = 0; row < _rows; row++)
            for (var col = 0; col < _columns; col++)
            {
                if (row == _rows - 1) continue;

                if (_obstacleMap[row, col])
                    CreateSingleObstacle(row, col);
            }
        }

        private void CreateSingleObstacle(int row, int col)
        {
            var cell = _cellGrid[row, col];

            if (!cell) return;

            var parent = _obstaclesContainer ? _obstaclesContainer : transform;
            var obstacle = Instantiate(_obstaclePrefab, parent);

            obstacle.transform.position = cell.transform.position;
            obstacle.transform.position += Vector3.up * obstacle.transform.localScale.y;

            _obstacles.Add(obstacle);
            cell.InitObstacle(obstacle);
        }

        private void CreateStretchedObstaclesBetweenNeighbors()
        {
            if (!_stretchedObstaclePrefab) return;

            var parent = _obstaclesContainer ? _obstaclesContainer : transform;

            for (var row = 0; row < _rows; row++)
            for (var column = 0; column < _columns; column++)
            {
                if (!_obstacleMap[row, column]) continue;

                if (column < _columns - 1 && _obstacleMap[row, column + 1])
                    CreateHorizontalStretchedObstacle(_cellPositions[row, column],
                        _cellPositions[row, column + 1], parent);

                if (row < _rows - 2 && _obstacleMap[row + 1, column])
                    CreateVerticalStretchedObstacle(_cellPositions[row, column],
                        _cellPositions[row + 1, column], parent);

                if (column == 0)
                    CreateHorizontalStretchedObstacle(_cellPositions[row, column],
                        _cellPositions[row, column] + Vector3.left * (_objectWidth + _cellSpacingX), parent);
                else if (column == _columns - 1)
                    CreateHorizontalStretchedObstacle(_cellPositions[row, column],
                        _cellPositions[row, column] + Vector3.right * (_objectWidth + _cellSpacingX), parent);

                if (row == 0)
                    CreateVerticalStretchedObstacle(_cellPositions[row, column],
                        _cellPositions[row, column] + Vector3.back * (_objectDepth + _cellSpacingZ), parent);
            }
        }

        private void CreateHorizontalStretchedObstacle(Vector3 startPos, Vector3 endPos, Transform parent)
        {
            var centerPosition = (startPos + endPos) / 2f;
            centerPosition.y = startPos.y + _stretchedObstaclePrefab.transform.localScale.y;

            var obstacle = Instantiate(_stretchedObstaclePrefab, parent);

            var distance = Vector3.Distance(startPos, endPos);
            var scale = obstacle.transform.localScale;
            scale.x = distance;
            obstacle.transform.localScale = scale;

            obstacle.transform.position = centerPosition;

            _obstacles.Add(obstacle);
        }

        private void CreateVerticalStretchedObstacle(Vector3 startPos, Vector3 endPos, Transform parent)
        {
            var centerPosition = (startPos + endPos) / 2f;
            centerPosition.y = startPos.y + _stretchedObstaclePrefab.transform.localScale.y;

            var obstacle = Instantiate(_stretchedObstaclePrefab, parent);

            var distance = Vector3.Distance(startPos, endPos);
            var scale = obstacle.transform.localScale;
            scale.x = distance;
            obstacle.transform.localScale = scale;

            obstacle.transform.SetPositionAndRotation(centerPosition, Quaternion.Euler(0f, 90f, 0f));

            _obstacles.Add(obstacle);
        }

        private bool[,] GenerateComplexObstacleMap()
        {
            var map = new bool[_rows, _columns];
            var total = 0;
            var groups = Random.Range(1, Mathf.Min(4, _rows / 2 + 1));

            for (var g = 0; g < groups; g++)
            {
                if (total >= _maxObstacles) break;

                var row = Random.Range(1, _rows - 1);
                var startCol = Random.Range(0, _columns / 2 - 1);
                var length = Random.Range(1, Mathf.Min(_maxObstacleLength + 1, _columns / 2 - startCol + 1));

                for (var i = 0; i < length; i++)
                {
                    if (startCol + i >= _columns / 2 || total >= _maxObstacles) continue;

                    map[row, startCol + i] = true;
                    map[row, _columns - 1 - (startCol + i)] = true;
                    total += 2;
                }
            }

            return map;
        }
    }
}