using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private SnakeHead _snakePrefab;
    [SerializeField] private PathHolder _pathHolder;
    [SerializeField] private CubeCreator _CustomCubesCreator;

    private void OnEnable()
    {
        //_CustomCubesCreator.Created += SpawnSnake;
    }

    private void OnDisable()
    {
        //_CustomCubesCreator.Created -= SpawnSnake;
    }

    private void SpawnSnake()
    {
        GameObject snake = new GameObject("snake");

        if (_pathHolder.TryGetStartPosition(out Vector3 spawnPoint))
        {
            var snakeHead = Instantiate(_snakePrefab, spawnPoint, Quaternion.identity, snake.transform);
            snakeHead.Init(_pathHolder, snake.transform, _CustomCubesCreator);
        }
    }
}
