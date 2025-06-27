using UnityEngine;

public class CubeStack : MonoBehaviour
{
    public void Init(Cube cube, int count)
    {
        Cube = cube;
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
