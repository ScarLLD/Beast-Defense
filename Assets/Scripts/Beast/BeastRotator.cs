using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BeastMover))]
public class BeastRotator : MonoBehaviour
{
    private readonly float _rotationSpeed = 5f;
    private Coroutine _coroutine;
    private BeastMover _beastMover;
    private Vector3 _lookDirection;
    private Vector3? _finalRotation;

    private void Awake()
    {
        _beastMover = GetComponent<BeastMover>();
    }

    private void OnDisable()
    {
        StopRotateRoutine();
    }

    public void StartRotateRoutine()
    {
        _coroutine ??= StartCoroutine(RotateToTarget());
    }

    public void SetLookDirection(Vector3 direction)
    {
        _lookDirection = direction;
        _finalRotation = null; // —брасываем финальный поворот при установке нового направлени€
    }

    public void SetFinalRotation(Vector3 direction)
    {
        _finalRotation = direction;
    }

    private IEnumerator RotateToTarget()
    {
        bool isWork = true;

        while (isWork == true)
        {
            Vector3 targetDirection;

            if (_finalRotation.HasValue)
            {
                // ‘инальный поворот (вниз после завершени€ движени€)
                targetDirection = _finalRotation.Value;
            }
            else if (_beastMover.TargetPoint != Vector3.zero && _beastMover.IsMoving)
            {
                // ƒвижемс€ - смотрим по направлению сплайна
                targetDirection = _lookDirection;
            }
            else
            {
                // ѕо умолчанию смотрим вперед
                targetDirection = Vector3.forward;
            }

            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
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