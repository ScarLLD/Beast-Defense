using System;
using System.Collections;
using Game.Scripts.CubeCore;
using Game.Scripts.MapGenerator;
using Game.Scripts.MapGenerator.Grid;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class CubeMover : MonoBehaviour
    {
        private const float ARRIVAL_THRESHOLD = 0.15f;
        
        private Vector3 _cachedCellTarget;
        private Vector3 _cachedShootingTarget;
        private Vector3 _escapePlace;
        private Vector3 _initialPosition;
        private Vector3 _target;
        private Transform _transform;
        private ShootingPlace _shootingPlace;
        private Coroutine _moveCoroutine;
        private GridCell _cell;
        private bool _isMoving;
        private bool _isNewMove = true;
        private float _speed;

        public event Action Arrived;
        public event Action Escaped;

        private void Awake()
        {
            _transform = transform;
            _initialPosition = _transform.position;
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

        public void SetDefaultSetting()
        {
            StopMoving();
            _isMoving = false;
            _isNewMove = true;
        }

        public void GoEscape()
        {
            _shootingPlace.ChangeEmptyStatus(true);
            _target = _escapePlace;
            _moveCoroutine = StartCoroutine(MoveRoutine());
        }

        private void StopMoving()
        {
            if (_moveCoroutine == null) return;

            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        private IEnumerator MoveRoutine()
        {
            _isMoving = true;

            while (_isMoving)
            {
                var direction = _target - _transform.position;
                _transform.position += _speed * Time.deltaTime * direction.normalized;
                _transform.LookAt(_target);

                if (Vector3.Distance(_target, _transform.position) < ARRIVAL_THRESHOLD)
                {
                    _transform.position = _target;

                    if (_target == _cachedShootingTarget)
                    {
                        Arrived?.Invoke();
                        _isMoving = false;
                    }

                    SelectTarget();
                }

                yield return null;
            }
        }

        private void SelectTarget()
        {
            if (_target == _cachedCellTarget)
            {
                var nextCell = RoadFinder.GetOptimalNextCell(_cell);

                if (nextCell)
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
                _isMoving = false;
                Escaped?.Invoke();
            }
        }

        private Vector3 GetCurrentTarget(Vector3 targetPosition)
        {
            return new Vector3(targetPosition.x, _initialPosition.y, targetPosition.z);
        }
    }
}