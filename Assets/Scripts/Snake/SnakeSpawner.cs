using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private SnakeHead _snakePrefab;
    [SerializeField] private CubeStorage _cubeStorage;

    public void Spawn(List<Vector3> road)
    {
        road = GetRaisedRoad(road);

        if (road.Count > 0)
        {
            var snakeHead = Instantiate(_snakePrefab, road.First(), Quaternion.identity, transform);
            snakeHead.Init(_cubeStorage, road);
            snakeHead.CreateTail();
        }
    }

    private List<Vector3> GetRaisedRoad(List<Vector3> road)
    {
        List<Vector3> RaisedRoad = new();

        for (int i = 0; i < road.Count; i++)
        {
            RaisedRoad.Add(new Vector3(road[i].x, road[i].y + _snakePrefab.transform.localScale.y / 2, road[i].z));
        }

        return RaisedRoad;
    }
}
