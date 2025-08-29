using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] private List<Cube> _cubes;

    private int currentCubesCount = 0;

    public Material Material { get; private set; }
    public bool IsTarget { get; private set; } = false;

    public void Init(Material material)
    {
        Material = material;

        foreach (var cube in _cubes)
        {
            cube.Init(material);
        }
    }

    public void SetIsTarget(bool isTarget)
    {
        IsTarget = isTarget;
    }

    public bool TryGetCube(out Cube cube)
    {
        cube = null;

        //

        return cube != null;
    }

    public void AddCube(Cube cube)
    {
        Material = cube.Material;
        currentCubesCount++;

        _cubes.Add(cube);
    }

    public bool IsCurrectColor(Color color)
    {
        return Material != null && Material.color == color;
    }

    public void TryDestroy()
    {
        //
    }

    public void ActivateCubes(Material material)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Cube cube))
            {
                cube.Init(material);
                AddCube(cube);
                cube.gameObject.SetActive(true);
            }
        }
    }
}