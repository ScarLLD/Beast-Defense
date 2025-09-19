using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private TargetStorage _targetStorage;
    [SerializeField] private Snake _snakePrefab;

    public void Spawn(List<CubeStack> stacks, SplineContainer splineContainer, Game game, out Snake snake)
    {
        AlignSpline(splineContainer);

        snake = Instantiate(_snakePrefab, transform);
        snake.InitializeSnake(stacks, splineContainer, game);
    }

    private void AlignSpline(SplineContainer splineContainer)
    {
        splineContainer.transform.position =
            new(splineContainer.transform.position.x,
            splineContainer.transform.position.y + _snakePrefab.transform.localScale.y,
            splineContainer.transform.position.z);
    }
}
