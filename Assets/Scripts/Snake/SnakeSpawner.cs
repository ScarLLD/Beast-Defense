using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private TargetStorage _targetStorage;
    [SerializeField] private Snake _snakePrefab;

    public bool TrySpawn(Game game, List<Vector3> road, List<CubeStack> stacks, SplineContainer splineContainer, out Snake snake)
    {
        snake = null;

        var tempRoad = UserUtils.GetRaisedRoad(road, _snakePrefab.transform.localScale.y / 2);

        if (tempRoad.Count > 0)
        {
            //snakeHead = Instantiate(_snakePrefab, tempRoad.First(), Quaternion.identity, transform);
            //snakeHead.Init(game, stacks, tempRoad, _targetStorage);
            //snakeHead.CreateTail();

            splineContainer.transform.position =
                new Vector3(splineContainer.transform.position.x,
                splineContainer.transform.position.y + _snakePrefab.transform.localScale.y,
                splineContainer.transform.position.z);

            snake = Instantiate(_snakePrefab, transform);
            snake.InitializeSnake(splineContainer);
        }

        return snake != null;
    }
}
