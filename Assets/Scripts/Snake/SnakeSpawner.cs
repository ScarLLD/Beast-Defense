using System.Collections.Generic;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private SnakeHead _snakePrefab;
    [SerializeField] private RoadSpawner _RoadSpawner;
    [SerializeField] private CubeStorage _cubeStorage;

    private void OnEnable()
    {
        _RoadSpawner.Spawned += Spawn;
    }

    private void OnDisable()
    {
        _RoadSpawner.Spawned -= Spawn;
    }

    private void Spawn(List<Vector3> road)
    {
        if (road.Count > 0)
        {
            var snakeHead = Instantiate(_snakePrefab, road[0], Quaternion.identity, transform);
            snakeHead.Init(road, transform, _cubeStorage);
        }
    }
}
