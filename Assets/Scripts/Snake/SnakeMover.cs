using System;
using System.Collections;
using UnityEngine;

[RequireComponent((typeof(SnakeLocalSettings)))]
public class SnakeMover : MonoBehaviour
{
    private float _maxDistanceRounding = 0.01f;
    private Coroutine _coroutine;
    private SnakeHead _snakeHead;
    private SnakeLocalSettings _localSettings;

    public event Action Arrived;

    private void Awake()
    {
        _localSettings = GetComponent<SnakeLocalSettings>();
    }

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    public void StartMoveRoutine()
    {
        _coroutine = StartCoroutine(MoveToTarget());
    }

    public void StopMoveRoutine()
    {
        StopCoroutine(_coroutine);
        _coroutine = null;
    }

    private IEnumerator MoveToTarget()
    {
        bool isWork = true;

        while (isWork && _localSettings.TargetPosition != null)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _localSettings.TargetPosition, _snakeHead.Speed * Time.deltaTime);

            if ((_localSettings.TargetPosition - transform.localPosition).magnitude < _maxDistanceRounding)
            {
                transform.localPosition = _localSettings.TargetPosition;
                Arrived?.Invoke();
            }

            yield return null;
        }
    }
}
