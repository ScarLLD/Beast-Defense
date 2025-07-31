using System;
using UnityEngine;

public class CubeStack : MonoBehaviour, ICube
{
    public void Init(Material material, int count)
    {
        Count = count;
        Material = material;
    }

    public int Count { get; private set; }
    public Material Material { get; private set; }
}
