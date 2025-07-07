using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeSegment))]
public class SnakeRotator : MonoBehaviour
{
    private readonly float _speedMultiplier = 1.5f;
    private Coroutine _coroutine;
    private SnakeHead _snakeHead;
    private SnakeMover _snakeMover;
    private Vector3 _direction;

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    private void Awake()
    {
        _snakeMover = GetComponent<SnakeMover>();
    }

    public void StartRotateRoutine()
    {
        _coroutine = StartCoroutine(RotateToTarget());
    }

    public void StopRotateRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void SetStartRotation()
    {
        if (_snakeHead != null && _snakeHead.TryGetFirstRoadPoint(out Vector3 firstPoint)
            && _snakeHead.TryGetNextRoadPoint(firstPoint, out Vector3 secondPoint))
        {
            Vector3 direction = secondPoint - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private IEnumerator RotateToTarget()
    {
        bool isWork = true;

        while (isWork == true)
        {
            if (_snakeMover.TargetPoint != Vector3.zero)
            {
                _direction = _snakeMover.TargetPoint - transform.position;

                if (_snakeMover.IsForwardMoving == false)
                    _direction *= -1;

                if (_direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(_direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _snakeHead.Speed * _speedMultiplier);
                }

            }

            yield return null;
        }
    }
}
