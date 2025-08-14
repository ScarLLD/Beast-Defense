using UnityEngine;

public class Cube : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private SnakeSegment _snakeSegment;

    public Material Material => _meshRenderer.material;
    public Quaternion GetSegmentRotation => _snakeSegment.transform.rotation;

    public void Init(Material material)
    {
        _meshRenderer.material = material;
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void InitSegment(SnakeSegment snakeSegment)
    {
        _snakeSegment = snakeSegment;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);

        if (_snakeSegment != null)
            _snakeSegment.TryDestroy();
    }
}
