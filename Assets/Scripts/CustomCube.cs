using UnityEngine;

public class CustomCube 
{
    public CustomCube(int count, Material material)
    {
        Count = count;
        Material = material;
    }

    public int Count { get; private set; }
    public Material Material { get; private set; }
}
