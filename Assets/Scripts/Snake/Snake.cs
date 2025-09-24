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
    [SerializeField] private float _segmentDistance = 1.15f;
    [SerializeField] private float _segmentRollback = 1.5f;

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
    private float _currentDistance;
    private int _startSegmentsCount;
    private bool _isRecoiling = false;
    private SplineContainer _splineContainer;
    private SnakeSpeedControl _speedControl;
    private Transform _head;
    private Beast _beast;
    private float _splineLength;
    private Coroutine _movementCoroutine;

    public float MoveSpeed { get; private set; }
    public float NormalizedDistance { get; private set; }

    public event Action<float, float> SegmentsCountChanged;

    private void Awake()
    {
        _speedControl = GetComponent<SnakeSpeedControl>();
    }

    public void InitializeSnake(List<CubeStack> stacks, SplineContainer splineContainer, Beast beast, Game game)
    {
        _beast = beast;
        MoveSpeed = _moveSpeed;
        _splineContainer = splineContainer;
        _splineLength = _splineContainer.Spline.GetLength();

        Cleanup();

        CreateHead();
        CreateSegmentsFromStacks(stacks);

        _movementCoroutine = StartCoroutine(SnakeMovement(game));
        _speedControl.StartControl();
    }

    private void Cleanup()
    {
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }

        foreach (var segment in _segments)
        {
            if (segment != null)
                Destroy(segment.gameObject);
        }
        _segments.Clear();
        _recoilQueue.Clear();

        if (_head != null)
            Destroy(_head.gameObject);
    }

    private void CreateHead()
    {
        _head = Instantiate(_headPrefab, transform).transform;
        _currentDistance = -2f;
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

    private IEnumerator SnakeMovement(Game game)
    {
        while (_segments.Count > 0 && NormalizedDistance < 0.999f)
        {
            if (!_isRecoiling)
            {
                _currentDistance += MoveSpeed * Time.deltaTime;
                UpdateHeadPosition();
                UpdateSegmentsPosition();
                _beast.ApproachNotify(NormalizedDistance);
            }
            yield return null;
        }

        if (_segments.Count == 0)
            yield return StartCoroutine(DeathRoutine());

        game.EndGame();
    }

    private void UpdateHeadPosition()
    {
        PlaceOnSpline(_head, _currentDistance);
        NormalizedDistance = _splineLength > 0 ?
            Mathf.Clamp01(_currentDistance / _splineLength) : 0f;
    }

    private void UpdateSegmentsPosition()
    {
        float currentDist = _currentDistance - _segmentDistance - _segmentRollback;
        int activeSegments = 0;

        for (int i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];
            if (segment == null) continue;

            bool shouldBeActive = currentDist > 0f;
            segment.SetActiveSegment(shouldBeActive);

            if (shouldBeActive)
            {
                PlaceOnSpline(segment.transform, currentDist);
                activeSegments++;
            }

            currentDist -= _segmentDistance;
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

        float startHeadDistance = _currentDistance;
        float targetHeadDistance = _currentDistance - _segmentDistance;

        var startDistances = new float[segmentsToRecoil.Length];
        var targetDistances = new float[segmentsToRecoil.Length];

        for (int i = 0; i < segmentsToRecoil.Length; i++)
        {
            startDistances[i] = _currentDistance - _segmentDistance - _segmentRollback - (_segmentDistance * i);
            targetDistances[i] = startDistances[i] - _segmentDistance;
        }

        float timer = 0f;
        while (timer < _recoilDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _recoilDuration;
            float smoothT = _recoilCurve.Evaluate(t);

            _currentDistance = Mathf.Lerp(startHeadDistance, targetHeadDistance, smoothT);
            UpdateHeadPosition();

            for (int i = 0; i < segmentsToRecoil.Length; i++)
            {
                if (segmentsToRecoil[i] != null)
                {
                    float dist = Mathf.Lerp(startDistances[i], targetDistances[i], smoothT);
                    PlaceOnSpline(segmentsToRecoil[i].transform, dist);
                }
            }
            yield return null;
        }

        _currentDistance = targetHeadDistance;
        UpdateHeadPosition();

        _segments.Remove(segmentToDestroy);
        Destroy(segmentToDestroy.gameObject);

        SegmentsCountChanged?.Invoke(_segments.Count, _startSegmentsCount);
        UpdateSegmentsPosition();
    }
}