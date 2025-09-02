using System.Collections.Generic;
using UnityEngine;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Beast _beastPrefab;

    public void Spawn(List<Vector3> road, Snake snake)
    {
        road = UserUtils.GetRaisedRoad(road, _beastPrefab.transform.localScale.y / 2);

        var beast = Instantiate(_beastPrefab, transform);
        beast.Init(road, snake);
    }
}
