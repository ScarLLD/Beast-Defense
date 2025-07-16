using System;
using UnityEngine;

public class CubeStack : MonoBehaviour, ICube
{
    [SerializeField] private Cube _cube;

    public void Init(Material material, int count)
    {
        Cube = _cube;
        Cube.Init(material);
        Count = count;
    }

    public Cube Cube { get; private set; }
    public int Count { get; private set; }

    public Material Material => Cube.Material;

    public bool IsStatic { get; private set; }

    public bool IsAvailable { get; private set; }

    public void ChangeStaticStatus(bool isStatic)
    {
        IsStatic = isStatic;
    }
}
