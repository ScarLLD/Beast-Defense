using System.Collections.Generic;
using UnityEngine;

public class StaticCubesHolder : MonoBehaviour
{
    private Queue<PlayerCube> _shootingPlaces;

    private void Awake()
    {
        _shootingPlaces = new Queue<PlayerCube>();
    }

    public void PutCube(PlayerCube cube)
    {
        _shootingPlaces.Enqueue(cube);
    }
}
