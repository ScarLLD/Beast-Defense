using System.Collections.Generic;
using UnityEngine;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Beast _beastPrefab;

    public void Spawn(List<Vector3> road, Snake snake, out Beast beast)
    {
        road = UserUtils.GetRaisedRoad(road, _beastPrefab.transform.localScale.y / 2);

        beast = Instantiate(_beastPrefab, transform);
        beast.Init(road, snake, _game);
    }
}
