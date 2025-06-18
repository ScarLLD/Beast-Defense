using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeLocalSettings))]
public class SnakeRotator : MonoBehaviour
{
    private SnakeLocalSettings _localSettings;
    private Coroutine _coroutine;
    private SnakeHead _snakeHead;

    private void Awake()
    {
        _localSettings = GetComponent<SnakeLocalSettings>();
    }

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    public void SetStartRotation()
    {
        if (_localSettings.TargetPosition != null)
        {
            Vector3 direction = _localSettings.TargetPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void StartRotateRoutine()
    {
        _coroutine = StartCoroutine(Rotate());
    }

    public void StopRotateRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator Rotate()
    {
        bool isWork = true;

        while (isWork)
        {
            Vector3 direction = _localSettings.TargetPosition - transform.position;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _snakeHead.Speed * 0.6f);

                if (transform.rotation == targetRotation)
                    StopRotateRoutine();
            }

            yield return null;
        }
    }
}
