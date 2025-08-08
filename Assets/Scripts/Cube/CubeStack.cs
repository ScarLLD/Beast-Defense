using UnityEngine;

public class CubeStack : MonoBehaviour
{
    public int Count { get; private set; }
    public Material Material { get; private set; }

    public void Init(Material material, int count)
    {
        Count = count;
        Material = material;
    }
}
