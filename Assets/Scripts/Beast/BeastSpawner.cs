using System.Collections.Generic;
using UnityEngine;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Beast _beastPrefab;

    public Beast Spawn(List<Vector3> road, SnakeHead snakeHead)
    {
        road = UserUtils.GetRaisedRoad(road, _beastPrefab.transform.localScale.y / 2);

        var beast = Instantiate(_beastPrefab, transform);
        beast.Init(road, snakeHead, _game);

        return beast;
    }
}
