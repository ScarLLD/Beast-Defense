using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private TargetStorage _targetStorage;
    [SerializeField] private SnakeHead _snakePrefab;
    [SerializeField] private CubeStorage _cubeStorage;

    public bool TrySpawn(List<Vector3> road, out SnakeHead snakeHead)
    {
        snakeHead = null;

        var tempRoad = UserUtils.GetRaisedRoad(road, _snakePrefab.transform.localScale.y / 2);

        if (tempRoad.Count > 0)
        {
            snakeHead = Instantiate(_snakePrefab, tempRoad.First(), Quaternion.identity, transform);
            snakeHead.Init(_cubeStorage, tempRoad, _targetStorage);
            snakeHead.CreateTail();
        }

        return snakeHead != null;
    }
}
