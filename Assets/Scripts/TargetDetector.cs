using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetStorage))]
public class TargetDetector : MonoBehaviour
{
    [SerializeField] private RoadSpawner _roadSpawner;

    private TargetStorage _targetsStorage;
    private Collider _collider;

    private void Awake()
    {
        _targetsStorage = GetComponent<TargetStorage>();
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        _roadSpawner.Spawned += SetPostion;
    }

    private void OnDisable()
    {
        _roadSpawner.Spawned -= SetPostion;
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

    private void SetPostion(List<Vector3> road)
    {
        if (road.Count > 1)
        {
            transform.position = road[1];
        }
    }
}
