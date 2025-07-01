using System;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private SnakeSegment _snakeSegment;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Init(Material material)
    {
        _meshRenderer.material = material;
    }

    public void GetSegment(SnakeSegment snakeSegment)
    {
        _snakeSegment = snakeSegment;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);

        if (_snakeSegment != null)
            _snakeSegment.TryDestroy();
    }

    public Material Material => _meshRenderer.material;
}
