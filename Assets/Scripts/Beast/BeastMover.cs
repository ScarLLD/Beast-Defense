using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Beast))]
public class BeastMover : MonoBehaviour
{
    private readonly int _escapeTriggerDistance = 1;
    private readonly float _speedMultiplier = 2f;
    private readonly float _arrivalThreshold = 0.01f;
    private Queue<Vector3> _roadTargets;
    private Coroutine _coroutine;
    private SnakeHead _snakeHead;
    private BeastRotator _beastRotator;
    private Beast _beast;

    public Vector3 TargetPoint { get; private set; }
    public bool IsMoving { get; private set; } = false;

    private void Awake()
    {
        _beast = GetComponent<Beast>();
        _beastRotator = GetComponent<BeastRotator>();
    }

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    public void SetRoadTarget(List<Vector3> road)
    {
        _roadTargets = new Queue<Vector3>();
        _roadTargets.Enqueue(road[(int)(road.Count * 0.75f)]);
        _roadTargets.Enqueue(road[^1]);

        var spawnPoint = road[road.Count / 2];

        if (spawnPoint != null)
        {
            TargetPoint = spawnPoint;
            transform.position = TargetPoint;
        }
    }

    public void StartMoveRoutine()
    {
        _coroutine ??= StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        bool isWork = true;

        Vector3 globalTargetPosition = Vector3.zero;

        while (isWork)
        {
            if ((IsMoving == true || CheckSnakeProximity()) && TargetPoint != Vector3.zero)
            {
                if (globalTargetPosition == Vector3.zero)
                {
                    if (_roadTargets.Count == 0)
                    {
                        isWork = false;
                        StopMoveRoutine();
                    }
                    else
                    {
                        globalTargetPosition = _roadTargets.Dequeue();
                        IsMoving = true;
                    }
                }

                if ((TargetPoint - transform.localPosition).magnitude < _arrivalThreshold)
                {
                    transform.localPosition = TargetPoint;

                    if (TargetPoint == globalTargetPosition)
                    {
                        IsMoving = false;
                        globalTargetPosition = Vector3.zero;
                    }
                    else if (_beast.TryGetNextRoadPosition(out Vector3 nextPosition))
                    {
                        TargetPoint = nextPosition;
                    }
                }

                transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPoint, _snakeHead.Speed * _speedMultiplier * Time.deltaTime);
            }

            yield return null;
        }
    }

    private bool CheckSnakeProximity()
    {
        if (_snakeHead.TryGetRoadIndex(out int snakeIndex) && _beast.TryGetRoadIndex(TargetPoint, out int beastIndex))
            return beastIndex - snakeIndex < _escapeTriggerDistance;

        return false;
    }

    private void StopMoveRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _beastRotator.StopRotateRoutine();
    }
}
