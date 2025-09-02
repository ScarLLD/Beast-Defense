using UnityEngine;

public class Cube : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private SnakeSegment _snakeSegment;
    private bool _isDestroyed = false;

    public Material Material => _meshRenderer.material;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Init(Material material)
    {
        _meshRenderer.material = material;
        _isDestroyed = false;
        gameObject.SetActive(false);
    }

    public void InitSegment(SnakeSegment snakeSegment)
    {
        _snakeSegment = snakeSegment;
    }
    
    public void Hit()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;
        gameObject.SetActive(false);
        _snakeSegment.TryDestroy();
    }

    public void Deactivate()
    {
        if (!_isDestroyed)
            gameObject.SetActive(false);
    }

    public void Activate()
    {
        if (!_isDestroyed)
            gameObject.SetActive(true);
    }
}
