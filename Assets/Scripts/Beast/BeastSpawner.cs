using System.Collections.Generic;
using UnityEngine;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Beast _beastPrefab;

    public void Spawn(List<Vector3> road, SnakeHead snakeHead)
    {
        road = UserUtils.GetRaisedRoad(road, _beastPrefab.transform.localScale.y);

        var beast = Instantiate(_beastPrefab);
        beast.Init(snakeHead, road);
    }
}
