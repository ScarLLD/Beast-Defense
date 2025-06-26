using System.Collections.Generic;
using UnityEngine;

public class ColorsHolder : MonoBehaviour
{
    [SerializeField]
    private List<Material> availableColors = new List<Material>();

    public Material GetRandomMaterial() => availableColors[Random.Range(0, availableColors.Count)];
}
