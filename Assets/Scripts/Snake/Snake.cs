using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Snake : MonoBehaviour
{
    [Header("Snake Settings")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _segmentDistance = 2f;

    [Header("Prefabs")]
    [SerializeField] private SnakeSegment _segmentPrefab;
    [SerializeField] private GameObject _headPrefab;

    [Header("Recoil Settings")]
    [SerializeField] private float _recoilDuration = 0.3f;

    private SplineContainer _splineContainer;
    private Transform _head;
    private readonly List<SnakeSegment> _segments = new();
    private float _currentDistance = 0f;

    private readonly Queue<RecoilRequest> _recoilQueue = new();
    private bool _isRecoiling = false;

    public float MoveSpeed => _moveSpeed;
    public float NormalizedDistance { get; private set; }

    private class RecoilRequest
    {
        public SnakeSegment Segment;
        public int Index;
    }

    public void InitializeSnake(List<CubeStack> stacks, SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;
        _segments.Clear();
        _currentDistance = 0f;

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

        UpdateAllSegments();

        // запускаем движение через корутину
        StartCoroutine(SnakeMovement());
    }

    private IEnumerator SnakeMovement()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_splineContainer != null && !_isRecoiling)
            {
                _currentDistance += _moveSpeed * Time.deltaTime;
                PlaceOnSpline(_head, _currentDistance);

                NormalizedDistance = _splineContainer.Spline.GetLength() > 0
                    ? Mathf.Clamp01(_currentDistance / _splineContainer.Spline.GetLength())
                    : 0f;

                UpdateAllSegments();
            }

            if (_segments.Count == 0)
            {
                isWork = false;
            }

            yield return null;
        }
    }

    private void PlaceOnSpline(Transform target, float distance)
    {
        if (_splineContainer == null) return;
        float t = Mathf.Clamp01(distance / _splineContainer.Spline.GetLength());
        _splineContainer.Evaluate(t, out var pos, out var tangent, out var up);
        target.position = pos;
        target.rotation = Quaternion.LookRotation(tangent, up);
    }

    private void UpdateAllSegments()
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            float dist = _currentDistance - _segmentDistance * (i + 1);

            if (dist > 0f)
            {
                _segments[i].SetActiveSegment(true);
                PlaceOnSpline(_segments[i].transform, dist);
            }
        }
    }

    public void DestroySegment(SnakeSegment segmentToDestroy)
    {
        int destroyedIndex = _segments.IndexOf(segmentToDestroy);
        if (destroyedIndex == -1) return;

        _recoilQueue.Enqueue(new RecoilRequest { Segment = segmentToDestroy, Index = destroyedIndex });

        if (!_isRecoiling)
            StartCoroutine(ProcessRecoilQueue());
    }

    private IEnumerator ProcessRecoilQueue()
    {
        _isRecoiling = true;

        while (_recoilQueue.Count > 0)
        {
            RecoilRequest request = _recoilQueue.Dequeue();

            List<SnakeSegment> segmentsToRecoil = new List<SnakeSegment>();
            for (int i = 0; i < request.Index; i++)
                segmentsToRecoil.Add(_segments[i]);

            float startHead = _currentDistance;
            float targetHead = _currentDistance - _segmentDistance;

            float[] startDistances = new float[segmentsToRecoil.Count];
            float[] targetDistances = new float[segmentsToRecoil.Count];

            for (int i = 0; i < segmentsToRecoil.Count; i++)
            {
                startDistances[i] = _currentDistance - _segmentDistance * (i + 1);
                targetDistances[i] = startDistances[i] - _segmentDistance;
            }

            float timer = 0f;
            while (timer < _recoilDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / _recoilDuration);
                float smooth = Mathf.Pow(t, 0.5f);

                _currentDistance = Mathf.Lerp(startHead, targetHead, smooth);
                PlaceOnSpline(_head, _currentDistance);

                for (int i = 0; i < segmentsToRecoil.Count; i++)
                {
                    segmentsToRecoil[i].SetActiveSegment(true);
                    float dist = Mathf.Lerp(startDistances[i], targetDistances[i], smooth);
                    PlaceOnSpline(segmentsToRecoil[i].transform, dist);
                }

                yield return null;
            }

            _currentDistance = targetHead;
            PlaceOnSpline(_head, _currentDistance);
            for (int i = 0; i < segmentsToRecoil.Count; i++)
                PlaceOnSpline(segmentsToRecoil[i].transform, targetDistances[i]);

            _segments.Remove(request.Segment);
            Destroy(request.Segment.gameObject);
        }

        _isRecoiling = false;
    }
}
