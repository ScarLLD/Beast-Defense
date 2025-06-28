using UnityEngine;

public class CubeStack : MonoBehaviour
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

    public void IncreaseCount()
    {
        if (Count > 0)
        {
            Count--;
        }
    }
}
