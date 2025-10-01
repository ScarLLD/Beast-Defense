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

    [Header("Death Settings")]
    [SerializeField] private AnimationCurve _deathAnimationCurve;
    [SerializeField] private ParticleSystem _cloadParticle;
    [SerializeField] private float _deathDuration;

    private readonly List<SnakeSegment> _segments = new();
    private readonly Queue<SnakeSegment> _recoilQueue = new();
    private float _splinePosition;
    private int _startSegmentsCount;
    private bool _isRecoiling = false;
    private SplineContainer _splineContainer;
    private SnakeSpeedControl _speedControl;
    private Transform _head;
    private Beast _beast;
    private float _splineLength;
    private Coroutine _movementCoroutine;
    private Game _game;

    public float MoveSpeed { get; private set; }
    public float NormalizedPosition { get; private set; }

    public event Action<float, float> SegmentsCountChanged;

    private void Awake()
    {
        _speedControl = GetComponent<SnakeSpeedControl>();
    }

    public void InitializeSnake(List<CubeStack> stacks, SplineContainer splineContainer, Beast beast, Game game)
    {
        _game = game;
        _beast = beast;
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
        if (segmentToDestroy == null || !_segments.Contains(segmentToDestroy)) return;

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
        bool isWork = true;

        while (isWork && _splinePosition > 0)
        {
            _splinePosition -= _moveBackSpeed * Time.deltaTime;
            UpdateHeadPosition();
            UpdateSegmentsPosition();

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
        _splinePosition = _startSplinePosition;
        PlaceOnSpline(_head, 0f);
    }

    private void CreateSegmentsFromStacks(List<CubeStack> stacks)
    {
        if (stacks == null) return;

        int totalSegments = 0;
        foreach (var stack in stacks)
        {
            if (stack == null) continue;

            int segmentsCount = stack.Count / 4;
            for (int i = 0; i < segmentsCount; i++)
            {
                var segment = Instantiate(_segmentPrefab, transform);
                segment.Init(stack.Material, this);
                segment.SetActiveSegment(false);
                _segments.Add(segment);
                totalSegments++;
            }
        }

        _startSegmentsCount = totalSegments;
        SegmentsCountChanged?.Invoke(_segments.Count, _startSegmentsCount);
    }

    private IEnumerator SnakeMovement()
    {
        while (_segments.Count > 0 && MoveSpeed != 0)
        {
            if (_isRecoiling == false)
            {
                _splinePosition += MoveSpeed * Time.deltaTime;
                UpdateHeadPosition();
                UpdateSegmentsPosition();
                _beast.ApproachNotify(NormalizedPosition);
            }

            yield return null;
        }

        if (_segments.Count == 0)
            yield return StartCoroutine(DeathRoutine());

        _game.EndGame();
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

        for (int i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];
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

    private IEnumerator DeathRoutine()
    {
        Vector3 startScale = _head.localScale;
        float timer = 0f;

        while (timer < _deathDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _deathDuration;
            _head.localScale = Vector3.Lerp(startScale, Vector3.zero, _deathAnimationCurve.Evaluate(t));
            yield return null;
        }

        _cloadParticle.transform.position = _head.position;
        _cloadParticle.Play();
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
            if (segmentToDestroy == null || !_segments.Contains(segmentToDestroy)) continue;

            yield return StartCoroutine(PerformRecoil(segmentToDestroy));
        }

        _isRecoiling = false;
    }

    private IEnumerator PerformRecoil(SnakeSegment segmentToDestroy)
    {
        int targetIndex = _segments.IndexOf(segmentToDestroy);
        if (targetIndex == -1) yield break;

        var segmentsToRecoil = new SnakeSegment[targetIndex];
        for (int i = 0; i < targetIndex; i++)
        {
            segmentsToRecoil[i] = _segments[i];
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

        _segments.Remove(segmentToDestroy);
        Destroy(segmentToDestroy.gameObject);

        SegmentsCountChanged?.Invoke(_segments.Count, _startSegmentsCount);
        UpdateSegmentsPosition();
    }
}