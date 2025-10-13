using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SnakeSpeedControl))]
public class Snake : MonoBehaviour
{
    [Header("Snake Settings")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _moveBackSpeed = 20f;
    [SerializeField] private float _segmentDistance = 1.15f;
    [SerializeField] private float _segmentRollback = 1.5f;
    [SerializeField] private float _startSplinePosition = 0;

    [Header("Prefabs")]
    [SerializeField] private SnakeSegment _segmentPrefab;
    [SerializeField] private GameObject _headPrefab;

    [Header("Recoil Settings")]
    [SerializeField] private float _recoilDuration = 0.3f;
    [SerializeField] private AnimationCurve _recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private readonly List<SnakeSegment> _savedSegments = new();
    private readonly List<SnakeSegment> _playableSegments = new();
    private readonly Queue<SnakeSegment> _recoilQueue = new();
    private float _splinePosition;
    private int _startSegmentsCount;
    private bool _isRecoiling = false;
    private SplineContainer _splineContainer;
    private SnakeSpeedControl _speedControl;
    private Transform _head;
    private DeathModule _deathModule;
    private Beast _beast;
    private float _splineLength;
    private Coroutine _movementCoroutine;
    private Coroutine _recoilCoroutine;
    private Animator _animator;

    public float MoveSpeed { get; private set; }
    public float NormalizedPosition { get; private set; }

    public event Action<float, float> SegmentsCountChanged;

    private void Awake()
    {
        _speedControl = GetComponent<SnakeSpeedControl>();
    }

    public void InitializeSnake(List<CubeStack> stacks, SplineContainer splineContainer, DeathModule deathModule, Beast beast)
    {
        _beast = beast;
        _deathModule = deathModule;
        MoveSpeed = _moveSpeed;
        _splineContainer = splineContainer;
        _splineLength = _splineContainer.Spline.GetLength();

        SetDefaultSetting();

        CreateHead();
        CreateSegmentsFromStacks(stacks);
        StartMove();
    }

    public void StartMove()
    {
        _movementCoroutine = StartCoroutine(SnakeMovement());
    }

    public void DestroySegment(SnakeSegment segmentToDestroy)
    {
        if (segmentToDestroy == null || !_playableSegments.Contains(segmentToDestroy)) return;

        _recoilQueue.Enqueue(segmentToDestroy);

        if (_isRecoiling == false)
            StartCoroutine(ProcessRecoilQueue());
    }

    public void ChangeSpeed(float newSpeed)
    {
        MoveSpeed = newSpeed;
    }

    public IEnumerator GetBackToStart()
    {
        bool isWork = true;

        while (isWork && _splinePosition > 0)
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

        _splinePosition = _startSplinePosition;
        MoveSpeed = _moveSpeed;
        _speedControl.StartControl();
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

    private void CreateHead()
    {
        _head = Instantiate(_headPrefab, transform).transform;
        _animator = _head.GetComponent<Animator>();
        _animator.SetBool("BeastClose", false);
        PlaceOnSpline(_head, _splinePosition);
    }

    public void CreateSegmentsFromStacks(List<CubeStack> stacks)
    {
        if (stacks == null) return;

        ClearSegments();

        _startSegmentsCount = 0;

        foreach (var stack in stacks)
        {
            if (stack == null) continue;

            int segmentsCount = stack.Count / 4;
            for (int i = 0; i < segmentsCount; i++)
            {
                var segment = Instantiate(_segmentPrefab, transform);
                segment.Init(stack.Material, this);
                segment.SetActiveSegment(false);
                _savedSegments.Add(segment);
                _startSegmentsCount++;
            }
        }

        foreach (var segment in _savedSegments)
        {
            _playableSegments.Add(segment);
        }

        SegmentsCountChanged?.Invoke(_savedSegments.Count, _startSegmentsCount);
    }

    private void ClearSegments()
    {
        ClearPlayableSegments();
        ClearSavedSegments();
    }

    private void ClearSavedSegments()
    {
        if (_savedSegments.Count > 0)
        {
            foreach (var segment in _savedSegments)
            {
                Destroy(segment.gameObject);
            }

            _savedSegments.Clear();
        }
    }

    private void ClearPlayableSegments()
    {
        if (_playableSegments.Count > 0)
        {
            foreach (var segment in _playableSegments)
            {
                Destroy(segment.gameObject);
            }

            _playableSegments.Clear();
        }
    }

    private IEnumerator SnakeMovement()
    {
        while (_playableSegments.Count > 0 && MoveSpeed != 0)
        {
            if (_isRecoiling == false)
            {
                _splinePosition += MoveSpeed * Time.deltaTime;
                UpdateHeadPosition();
                UpdateSegmentsPosition();

                if (_beast.TryApproachNotify(NormalizedPosition))
                    _animator.SetBool("BeastClose", true);
                else
                    _animator.SetBool("BeastClose", false);

            }

            yield return null;
        }

        _animator.SetBool("BeastClose", false);

        if (_playableSegments.Count == 0)
        {
            _deathModule.KillSnake(transform);

        }

        if (MoveSpeed == 0)
        {
            _animator.SetTrigger("AteBeast");
            _deathModule.KillBeast(_beast.transform);
        }
    }

    private void UpdateHeadPosition()
    {
        PlaceOnSpline(_head, _splinePosition);
        NormalizedPosition = _splineLength > 0 ?
            Mathf.Clamp01(_splinePosition / _splineLength) : 0f;

        bool shouldBeActive = _splinePosition > 0f;
        _head.gameObject.SetActive(shouldBeActive);
    }

    private void UpdateSegmentsPosition()
    {
        float splinePosition = _splinePosition - _segmentDistance - _segmentRollback;
        int activeSegments = 0;

        for (int i = 0; i < _playableSegments.Count; i++)
        {
            var segment = _playableSegments[i];
            if (segment == null) continue;

            bool shouldBeActive = splinePosition > 0f;
            segment.SetActiveSegment(shouldBeActive);

            if (shouldBeActive)
            {
                PlaceOnSpline(segment.transform, splinePosition);
                activeSegments++;
            }

            splinePosition -= _segmentDistance;
        }
    }

    private void PlaceOnSpline(Transform target, float distance)
    {
        if (_splineContainer == null) return;

        float t = Mathf.Clamp01(distance / _splineLength);
        _splineContainer.Evaluate(t, out var position, out var tangent, out var up);
        target.SetPositionAndRotation(position, Quaternion.LookRotation(tangent, up));
    }

    private IEnumerator ProcessRecoilQueue()
    {
        _isRecoiling = true;

        while (_recoilQueue.Count > 0)
        {
            var segmentToDestroy = _recoilQueue.Dequeue();
            if (segmentToDestroy == null || !_playableSegments.Contains(segmentToDestroy)) continue;

            yield return _recoilCoroutine = StartCoroutine(PerformRecoil(segmentToDestroy));
        }

        _isRecoiling = false;
    }

    private IEnumerator PerformRecoil(SnakeSegment segmentToDestroy)
    {
        int targetIndex = _playableSegments.IndexOf(segmentToDestroy);
        if (targetIndex == -1) yield break;

        var segmentsToRecoil = new SnakeSegment[targetIndex];
        for (int i = 0; i < targetIndex; i++)
        {
            segmentsToRecoil[i] = _playableSegments[i];
        }

        float startHeadPosition = _splinePosition;
        float targetHeadPosition = _splinePosition - _segmentDistance;

        var startPosition = new float[segmentsToRecoil.Length];
        var targetPosition = new float[segmentsToRecoil.Length];

        for (int i = 0; i < segmentsToRecoil.Length; i++)
        {
            startPosition[i] = _splinePosition - _segmentDistance - _segmentRollback - (_segmentDistance * i);
            targetPosition[i] = startPosition[i] - _segmentDistance;
        }

        float timer = 0f;
        while (timer < _recoilDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _recoilDuration;
            float smoothT = _recoilCurve.Evaluate(t);

            _splinePosition = Mathf.Lerp(startHeadPosition, targetHeadPosition, smoothT);
            UpdateHeadPosition();

            for (int i = 0; i < segmentsToRecoil.Length; i++)
            {
                if (segmentsToRecoil[i] != null)
                {
                    float dist = Mathf.Lerp(startPosition[i], targetPosition[i], smoothT);
                    PlaceOnSpline(segmentsToRecoil[i].transform, dist);
                }
            }
            yield return null;
        }

        _splinePosition = targetHeadPosition;
        UpdateHeadPosition();

        if (segmentToDestroy.gameObject != null)
        {
            _playableSegments.Remove(segmentToDestroy);
            segmentToDestroy.gameObject.SetActive(false);
        }

        SegmentsCountChanged?.Invoke(_playableSegments.Count, _startSegmentsCount);
        UpdateSegmentsPosition();

        _recoilCoroutine = null;
    }
}