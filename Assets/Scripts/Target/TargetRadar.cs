using System;
using UnityEngine;

public class TargetRadar : MonoBehaviour
{
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask layerMask;

    private Ray _ray;
    private RaycastHit[] _hits;

    public event Action<Transform> Found;

    void Update()
    {
        _ray = new Ray(transform.position, transform.forward);

        _hits = Physics.SphereCastAll(_ray.origin, sphereRadius, _ray.direction, maxDistance);

        if (_hits.Length > 0)
        {
            Interact();
        }
    }

    private void Interact()
    {
        foreach (var hit in _hits)
        {
            if (hit.collider.TryGetComponent(out ITarget target) && target.IsDetected == false)
            {
                if (target as Enemy)
                {
                    target.ChangeDetectedStatus();
                    Found?.Invoke(hit.transform);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (_ray.direction == Vector3.zero)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * maxDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_ray.origin + _ray.direction * maxDistance, sphereRadius);
    }
}
