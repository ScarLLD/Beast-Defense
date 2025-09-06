using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RoadFinder))]
public class CubeMover : MonoBehaviour
{
    private readonly float _arrivalThreshold = 0.1f;
    private float _speed;
    private bool _isNewMove = true;
    private Vector3 _initialPosition;
    private Vector3 _escapePlace;
    private Vector3 _target;
    private ShootingPlace _shootingPlace;
    private Coroutine _moveCoroutine;
    private RoadFinder _roadfinder;
    private GridCell _cell;

    private Vector3 _cachedCellTarget;
    private Vector3 _cachedShootingTarget;

    public event Action Arrived;

    private void Awake()
    {
        _roadfinder = GetComponent<RoadFinder>();
        _initialPosition = transform.position;
    }

    public void Init(float speed)
    {
        _speed = speed;
    }

    public void StartMoving()
    {
        if (_isNewMove)
        {
            _isNewMove = false;
            _target = _cachedCellTarget;
        }

        _moveCoroutine ??= StartCoroutine(MoveRoutine());
    }

    public void SetPlaces(ShootingPlace shootingPlace, Vector3 escapePlace, GridCell cell)
    {
        _shootingPlace = shootingPlace;
        _cell = cell;

        _cachedCellTarget = GetCurrentTarget(cell.transform.position);
        _cachedShootingTarget = GetCurrentTarget(shootingPlace.transform.position);
        _escapePlace = GetCurrentTarget(escapePlace);
    }

    public void GoEscape()
    {
        _shootingPlace.ChangeEmptyStatus(true);
        _target = _escapePlace;
        _moveCoroutine = StartCoroutine(MoveRoutine());
    }

    public void StopMoving()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    private IEnumerator MoveRoutine()
    {
        bool isWork = true;

        while (isWork)
        {
            Vector3 direction = _target - transform.position;
            transform.position += _speed * Time.deltaTime * direction.normalized;
            transform.LookAt(_target);

            if (Vector3.Distance(_target, transform.position) < _arrivalThreshold)
            {
                transform.position = _target;

                if (_target == _cachedShootingTarget)
                    Arrived?.Invoke();

                SelectTarget();
            }

            yield return null;
        }
    }

    private void SelectTarget()
    {
        if (_target == _cachedCellTarget)
        {
            var nextCell = _roadfinder.GetOptimalNextCell(_cell);

            if (nextCell != null)
            {
                _cell = nextCell;
                _cachedCellTarget = GetCurrentTarget(_cell.transform.position);
                _target = _cachedCellTarget;
            }
            else
            {
                _target = _cachedShootingTarget;
            }
        }
        else if (_target == _escapePlace)
        {
            StopMoving();
            gameObject.SetActive(false);
        }
    }

    private Vector3 GetCurrentTarget(Vector3 targetPosition)
    {
        return new(targetPosition.x, _initialPosition.y, targetPosition.z);
    }
}