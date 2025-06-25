using System;
using System.Collections;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    private float _speed;
    private float _maxDistanceRounding = 0.01f;
    private Coroutine _moveCoroutine;

    public event Action Arrived;

    public void Init(float speed)
    {
        _speed = speed;
    }

    public void StartMoving(Vector3 target)
    {
        _moveCoroutine = StartCoroutine(MoveRoutine(target));
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        bool isWork = true;

        target = new Vector3(target.x, transform.position.y, target.z);

        while (isWork)
        {
            Vector3 direction = target - transform.position;
            transform.Translate(_speed * Time.deltaTime * direction.normalized);

            if ((target - transform.position).magnitude < _maxDistanceRounding)
            {
                transform.position = target;
                Arrived?.Invoke();
                isWork = false;
                StopMoving();
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
