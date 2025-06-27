using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeCreator : MonoBehaviour
{
    [SerializeField]
    private List<Material> _ñolors = new();

    [SerializeField]
    private List<int> _counts = new();

    [SerializeField] private PathHolder _pathHolder;
    [SerializeField] private GridCreator _gridCreator;
    [SerializeField] private CubeSpawner _cubeSpawner;

    private void OnEnable()
    {
        _pathHolder.PathInit += CreateCubes;
    }

    private void OnDisable()
    {
        _pathHolder.PathInit -= CreateCubes;
    }

    public void CreateCubes()
    {
        if (_gridCreator.GridCount > 0)
        {
            for (int i = 0; i < _gridCreator.GridCount; i++)
            {
                int count = _counts[Random.Range(0, _counts.Count)];
                Material material = _ñolors[Random.Range(0, _ñolors.Count)];

                _cubeSpawner.Spawn()
            }

            //SpawnCubes();


            //Debug.Log($"Êóáèêè ñîçäàíû: {_cubes.Count}");
        }
    }


}
