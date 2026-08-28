using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Options;
using UnityEngine;
using UnityEngine.Splines;

namespace Game.Scripts.BeastCore
{
    [RequireComponent(typeof(BeastAnimator))]
    public class Beast : MonoBehaviour
    {
        private const float ARRIVAL_THRESHOLD = 0.005f;
        private const float ESCAPE_THRESHOLD = 0.15f;
        private const float START_SPLINE_POSITION = 0.5f;
        private const float INITIAL_TARGET_PERCENTAGE = 0.75f;
        private const float FINAL_TARGET_PERCENTAGE = 1f;

        [SerializeField] private float _speedMultiplier = 3f;
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _rotateDuration = 0.3f;

        private Vector3 _originalScale;
        private Queue<float> _targetPercentages;
        private SplineContainer _splineContainer;
        private AudioPlayer _audioPlayer;
        private BeastAnimator _animator;
        private Coroutine _rotateCoroutine;
        private Coroutine _moveCoroutine;
        private Transform _transform;
        private float _currentSplinePosition;
        private float _snakeSpeed;

        private float _cachedSplineLength;

        public bool IsMoving { get; private set; }

        private void Awake()
        {
            _transform = transform;
            _originalScale = _transform.localScale;
            _targetPercentages = new Queue<float>();
            _animator = GetComponent<BeastAnimator>();
        }

        public void Init(float snakeSpeed, SplineContainer splineContainer, AudioPlayer audioPlayer)
        {
            if (snakeSpeed < 0)
                throw new ArgumentException("SnakeSpeed не может быть меньше 0.", nameof(snakeSpeed));

            if (!splineContainer)
                throw new ArgumentNullException("splineContainer не может быть null.", nameof(splineContainer));

            if (!audioPlayer)
                throw new ArgumentNullException("audioPlayer не может быть null.", nameof(audioPlayer));

            _snakeSpeed = snakeSpeed;
            _splineContainer = splineContainer;
            _audioPlayer = audioPlayer;

            _cachedSplineLength = _splineContainer.Spline.GetLength();

            SetDefaultSettings();
        }

        public void SetDefaultSettings()
        {
            Cleanup();
            _animator.ResetSettings();
            _animator.EnableAnimator(true);

            IsMoving = false;

            gameObject.SetActive(true);
            _transform.localScale = _originalScale;

            _cachedSplineLength = _splineContainer.Spline.GetLength();

            _currentSplinePosition = START_SPLINE_POSITION;

            _targetPercentages.Enqueue(INITIAL_TARGET_PERCENTAGE);
            _targetPercentages.Enqueue(FINAL_TARGET_PERCENTAGE);

            PlaceOnSpline();
            _rotateCoroutine = StartCoroutine(RotateToFace());
        }

        public void CallJumpSound()
        {
            _audioPlayer.PlayBeastJumpSound();
        }

        public bool TryApproachNotify(float snakeSplinePosition)
        {
            if (_currentSplinePosition - snakeSplinePosition >= ESCAPE_THRESHOLD)
                return false;

            if (_targetPercentages.Count <= 0)
                return false;

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }

            _moveCoroutine = StartCoroutine(MoveRoutine());
            return true;
        }

        private void Cleanup()
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;

                IsMoving = false;
            }

            if (_rotateCoroutine != null)
            {
                StopCoroutine(_rotateCoroutine);
                _rotateCoroutine = null;
            }

            _targetPercentages.Clear();
        }

        private IEnumerator MoveRoutine()
        {
            _animator.ResetSettings();
            _animator.SetWalkBool(true);

            var currentTargetPercentage = _targetPercentages.Dequeue();

            var isWork = true;

            IsMoving = true;

            while (isWork)
            {
                var moveDistance = _snakeSpeed * _speedMultiplier * Time.deltaTime / _cachedSplineLength;
                _currentSplinePosition = Mathf.MoveTowards(_currentSplinePosition, currentTargetPercentage, moveDistance);

                PlaceOnSpline();

                if (Mathf.Abs(_currentSplinePosition - currentTargetPercentage) < ARRIVAL_THRESHOLD)
                {
                    _currentSplinePosition = currentTargetPercentage;
                    PlaceOnSpline();
                    isWork = false;
                }

                yield return null;
            }

            _animator.SetWalkBool(false);

            yield return _rotateCoroutine = StartCoroutine(RotateToFace());

            IsMoving = false;
        }

        private void PlaceOnSpline()
        {
            if (!_splineContainer)
                throw new ArgumentException("_splineContainer не может быть null.", nameof(_splineContainer));

            _splineContainer.Spline.Evaluate(_currentSplinePosition,
                out var position,
                out var tangent,
                out var up);

            position.y += transform.localScale.y;
            _transform.position = position;

            if (!IsMoving) return;

            var safeTangent = (Vector3)tangent;
            var safeUp = (Vector3)up;

            if (safeTangent == Vector3.zero)
                safeTangent = Vector3.forward;

            if (safeUp == Vector3.zero)
                safeUp = Vector3.up;

            var targetRotation = Quaternion.LookRotation(safeTangent, safeUp);

            if (Quaternion.Angle(_transform.rotation, targetRotation) > 0.1f)
            {
                _transform.rotation = Quaternion.Lerp(
                    _transform.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime);
            }
        }

        private IEnumerator RotateToFace()
        {
            var targetRotation = Quaternion.LookRotation(Vector3.back);
            var startRotation = _transform.rotation;

            var timer = 0f;
            var inverseDuration = 1f / _rotateDuration;

            while (timer < _rotateDuration)
            {
                timer += Time.deltaTime;
                var t = timer * inverseDuration;
                _transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            _transform.rotation = targetRotation;
        }

        private void OnDestroy()
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
        }
    }
}