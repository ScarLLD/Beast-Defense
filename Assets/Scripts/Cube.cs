using UnityEngine;

public class Cube : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        Material = _meshRenderer.material;
    }

    public void Init(Material material)
    {        
        _meshRenderer.material = material; 
    }

    public Material Material { get; private set; }
}
