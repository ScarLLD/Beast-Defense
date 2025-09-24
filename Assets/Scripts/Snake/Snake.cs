using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
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
    private readonly Queue<RecoilRequest> _recoilQueue = new();
    private float _currentDistance;
    private float _startSegmentsCount;
    private bool _isRecoiling = false;
    private SplineContainer _splineContainer;
    private SnakeSpeedControl _speedControl;
    private Transform _head;
    private Beast _beast;

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
        _segments.Clear();
        _currentDistance = -2f;

        if (_head != null) Destroy(_head.gameObject);
        _head = Instantiate(_headPrefab, transform).transform;
        PlaceOnSpline(_head, 0f);

        if (stacks != null)
        {
            foreach (var stack in stacks)
            {
                if (stack == null) continue;
                int count = Mathf.Max(1, stack.Count / 4);
                for (int i = 0; i < count; i++)
                {
                    var seg = Instantiate(_segmentPrefab, transform);
                    seg.Init(stack.Material, this);
                    seg.SetActiveSegment(false);
                    _segments.Add(seg);
                }
            }
        }

        _startSegmentsCount = _segments.Count;
        UpdateAllSegments();
        StartCoroutine(SnakeMovement(game));
        _speedControl.StartControl();
    }

    public void DestroySegment(SnakeSegment segmentToDestroy)
    {
        if (segmentToDestroy == null) return;
        _recoilQueue.Enqueue(new RecoilRequest { Segment = segmentToDestroy });

        if (!_isRecoiling)
            StartCoroutine(ProcessRecoilQueue());
    }

    public void ChangeSpeed(float newSpeed)
    {
        MoveSpeed = newSpeed;
    }

    private IEnumerator SnakeMovement(Game game)
    {
        while (_segments.Count > 0 && NormalizedDistance != 1)
        {
            if (_splineContainer != null && _isRecoiling == false)
            {
                _currentDistance += MoveSpeed * Time.deltaTime;
                PlaceOnSpline(_head, _currentDistance);

                NormalizedDistance = _splineContainer.Spline.GetLength() > 0
                    ? Mathf.Clamp01(_currentDistance / _splineContainer.Spline.GetLength())
                    : 0f;

                UpdateAllSegments();
                _beast.ApproachNotify(NormalizedDistance);
            }

            yield return null;
        }

        if (_segments.Count == 0)
            yield return StartCoroutine(DeathRoutine());

        game.EndGame();
    }

    private IEnumerator DeathRoutine()
    {
        Vector3 startScale = _head.transform.localScale;

        float timer = 0f;

        while (timer < _deathDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / _deathDuration);
            float smooth = _deathAnimationCurve.Evaluate(t);
            _head.transform.localScale = Vector3.Lerp(startScale, new(0, 0, 0), smooth);

            yield return null;
        }

        _cloadParticle.transform.position = _head.transform.position;
        _cloadParticle.Play();
    }

    private void PlaceOnSpline(Transform target, float distance)
    {
        if (_splineContainer == null) return;
        float t = Mathf.Clamp01(distance / _splineContainer.Spline.GetLength());
        _splineContainer.Evaluate(t, out var position, out var tangent, out var up);
        target.SetPositionAndRotation(position, Quaternion.LookRotation(tangent, up));
    }

    private void UpdateAllSegments()
    {
        float currentDist = _currentDistance - _segmentDistance - _segmentRollback;

        foreach (var seg in _segments)
        {
            if (currentDist > 0f)
            {
                seg.SetActiveSegment(true);
                PlaceOnSpline(seg.transform, currentDist);
            }

            currentDist -= _segmentDistance;
        }
    }

    private IEnumerator ProcessRecoilQueue()
    {
        _isRecoiling = true;

        while (_recoilQueue.Count > 0)
        {
            RecoilRequest request = _recoilQueue.Dequeue();
            if (request.Segment == null || !_segments.Contains(request.Segment)) continue;

            List<SnakeSegment> segmentsToRecoil = new List<SnakeSegment>();
            foreach (var seg in _segments)
            {
                if (seg == request.Segment) break;
                segmentsToRecoil.Add(seg);
            }

            float startHead = _currentDistance;
            float targetHead = _currentDistance - _segmentDistance;

            float[] startDistances = new float[segmentsToRecoil.Count];
            float[] targetDistances = new float[segmentsToRecoil.Count];

            for (int i = 0; i < segmentsToRecoil.Count; i++)
            {
                startDistances[i] = _currentDistance - _segmentDistance - _segmentRollback - _segmentDistance * i;
                targetDistances[i] = startDistances[i] - _segmentDistance;
            }

            float timer = 0f;
            while (timer < _recoilDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / _recoilDuration);
                float smooth = _recoilCurve.Evaluate(t);

                _currentDistance = Mathf.Lerp(startHead, targetHead, smooth);
                PlaceOnSpline(_head, _currentDistance);

                for (int i = 0; i < segmentsToRecoil.Count; i++)
                {
                    if (segmentsToRecoil[i] != null)
                    {
                        segmentsToRecoil[i].SetActiveSegment(true);
                        float dist = Mathf.Lerp(startDistances[i], targetDistances[i], smooth);
                        PlaceOnSpline(segmentsToRecoil[i].transform, dist);
                    }
                }

                yield return null;
            }

            _currentDistance = targetHead;
            PlaceOnSpline(_head, _currentDistance);

            foreach (var seg in segmentsToRecoil)
            {
                if (seg != null)
                    PlaceOnSpline(seg.transform, startDistances[segmentsToRecoil.IndexOf(seg)] - _segmentDistance);
            }

            if (_segments.Contains(request.Segment))
            {
                _segments.Remove(request.Segment);
                Destroy(request.Segment.gameObject);
                SegmentsCountChanged?.Invoke(_segments.Count, _startSegmentsCount);
            }

            UpdateAllSegments();
        }

        _isRecoiling = false;
    }

    private class RecoilRequest
    {
        public SnakeSegment Segment;
    }
}