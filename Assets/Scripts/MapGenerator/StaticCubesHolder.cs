using System.Collections.Generic;
using UnityEngine;

public class StaticCubesHolder : MonoBehaviour
{
    private Queue<Cube> _shootingPlaces;

    private void Awake()
    {
        _shootingPlaces = new Queue<Cube>();
    }

    public void PutCube(Cube cube)
    {
        _shootingPlaces.Enqueue(cube);
    }
}
