using UnityEngine;

[RequireComponent(typeof(TargetStorage))]
public class TargetDetector : MonoBehaviour
{
    private TargetStorage _targetsStorage;
    private Collider _collider;

    private void Awake()
    {
        _targetsStorage = GetComponent<TargetStorage>();
        _collider = GetComponent<Collider>();
        _collider.isTrigger = false;
    }

    public void EnableTrigger()
    {
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SnakeSegment segment))
        {
            _targetsStorage.AddTarget(segment);
        }
    }
}
