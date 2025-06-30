using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private SnakeHead _snakePrefab;
    [SerializeField] private PathHolder _pathHolder;
    [SerializeField] private CubeStorage _cubeStorage;

    private void OnEnable()
    {
        _pathHolder.Initialized += SpawnSnake;
    }

    private void OnDisable()
    {
        _pathHolder.Initialized -= SpawnSnake;
    }

    private void SpawnSnake()
    {
        GameObject snake = new("snake");

        if (_pathHolder.TryGetStartPosition(out Vector3 spawnPoint))
        {
            var snakeHead = Instantiate(_snakePrefab, spawnPoint, Quaternion.identity, snake.transform);
            snakeHead.Init(_pathHolder, snake.transform, _cubeStorage);
        }
    }
}
