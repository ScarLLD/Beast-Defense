using UnityEngine;

public class Cube : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private SnakeSegment _snakeSegment;

    public bool IsDestroyed { get; private set; } = false;

    public Material Material => _meshRenderer.material;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Init(Material material)
    {
        _meshRenderer.material = material;
        gameObject.SetActive(false);
    }

    public void InitSegment(SnakeSegment snakeSegment)
    {
        _snakeSegment = snakeSegment;
    }

    public void Hit()
    {
        if (IsDestroyed == false)
        {
            Deactivate();
            IsDestroyed = true;
            _snakeSegment.TryDestroy();
        }
    }

    public void Deactivate()
    {
        if (IsDestroyed == false)
            gameObject.SetActive(false);
    }

    public void Activate()
    {
        if (IsDestroyed == false)
            gameObject.SetActive(true);
    }
}
