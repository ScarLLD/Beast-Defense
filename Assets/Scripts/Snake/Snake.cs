using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SnakeSpeedControl))]
public class Snake : MonoBehaviour
{
    [Header("Snake Settings")]
    public float moveSpeed = 2f;
    public float segmentDistance = 1.15f;
    public float segmentRollback = 1.5f;

    [Header("Prefabs")]
    public SnakeSegment segmentPrefab;
    public GameObject headPrefab;

    [Header("Recoil Settings")]
    public float recoilDuration = 0.3f;
    public AnimationCurve recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private readonly List<SnakeSegment> _segments = new();
    private readonly Queue<RecoilRequest> _recoilQueue = new();
    private SplineContainer _splineContainer;
    private SnakeSpeedControl _speedControl;
    private float _currentDistance = -1f;
    private bool _isRecoiling = false;
    private Transform _head;

    public float MoveSpeed { get; private set; }
    public float NormalizedDistance { get; private set; }

    private void Awake()
    {
        _speedControl = GetComponent<SnakeSpeedControl>();
    }

    public void InitializeSnake(List<CubeStack> stacks, SplineContainer splineContainer)
    {
        MoveSpeed = moveSpeed;
        _splineContainer = splineContainer;
        _segments.Clear();

        if (_head != null) Destroy(_head.gameObject);
        _head = Instantiate(headPrefab, transform).transform;
        PlaceOnSpline(_head, 0f);

        if (stacks != null)
        {
            foreach (var stack in stacks)
            {
                if (stack == null) continue;
                int count = Mathf.Max(1, stack.Count / 4);
                for (int i = 0; i < count; i++)
                {
                    var seg = Instantiate(segmentPrefab, transform);
                    seg.Init(stack.Material, this);
                    seg.SetActiveSegment(false);
                    _segments.Add(seg);
                }
            }
        }

        UpdateAllSegments();
        StartCoroutine(SnakeMovement());
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

    public void Stop()
    {
        MoveSpeed = 0f;
    }

    public void Resume()
    {
        MoveSpeed = moveSpeed;
    }

    private IEnumerator SnakeMovement()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_splineContainer != null && !_isRecoiling)
            {
                _currentDistance += MoveSpeed * Time.deltaTime;
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
        _splineContainer.Evaluate(t, out var position, out var tangent, out var up);
        target.SetPositionAndRotation(position, Quaternion.LookRotation(tangent, up));
    }

    private void UpdateAllSegments()
    {
        float currentDist = _currentDistance - segmentDistance - segmentRollback;

        foreach (var seg in _segments)
        {
            if (currentDist > 0f)
            {
                seg.SetActiveSegment(true);
                PlaceOnSpline(seg.transform, currentDist);
            }

            currentDist -= segmentDistance;
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
            float targetHead = _currentDistance - segmentDistance;

            float[] startDistances = new float[segmentsToRecoil.Count];
            float[] targetDistances = new float[segmentsToRecoil.Count];

            for (int i = 0; i < segmentsToRecoil.Count; i++)
            {
                startDistances[i] = _currentDistance - segmentDistance - segmentRollback - segmentDistance * i;
                targetDistances[i] = startDistances[i] - segmentDistance;
            }

            float timer = 0f;
            while (timer < recoilDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / recoilDuration);
                float smooth = recoilCurve.Evaluate(t);

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
                    PlaceOnSpline(seg.transform, startDistances[segmentsToRecoil.IndexOf(seg)] - segmentDistance);
            }

            if (_segments.Contains(request.Segment))
            {
                _segments.Remove(request.Segment);
                Destroy(request.Segment.gameObject);
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