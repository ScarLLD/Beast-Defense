using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCube : MonoBehaviour
{
    public int Count { get; private set; }
    public Color Color { get; private set; }

    public CustomCube(int count, Color color)
    {
        Count = count;
        Color = color;
    }
}
