using UnityEngine;

public class MGCube : MonoBehaviour
{
    [SerializeField] private Material _material;

    public void SetColor(Color color)
    {
        _material.color = color;
    }
}
