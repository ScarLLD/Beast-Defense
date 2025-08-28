using System.Collections.Generic;
using UnityEngine;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Beast _beastPrefab;

    public bool TrySpawn(List<Vector3> road, Snake snake, out Beast beast)
    {
        beast = null;

        road = UserUtils.GetRaisedRoad(road, _beastPrefab.transform.localScale.y / 2);

        beast = Instantiate(_beastPrefab, transform);
        beast.Init(road, snake, _game);

        return beast != null;
    }
}
