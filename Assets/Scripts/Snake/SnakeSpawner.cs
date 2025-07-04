using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private SnakeHead _snakePrefab;
    [SerializeField] private RoadSpawner _RoadSpawner;
    [SerializeField] private CubeStorage _cubeStorage;

    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + _snakePrefab.transform.localScale.y / 2, transform.position.z);
    }

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
            var snakeHead = Instantiate(_snakePrefab, road.First(), Quaternion.identity, transform);
            snakeHead.Init(_cubeStorage, road);
            snakeHead.CreateTail();
        }
    }
}
