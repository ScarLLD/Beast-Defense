using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine.ProBuilder.Shapes;

public class Snake : MonoBehaviour
{
    [Header("Snake Settings")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 200f;
    [SerializeField] private float _segmentDistance = 2f;
    [SerializeField] private float _rotationSmoothness = 5f;

    [Header("Prefabs")]
    [SerializeField] private SnakeSegment _segmentPrefab;
    [SerializeField] private GameObject _headPrefab;

    private Queue<CubeStack> _stacks;
    private SplineContainer _splineContainer;
    private SnakeSegment[] _segments;
    private Transform _head;
    private float _currentDistance;
    private float _splineLength;
    private bool _reachedEnd = false;

    public float NormalizedDistance { get; private set; }
    public float MoveSpeed => _moveSpeed;

    public void InitializeSnake(List<CubeStack> stacks, SplineContainer splineContainer)
    {
        _stacks = new Queue<CubeStack>(stacks);
        _splineContainer = splineContainer;

        int stacksLength = 0;

        foreach (var stack in stacks)
        {
            stacksLength += stack.Count;
        }

        int initialLength = stacksLength / 4;
        Debug.Log(initialLength);

        if (_splineContainer != null && _splineContainer.Spline != null)
        {
            _splineLength = _splineContainer.Spline.GetLength();
        }

        _head = Instantiate(_headPrefab, transform).transform;
        _segments = new SnakeSegment[initialLength];

        int index = 0;

        while (_stacks.Count > 0)
        {
            var stack = _stacks.Dequeue();

            for (int i = 0; i < stack.Count / 4; i++)
            {
                _segments[index] = Instantiate(_segmentPrefab, transform);
                _segments[index].Init(stack.Material);
                index++;
            }
        }

        MoveHeadToStart();
    }

    private void MoveHeadToStart()
    {
        if (_splineContainer == null) return;

        _splineContainer.Evaluate(0f, out float3 position, out float3 tangent, out float3 upVector);
        _head.position = position;

        if (((Vector3)tangent).magnitude > 0.1f)
        {
            _head.rotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
        }
    }

    private void Update()
    {
        if (_splineContainer == null) return;

        HandleInput();
        MoveAlongSpline();
        UpdateSegments();
    }

    private void HandleInput()
    {
        if (_reachedEnd) return;

        float horizontal = Input.GetAxis("Horizontal");
        _moveSpeed += horizontal * _rotationSpeed * Time.deltaTime;
        _moveSpeed = Mathf.Clamp(_moveSpeed, 1f, 10f);
    }

    private void MoveAlongSpline()
    {
        if (_splineContainer == null || _splineContainer.Spline == null || _head == null) return;

        if (!_reachedEnd)
        {
            _currentDistance += _moveSpeed * Time.deltaTime;

            if (_currentDistance >= _splineLength)
            {
                _currentDistance = _splineLength;
                _reachedEnd = true;
                Debug.Log("Змейка достигла конца пути!");
            }
        }

        UpdateHeadPosition();
    }

    private void UpdateHeadPosition()
    {
        NormalizedDistance = Mathf.Clamp01(_currentDistance / _splineLength);

        _splineContainer.Evaluate(NormalizedDistance,
            out float3 position, out float3 tangent, out float3 upVector);

        _head.position = position;

        if (((Vector3)tangent).magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
            _head.rotation = Quaternion.Slerp(_head.rotation, targetRotation,
                _rotationSmoothness * Time.deltaTime);
        }
    }

    private void UpdateSegments()
    {
        if (_segments == null || _segments.Length == 0 || _splineContainer == null) return;

        for (int i = 0; i < _segments.Length; i++)
        {
            float segmentDist = _currentDistance - _segmentDistance * (i + 1) * 1.5f;

            if (segmentDist < 0)
            {
                _segments[i].gameObject.SetActive(false);
                continue;
            }

            _segments[i].gameObject.SetActive(true);

            float normalizedDistance = Mathf.Clamp01(segmentDist / _splineLength);

            _splineContainer.Evaluate(normalizedDistance,
                out float3 position, out float3 tangent, out float3 upVector);

            _segments[i].transform.position = position;

            if (((Vector3)tangent).magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
                _segments[i].transform.rotation = Quaternion.Slerp(_segments[i].transform.rotation, targetRotation,
                    _rotationSmoothness * Time.deltaTime);
            }
        }
    }

    public void AddSegment()
    {
        SnakeSegment[] newSegments = new SnakeSegment[_segments.Length + 1];
        _segments.CopyTo(newSegments, 0);

        newSegments[^1] = Instantiate(_segmentPrefab, transform);
        newSegments[^1].gameObject.SetActive(false); // Сначала скрываем

        _segments = newSegments;
    }

    public void ResetSnake()
    {
        _currentDistance = 0f;
        _reachedEnd = false;
        _moveSpeed = 2f;
        MoveHeadToStart();

        foreach (var segment in _segments)
        {
            if (segment != null)
            {
                segment.gameObject.SetActive(true);
            }
        }
    }

    public bool HasReachedEnd()
    {
        return _reachedEnd;
    }

    public float GetProgress()
    {
        return Mathf.Clamp01(_currentDistance / _splineLength);
    }

    void OnDrawGizmos()
    {
        if (_splineContainer != null && _splineContainer.Spline != null)
        {
            Gizmos.color = Color.green;
            for (float t = 0; t < 1f; t += 0.02f)
            {
                _splineContainer.Evaluate(t, out float3 position, out _, out _);
                Gizmos.DrawSphere((Vector3)position, 0.1f);
            }

            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                float progress = GetProgress();
                _splineContainer.Evaluate(progress, out float3 headPos, out _, out _);
                Gizmos.DrawWireSphere((Vector3)headPos, 0.3f);
            }
        }
    }
}