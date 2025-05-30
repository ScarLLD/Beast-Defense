using System;
using System.Collections;
using UnityEngine;

public class TargetRadar : MonoBehaviour
{
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float maxDistance = 10f;

    private bool _isWork;
    private Ray _ray;
    private RaycastHit[] _hits;
    private Coroutine _moveCoroutine;

    public event Action<Transform> OnTargetFound;

    public void StartScanning()
    {
        _isWork = true;
        _moveCoroutine = StartCoroutine(ScanRoutine());
    }

    public void EndScan()
    {
        _isWork = false;
        _moveCoroutine = null;
    }

    private IEnumerator ScanRoutine()
    {
        while (_isWork)
        {
            _ray = new Ray(transform.position, transform.forward);

            _hits = Physics.SphereCastAll(_ray.origin, sphereRadius, _ray.direction, maxDistance);

            if (_hits.Length > 0)
            {
                Interact();
            }

            yield return new WaitForSeconds(0);
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
                    OnTargetFound?.Invoke(hit.transform);
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