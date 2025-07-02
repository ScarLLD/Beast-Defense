using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeMover))]
public class SnakeRotator : MonoBehaviour
{
    private float _speedMultiplier = 0.6f;
    private Coroutine _coroutine;
    private SnakeHead _snakeHead;
    private SnakeMover _snakeMover;
    private Vector3 _direction;

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    private void Start()
    {
        StartRotateRoutine();
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
        if (_snakeMover.TargetPosition != null)
        {
            Vector3 direction = _snakeMover.TargetPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private IEnumerator RotateToTarget()
    {
        while (_snakeMover.TargetPosition != null)
        {
            _direction = _snakeMover.TargetPosition - transform.position;

            if (_snakeMover.IsForwardMoving == false)
                _direction *= -1;

            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _snakeHead.Speed * _speedMultiplier);
        }

        yield return null;
    }
}
