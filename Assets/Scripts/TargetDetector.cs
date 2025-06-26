using UnityEngine;

[RequireComponent(typeof(TargetsHolder))]
public class TargetDetector : MonoBehaviour
{
    private TargetsHolder _targetsHolder;
    private Collider _collider;

    private void Awake()
    {
        _targetsHolder = GetComponent<TargetsHolder>();
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
