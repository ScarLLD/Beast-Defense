using UnityEngine;

[RequireComponent(typeof(TargetStorage))]
public class TargetDetector : MonoBehaviour
{
    private TargetStorage _targetsHolder;
    private Collider _collider;

    private void Awake()
    {
        _targetsHolder = GetComponent<TargetStorage>();
        _collider = GetComponent<Collider>();
    }

    public void EnableTrigger()
    {
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SnakeSegment segment))
        {
            _targetsHolder.AddTarget(segment);
        }
    }
}
