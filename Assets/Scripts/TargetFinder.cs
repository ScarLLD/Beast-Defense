using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private TargetCollector _targetCollector;

    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float maxDistance = 10f;
    public LayerMask layerMask;

    private Ray _ray;
    private RaycastHit[] _hits;

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
            if (hit.collider.TryGetComponent<ITarget>(out ITarget target) && target.IsDetected == false)
            {
                target.ChangeDetectedStatus();
                _targetCollector.PutTarget(hit.transform);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (_ray.direction == Vector3.zero)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * maxDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_ray.origin, sphereRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_ray.origin + _ray.direction * maxDistance, sphereRadius);

    }
}
