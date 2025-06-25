using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private SnakeHead _snakePrefab;
    [SerializeField] private PathHolder _pathHolder;
    [SerializeField] private SpecificCubesCreator _specificCubesCreator;

    private void OnEnable()
    {
        _specificCubesCreator.Created += SpawnSnake;
    }

    private void OnDisable()
    {
        _specificCubesCreator.Created -= SpawnSnake;
    }

    private void SpawnSnake()
    {
        GameObject snake = new GameObject("snake");

        if (_pathHolder.TryGetStartPosition(out Vector3 spawnPoint))
        {
            var snakeHead = Instantiate(_snakePrefab, spawnPoint, Quaternion.identity, snake.transform);
            snakeHead.Init(_pathHolder, snake.transform, _specificCubesCreator);
        }
    }
}
