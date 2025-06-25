using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    [SerializeField] private TargetsHolder _targetsHolder;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SnakeSegment segment))
        {
            _targetsHolder.AddTarget(segment);
        }
    }
}
