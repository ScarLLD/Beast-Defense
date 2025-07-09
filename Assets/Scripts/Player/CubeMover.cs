using System;
using System.Collections;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    private float _speed;
    private float _maxDistanceRounding = 0.1f;
    private Coroutine _moveCoroutine;
    private Vector3 _escapePlace;
    private ShootingPlace _shootingPlace;

    public event Action Arrived;

    public void Init(float speed)
    {
        _speed = speed;
    }

    public void StartMoving(Vector3 target)
    {
        _moveCoroutine = StartCoroutine(MoveRoutine(target));
    }

    public void SetPlaces(ShootingPlace shootingPlace, Vector3 escapePlace)
    {
        _shootingPlace = shootingPlace;
        _escapePlace = escapePlace;
    }

    public void GoEscape()
    {
        _shootingPlace.ChangeEmptyStatus();
        _moveCoroutine = StartCoroutine(MoveRoutine(_escapePlace));
    }

    public void StopMoving()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        bool isWork = true;

        target = new Vector3(target.x, target.y + transform.localScale.y / 2, target.z);

        while (isWork)
        {
            Vector3 direction = target - transform.position;
            transform.Translate(_speed * Time.deltaTime * direction.normalized);

            if ((target - transform.position).magnitude < _maxDistanceRounding)
            {
                transform.position = target;

                if (target != _escapePlace)
                    Arrived?.Invoke();
                else
                    StopMoving();
            }

            yield return null;
        }
    }
}
