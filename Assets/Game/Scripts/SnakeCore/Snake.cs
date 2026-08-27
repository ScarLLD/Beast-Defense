using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.BeastCore;
using Game.Scripts.Core;
using Game.Scripts.CubeCore;
using Game.Scripts.Lifecycle;
using UnityEngine;
using UnityEngine.Splines;

namespace Game.Scripts.SnakeCore
{
    [RequireComponent(typeof(SnakeSpeedControl))]
    public class Snake : MonoBehaviour
    {
        private static readonly int IsMouthOpen = Animator.StringToHash("isMouthOpen");

        [Header("Snake Settings")] [SerializeField]
        private Animator _animator;

        [SerializeField] private SnakeHead _head;
        [SerializeField] private Transform _modelContainer;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _moveBackSpeed = 20f;
        [SerializeField] private float _segmentDistance = 1.15f;
        [SerializeField] private float _segmentRollback = 1.5f;
        [SerializeField] private float _headRollback = 1.5f;

        [SerializeField] private float _startSplinePosition;

        [Header("Prefabs")] [SerializeField] private SnakeSegment _segmentPrefab;

        [Header("Recoil Settings")] [SerializeField]
        private float _recoilDuration = 0.3f;

        [SerializeField] private AnimationCurve _recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private readonly List<SnakeSegment> _playableSegments = new();
        private readonly Queue<SnakeSegment> _recoilQueue = new();
        private readonly List<SnakeSegment> _savedSegments = new();
        private Beast _beast;
        private DeathModule _deathModule;
        private Vector3 _initialHeadSize;
        private bool _isRecoiling;
        private Coroutine _movementCoroutine;
        private Coroutine _recoilCoroutine;
        private SnakeSpeedControl _speedControl;
        private SplineContainer _splineContainer;
        private float _splineLength;
        private float _splinePosition;

        private int _startSegmentsCount;

        public float MoveSpeed { get; private set; }
        public float BaseSpeed { get; private set; }
        public float NormalizedPosition { get; private set; }
        public Transform ModelContainer => _modelContainer;

        private void Awake()
        {
            _initialHeadSize = _head.transform.localScale;
            _speedControl = GetComponent<SnakeSpeedControl>();
            _isRecoiling = false;
        }

        public event Action<float, float> SegmentsCountChanged;

        public void InitializeSnake(List<CubeStack> stacks, SplineContainer splineContainer, DeathModule deathModule,
            Beast beast)
        {
            _beast = beast;
            _deathModule = deathModule;
            MoveSpeed = _moveSpeed;
            BaseSpeed = _moveSpeed;
            _splineContainer = splineContainer;
            _splineLength = _splineContainer.Spline.GetLength();

            CreateSegmentsFromStacks(stacks);

            SetDefaultSetting();
        }

        public void StartMove()
        {
            _animator.enabled = true;
            _speedControl.StartControl();
            _movementCoroutine = StartCoroutine(SnakeMovement());
        }

        public void DestroySegment(SnakeSegment segmentToDestroy)
        {
            if (!segmentToDestroy || !_playableSegments.Contains(segmentToDestroy)) return;

            _recoilQueue.Enqueue(segmentToDestroy);

            if (!_isRecoiling)
                StartCoroutine(ProcessRecoilQueue());
        }

        public void ChangeSpeed(float newSpeed)
        {
            MoveSpeed = newSpeed;
        }

        public IEnumerator GetBackToStart()
        {
            while (_splinePosition > 0)
            {
                if (_recoilCoroutine == null)
                {
                    _splinePosition -= _moveBackSpeed * (NormalizedPosition + 1) * Time.deltaTime;
                    UpdateHeadPosition();
                    UpdateSegmentsPosition();
                }

                yield return null;
            }
        }

        public void SetDefaultSetting()
        {
            Cleanup();

            MoveSpeed = _moveSpeed;
            _animator.Rebind();
            _animator.StopPlayback();

            _head.enabled = false;
            _head.SetDefaultSetting();
            _head.transform.localScale = _initialHeadSize;
            PlaceOnSpline(_head.transform, _splinePosition);

            _splinePosition = _startSplinePosition;
        }

        public void CreateSegmentsFromStacks(List<CubeStack> stacks)
        {
            if (stacks == null) return;

            ClearSegments();

            stacks = UserUtils.ShuffleList(stacks);

            _startSegmentsCount = 0;

            foreach (var stack in stacks)
            {
                if (!stack) continue;

                var segmentsCount = stack.Count / 4;
                for (var i = 0; i < segmentsCount; i++)
                {
                    var segment = Instantiate(_segmentPrefab, transform);
                    segment.Init(stack.Material, this);
                    segment.SetActiveSegment(false);
                    _savedSegments.Add(segment);
                    _startSegmentsCount++;
                }
            }

            foreach (var segment in _savedSegments) _playableSegments.Add(segment);

            SegmentsCountChanged?.Invoke(_savedSegments.Count, _startSegmentsCount);
        }

        private void ClearSegments()
        {
            ClearPlayableSegments();
            ClearSavedSegments();
        }

        private void ClearSavedSegments()
        {
            if (_savedSegments.Count <= 0) return;

            foreach (var segment in _savedSegments) Destroy(segment.gameObject);

            _savedSegments.Clear();
        }

        private void ClearPlayableSegments()
        {
            if (_playableSegments.Count <= 0) return;

            foreach (var segment in _playableSegments) Destroy(segment.gameObject);

            _playableSegments.Clear();
        }

        private IEnumerator SnakeMovement()
        {
            while (_playableSegments.Count > 0 && MoveSpeed != 0)
            {
                if (!_isRecoiling)
                {
                    _splinePosition += MoveSpeed * Time.deltaTime;
                    UpdateHeadPosition();
                    UpdateSegmentsPosition();

                    if ((!_head.IsPlaying || !_beast.IsMoving)
                        && _beast.TryApproachNotify(NormalizedPosition))
                        OpenMouth();
                }

                yield return null;
            }

            if (_playableSegments.Count == 0)
            {
                _deathModule.KillSnake(_head.transform);
                _animator.Rebind();
                _animator.StopPlayback();
                _head.enabled = false;
            }
            else if (MoveSpeed == 0)
            {
                _deathModule.KillBeast(_beast.transform);
                _animator.Rebind();
                _animator.enabled = false;
                _head.enabled = false;
            }
        }

        private void OpenMouth()
        {
            _head.ChangeParticleSpeed(MoveSpeed);
            _animator.SetTrigger(IsMouthOpen);
        }

        private void UpdateHeadPosition()
        {
            var shouldBeActive = _splinePosition - _headRollback > 0f;
            _head.gameObject.SetActive(shouldBeActive);

            PlaceOnSpline(_head.transform, _splinePosition - _headRollback);
            NormalizedPosition = _splineLength > 0 ? Mathf.Clamp01(_splinePosition / _splineLength) : 0f;
        }

        private void UpdateSegmentsPosition()
        {
            var splinePosition = _splinePosition - _segmentDistance - _segmentRollback;

            foreach (var segment in _playableSegments)
            {
                if (!segment) continue;

                var shouldBeActive = splinePosition > 0f;
                segment.SetActiveSegment(shouldBeActive);

                if (shouldBeActive) PlaceOnSpline(segment.transform, splinePosition);

                splinePosition -= _segmentDistance;
            }
        }

        private void PlaceOnSpline(Transform target, float distance)
        {
            if (!_splineContainer) return;

            var t = Mathf.Clamp01(distance / _splineLength);
            _splineContainer.Evaluate(t, out var position, out var tangent, out var up);
            position.y += transform.localScale.y;

            var safeTangent = (Vector3)tangent;
            var safeUp = (Vector3)up;

            if (safeTangent == Vector3.zero) safeTangent = Vector3.forward;
            if (safeUp == Vector3.zero) safeUp = Vector3.up;

            target.SetPositionAndRotation(position, Quaternion.LookRotation(safeTangent, safeUp));
        }

        private IEnumerator ProcessRecoilQueue()
        {
            _isRecoiling = true;

            while (_recoilQueue.Count > 0)
            {
                var segmentToDestroy = _recoilQueue.Dequeue();
                if (!segmentToDestroy || !_playableSegments.Contains(segmentToDestroy)) continue;

                yield return _recoilCoroutine = StartCoroutine(PerformRecoil(segmentToDestroy));
            }

            _isRecoiling = false;
        }

        private IEnumerator PerformRecoil(SnakeSegment segmentToDestroy)
        {
            var targetIndex = _playableSegments.IndexOf(segmentToDestroy);
            if (targetIndex == -1) yield break;

            var segmentsToRecoil = new SnakeSegment[targetIndex];
            for (var i = 0; i < targetIndex; i++) segmentsToRecoil[i] = _playableSegments[i];

            var startHeadPosition = _splinePosition;
            var targetHeadPosition = _splinePosition - _segmentDistance;

            var startPosition = new float[segmentsToRecoil.Length];
            var targetPosition = new float[segmentsToRecoil.Length];

            for (var i = 0; i < segmentsToRecoil.Length; i++)
            {
                startPosition[i] = _splinePosition - _segmentDistance - _segmentRollback - _segmentDistance * i;
                targetPosition[i] = startPosition[i] - _segmentDistance;
            }

            var timer = 0f;

            while (timer < _recoilDuration)
            {
                timer += Time.deltaTime;
                var t = timer / _recoilDuration;
                var smoothT = _recoilCurve.Evaluate(t);

                _splinePosition = Mathf.Lerp(startHeadPosition, targetHeadPosition, smoothT);

                UpdateHeadPosition();

                for (var i = 0; i < segmentsToRecoil.Length; i++)
                {
                    if (!segmentsToRecoil[i]) continue;

                    var dist = Mathf.Lerp(startPosition[i], targetPosition[i], smoothT);
                    PlaceOnSpline(segmentsToRecoil[i].transform, dist);
                }

                yield return null;
            }

            _splinePosition = targetHeadPosition;

            UpdateHeadPosition();

            if (segmentToDestroy.gameObject)
            {
                _playableSegments.Remove(segmentToDestroy);
                segmentToDestroy.gameObject.SetActive(false);
            }

            SegmentsCountChanged?.Invoke(_playableSegments.Count, _startSegmentsCount);
            UpdateSegmentsPosition();

            _recoilCoroutine = null;
        }

        private void Cleanup()
        {
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
            }

            _recoilQueue.Clear();
        }
    }
}