using UnityEngine;

public class CustomCube
{
    public int Count { get; private set; }
    public Color Color { get; private set; }

    public CustomCube(int count, Color color)
    {
        Count = count;
        Color = color;
    }
}
