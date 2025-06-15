using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private SnakeHead _snakePrefab;
    [SerializeField] private PathHolder _pathHolder;

    private void OnEnable()
    {
        _pathHolder.PathInit += SpawnSnake;
    }

    private void OnDisable()
    {
        _pathHolder.PathInit -= SpawnSnake;
    }

    private void SpawnSnake()
    {
        GameObject snake = new GameObject("snake");

        if (_pathHolder.TryGetStartPosition(out Vector3 spawnPoint))
        {
            var snakeHead = Instantiate(_snakePrefab, spawnPoint, Quaternion.identity, snake.transform);
            snakeHead.Init(_pathHolder, snake.transform);
        }
    }
}
