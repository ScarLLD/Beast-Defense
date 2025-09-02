using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BeastMover))]
public class BeastRotator : MonoBehaviour
{
    private readonly float _rotationSpeed;
    private Coroutine _coroutine;
    private BeastMover _beastMover;
    private Vector3 _direction;
    private Snake _snake;

    private void Awake()
    {
        _beastMover = GetComponent<BeastMover>();
    }

    private void OnDisable()
    {
        StopRotateRoutine();
    }

    public void Init(Snake snake)
    {
        _snake = snake;
    }

    public void StartRotateRoutine()
    {
        _coroutine ??= StartCoroutine(RotateToTarget());
    }

    private IEnumerator RotateToTarget()
    {
        bool isWork = true;

        while (isWork == true)
        {
            if (_beastMover.TargetPoint != Vector3.zero && _beastMover.IsMoving)
                _direction = _beastMover.TargetPoint - transform.position;
            else
                _direction = Vector3.back;

            if (_direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }

            yield return null;
        }
    }

    public void StopRotateRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
