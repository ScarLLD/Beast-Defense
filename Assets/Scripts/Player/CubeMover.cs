using System;
using System.Collections;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    private float _speed;
    private float _maxDistanceRounding;
    private Coroutine _moveCoroutine;

    public event Action Arrived;

    public void Init(float speed, float maxDistance)
    {
        _speed = speed;
        _maxDistanceRounding = maxDistance;
    }

    public void StartMoving(Transform targetTransform)
    {
        _moveCoroutine = StartCoroutine(MoveRoutine(targetTransform));
    }

    private IEnumerator MoveRoutine(Transform target)
    {
        bool isWork = true;

        while (isWork)
        {
            Vector3 direction = target.position - transform.position;
            transform.Translate(_speed * Time.deltaTime * direction.normalized);

            if ((target.position - transform.position).magnitude < _maxDistanceRounding)
            {
                transform.position = target.position;
                StopMoving();
                Arrived?.Invoke();
                isWork = false;
            }

            yield return null;
        }
    }

    private void StopMoving()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }
}
