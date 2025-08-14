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

        road = UserUtils.GetRaisedRoad(road, _snakePrefab.transform.localScale.y / 2);

        if (road.Count > 0)
        {
            snakeHead = Instantiate(_snakePrefab, road.First(), Quaternion.identity, transform);
            snakeHead.Init(_cubeStorage, road, _targetStorage);
            snakeHead.CreateTail();
        }

        return snakeHead != null;
    }
}
