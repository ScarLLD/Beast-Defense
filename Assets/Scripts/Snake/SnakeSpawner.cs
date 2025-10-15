using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private TargetStorage _targetStorage;
    [SerializeField] private Snake _snakePrefab;

    private Snake _snake;

    public Snake Spawn(List<CubeStack> stacks, SplineContainer splineContainer, DeathModule deathModule, Beast beast)
    {
        if (_snake == null)
            _snake = Instantiate(_snakePrefab, transform);

        _snake.InitializeSnake(stacks, splineContainer, deathModule, beast);

        return _snake;
    }
}
