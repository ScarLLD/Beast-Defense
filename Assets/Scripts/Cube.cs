using UnityEngine;

public class Cube : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Init(Material material)
    {
        _meshRenderer.material = material;
    }

    public Material Material => _meshRenderer.material;
}
