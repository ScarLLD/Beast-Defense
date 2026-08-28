using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Effects;
using Game.Scripts.Options;
using UnityEngine;

namespace Game.Scripts.MiniGameCore
{
    [RequireComponent(typeof(Rigidbody))]
    public class MGSnake : MonoBehaviour
    {
        private readonly List<GameObject> _bodyParts = new();
        private readonly List<Vector3> _positionsHistory = new();
        
        [Header("Movement Settings")] 
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _steerSpeed = 180f;
        [SerializeField] private int _gap = 10;

        [Header("Body Settings")] 
        [SerializeField] private GameObject _bodyContainer;
        [SerializeField] private MGCube _bodyPrefab;
        [SerializeField] private float _growInterval = 3f;
        [SerializeField] private float _tailPullback = 0.5f;

        [Header("Other")] 
        [SerializeField] private DOTWeenAnimator _animator;
        [SerializeField] private DeathAnimator _deathAnimator;
        [SerializeField] private BeastCollector _collector;
        [SerializeField] private AudioPlayer _audioPlayer;
        
        private Coroutine _movementCoroutine;
        private Coroutine _growCoroutine;
        private WaitForSeconds _growSleep;
        private Rigidbody _rb;
        private float _steerDirection;
        private bool _isMove;

        public event Action Died;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ |
                              RigidbodyConstraints.FreezePositionY;

            _growSleep = new WaitForSeconds(_growInterval);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out MGBeast beast))
            {
                if (_collector.IsBeastsFull) return;

                _audioPlayer.PlayBeastJumpSound();
                _collector.IncreaseBeastCount();
                _deathAnimator.KillRoutine(beast.transform, Color.white);
            }
            else
            {
                Die();
            }
        }

        public void ResetSettings()
        {
            StopAllCoroutines();
            _isMove = false;
            ClearBody();
            _positionsHistory.Clear();
        }

        public void SetBodyColor(Color color)
        {
            _bodyPrefab.SetColor(color);
        }

        public void StartMove()
        {
            _isMove = true;

            if (_growCoroutine != null)
                StopCoroutine(_growCoroutine);
            _growCoroutine = StartCoroutine(GrowSnakeRoutine());

            if (_movementCoroutine != null)
                StopCoroutine(_movementCoroutine);
            _movementCoroutine = StartCoroutine(MovementRoutine());

            _rb.velocity = transform.forward * _moveSpeed;
        }

        public void Die()
        {
            _isMove = false;

            StopCoroutine(_growCoroutine);
            StopCoroutine(_movementCoroutine);

            _rb.velocity = Vector3.zero;

            ClearBody();
            Died?.Invoke();
        }

        private IEnumerator MovementRoutine()
        {
            while (_isMove)
            {
                yield return new WaitForFixedUpdate();

                var moveDirection = transform.forward * _moveSpeed;
                _rb.velocity = new Vector3(moveDirection.x, _rb.velocity.y, moveDirection.z);

                if (Application.isEditor || !Application.isMobilePlatform)
                {
                    _steerDirection = Input.GetAxis("Horizontal");
                }
                else if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    var screenCenter = Screen.width * 0.5f;

                    if (touch.position.x < screenCenter)
                        _steerDirection = -1f;
                    else
                        _steerDirection = 1f;
                }
                else
                {
                    _steerDirection = 0f;
                }

                var turnRotation = Quaternion.Euler(0f, _steerDirection * _steerSpeed * Time.fixedDeltaTime, 0f);
                _rb.MoveRotation(_rb.rotation * turnRotation);

                _positionsHistory.Insert(0, transform.position);
                MoveBodyParts();
            }
        }

        private void MoveBodyParts()
        {
            var index = 0;
            foreach (var body in _bodyParts)
            {
                if (!body) continue;

                var historyIndex = (index + 1) * _gap;
                var pullbackIndex = Mathf.FloorToInt(historyIndex + _tailPullback);

                if (pullbackIndex < _positionsHistory.Count)
                {
                    var targetPoint = _positionsHistory[pullbackIndex];
                    body.transform.position = targetPoint;

                    if (pullbackIndex + 1 < _positionsHistory.Count)
                    {
                        var nextPoint = _positionsHistory[pullbackIndex + 1];
                        var direction = nextPoint - targetPoint;
                        
                        if (direction.magnitude > 0.001f) 
                            body.transform.rotation = Quaternion.LookRotation(direction);
                    }
                }
                else if (_positionsHistory.Count > 0)
                {
                    var targetPoint = _positionsHistory[^1];
                    body.transform.position = targetPoint;
                }

                index++;
            }
        }

        private IEnumerator GrowSnakeRoutine()
        {
            while (_isMove)
            {
                yield return _growSleep;
                
                GrowSnake();
            }
        }

        private void GrowSnake()
        {
            if (!_isMove) return;

            Vector3 spawnPosition;
            Quaternion spawnRotation;

            if (_bodyParts.Count > 0 && _bodyParts[^1])
            {
                var lastSegment = _bodyParts[^1];
                spawnPosition = lastSegment.transform.position - lastSegment.transform.forward * 1.5f;
                spawnRotation = lastSegment.transform.rotation;
            }
            else
            {
                spawnPosition = transform.position - transform.forward * 2f;
                spawnRotation = transform.rotation;
            }

            spawnPosition.y = transform.position.y;

            var body = Instantiate(_bodyPrefab.gameObject, spawnPosition, spawnRotation);
            body.transform.parent = _bodyContainer.transform;
            _bodyParts.Add(body);

            if (_animator && body) DOTWeenAnimator.DoScaleUp(body);
        }

        private void ClearBody()
        {
            foreach (var bodyPart in _bodyParts.Where(bodyPart => bodyPart))
                Destroy(bodyPart);

            _bodyParts.Clear();
        }
    }
}