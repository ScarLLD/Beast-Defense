using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Beast))]
public class BeastMover : MonoBehaviour
{
    private readonly float _arrivalThreshold = 0.01f;
    private Queue<Vector3> _roadTargets;
    private Coroutine _coroutine;
    private Beast _beast;

    public Vector3 LocalTargetPoint { get; private set; }

    private void Awake()
    {
        _beast = GetComponent<Beast>();
    }

    public void SetRoadTarget(List<Vector3> road)
    {
        _roadTargets = new Queue<Vector3>();
        _roadTargets.Enqueue(road[(int)(road.Count * 0.75f)]);
        _roadTargets.Enqueue(road[road.Count]);

        var spawnPoint = road[road.Count / 2];

        if (spawnPoint != null)
        {
            LocalTargetPoint = spawnPoint;
            transform.position = LocalTargetPoint;
        }
    }

    public void StartMove()
    {
        _coroutine ??= StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        bool isWork = true;

        Vector3 globalTargetPosition = _roadTargets.Dequeue();

        if (_beast.TryGetNextRoadPosition(out Vector3 nextPosition))
            LocalTargetPoint = nextPosition;

        while (isWork && LocalTargetPoint != Vector3.zero)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, LocalTargetPoint, _beast.Speed * Time.deltaTime);

            if ((LocalTargetPoint - transform.localPosition).magnitude < _arrivalThreshold)
            {
                transform.localPosition = LocalTargetPoint;

                if (LocalTargetPoint == globalTargetPosition)
                {
                    isWork = false;
                    EndMove();
                }
                else if (_beast.TryGetNextRoadPosition(out nextPosition))
                {
                    LocalTargetPoint = nextPosition;
                }
            }


            yield return null;
        }
    }

    private void EndMove()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
