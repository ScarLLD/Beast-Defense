using System;
using System.Collections;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    private float _speed;
    private float _arrivalThreshold = 0.1f;
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

        double targetY = Math.Round(escapePlace.y + transform.localScale.y / 2);
        _escapePlace = new Vector3(escapePlace.x, (float)targetY, escapePlace.z);
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

        double targetY = Math.Round(target.y + transform.localScale.y / 2);
        target = new Vector3(target.x, (float)targetY, target.z);

        while (isWork)
        {
            Vector3 direction = target - transform.position;
            transform.Translate(_speed * Time.deltaTime * direction.normalized);

            if (Vector3.Distance(target, transform.position) < _arrivalThreshold)
            {
                transform.position = target;

                if (target != _escapePlace)
                {
                    Arrived?.Invoke();
                }
                else
                {
                    StopMoving();
                    gameObject.SetActive(false);
                    Debug.Log("It Escaped");
                }
            }

            yield return null;
        }
    }
}
