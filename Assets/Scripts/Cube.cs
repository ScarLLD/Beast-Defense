using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private void Awake()
    {
        Material = GetComponent<MeshRenderer>().material;
    }

    public void Init(Material material)
    {
        Material = material;
    }

    public Material Material { get; private set; }
}
