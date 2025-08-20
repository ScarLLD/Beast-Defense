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
    private Beast _beast;

    public Vector3 LocalTargetPoint { get; private set; }

    private void Awake()
    {
        _beast = GetComponent<Beast>();
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
        bool isMoving = false;

        Vector3 globalTargetPosition = Vector3.zero;

        while (isWork && LocalTargetPoint != Vector3.zero)
        {
            if (isMoving == true || CheckSnakeProximity())
            {
                Debug.Log("Start move!");

                if (globalTargetPosition == Vector3.zero)
                {
                    if (_roadTargets.Count == 0)
                    {
                        isWork = false;
                        EndMove();
                    }
                    else
                    {
                        globalTargetPosition = _roadTargets.Dequeue();
                        isMoving = true;
                    }
                }

                if ((LocalTargetPoint - transform.localPosition).magnitude < _arrivalThreshold)
                {
                    transform.localPosition = LocalTargetPoint;

                    if (LocalTargetPoint == globalTargetPosition)
                    {
                        isMoving = false;
                        globalTargetPosition = Vector3.zero;
                        Debug.Log("Stop move!");
                    }
                    else if (_beast.TryGetNextRoadPosition(out Vector3 nextPosition))
                    {
                        LocalTargetPoint = nextPosition;
                        Debug.Log("Next move!");
                    }
                }


                transform.localPosition = Vector3.MoveTowards(transform.localPosition, LocalTargetPoint, _snakeHead.Speed * _speedMultiplier * Time.deltaTime);
            }

            yield return null;
        }
    }

    private bool CheckSnakeProximity()
    {
        if (_snakeHead.TryGetRoadIndex(out int snakeIndex) && _beast.TryGetRoadIndex(LocalTargetPoint, out int beastIndex))
            return beastIndex - snakeIndex < _escapeTriggerDistance;

        return false;
    }



    private void EndMove()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        Debug.Log("Move ENDED");
    }
}
