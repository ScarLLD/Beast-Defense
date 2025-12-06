using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SnakeSpeedControl))]
public class Snake : MonoBehaviour
{
    [Header("Snake Settings")]
    [SerializeField] private Animator _animator;
    [SerializeField] private SnakeHead _head;
    [SerializeField] private Transform _modelContainer;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _moveBackSpeed = 20f;
    [SerializeField] private float _segmentDistance = 1.15f;
    [SerializeField] private float _segmentRollback = 1.5f;
    [SerializeField] private float _snakeRollback = 3f;
    [SerializeField] private float _startSplinePosition = 0;

    [Header("Prefabs")]
    [SerializeField] private SnakeSegment _segmentPrefab;

    [Header("Recoil Settings")]
    [SerializeField] private float _recoilDuration = 0.3f;
    [SerializeField] private AnimationCurve _recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private readonly List<SnakeSegment> _savedSegments = new();
    private readonly List<SnakeSegment> _playableSegments = new();
    private readonly Queue<SnakeSegment> _recoilQueue = new();
    private int _startSegmentsCount;
    private float _splinePosition;
    private float _splineLength;
    private SplineContainer _splineContainer;
    private SnakeSpeedControl _speedControl;
    private DeathModule _deathModule;
    private Vector3 _initialHeadSize;
    private Beast _beast;
    private Coroutine _movementCoroutine;
    private Coroutine _recoilCoroutine;
    private bool _isRecoiling = false;

    public float MoveSpeed { get; private set; }
    public float BaseSpeed { get; private set; }
    public float NormalizedPosition { get; private set; }
    public Transform ModelContainer => _modelContainer;

    public event Action<float, float> SegmentsCountChanged;

    private void Awake()
    {
        _initialHeadSize = _head.transform.localScale;
        _speedControl = GetComponent<SnakeSpeedControl>();
    }

    public void InitializeSnake(List<CubeStack> stacks, SplineContainer splineContainer, DeathModule deathModule, Beast beast)
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

        MoveSpeed = _moveSpeed;
        _animator.Rebind();
        _animator.StopPlayback();

        _head.enabled = false;
        _head.transform.localScale = _initialHeadSize;
        PlaceOnSpline(_head.transform, _splinePosition);

        _splinePosition = _startSplinePosition;
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

                if ((_head.IsPlaying == false || _beast.IsMoving == false)
                    && _beast.TryApproachNotify(NormalizedPosition))
                {
                    OpenMouth();
                }
            }

            yield return null;
        }

        if (_playableSegments.Count == 0)
        {
            _head.enabled = false;
            _deathModule.KillSnake(_head.transform);
        }

        if (MoveSpeed == 0)
        {
            _deathModule.KillBeast(_beast.transform);
            _animator.SetTrigger("isMouthClose");
        }
    }

    private void OpenMouth()
    {
        _head.ChangeParticleSpeed(MoveSpeed);
        _animator.SetTrigger("isMouthOpen");
    }

    private void UpdateHeadPosition()
    {
        PlaceOnSpline(_head.transform, _splinePosition);
        NormalizedPosition = _splineLength > 0 ?
            Mathf.Clamp01(_splinePosition / _splineLength) : 0f;

        bool shouldBeActive = _splinePosition > 0f;
        _head.gameObject.SetActive(shouldBeActive);
    }

    private void UpdateSegmentsPosition()
    {
        float headSplinePos = _splinePosition;
        float segmentSplinePos = headSplinePos - _segmentDistance - _segmentRollback - _snakeRollback;

        for (int i = 0; i < _playableSegments.Count; i++)
        {
            var segment = _playableSegments[i];
            if (segment == null) continue;

            bool shouldBeActive = segmentSplinePos > 0f;
            segment.SetActiveSegment(shouldBeActive);

            if (shouldBeActive)
            {
                PlaceOnSpline(segment.transform, segmentSplinePos);
            }

            segmentSplinePos -= _segmentDistance;
        }
    }

    private void PlaceOnSpline(Transform target, float distance)
    {
        if (_splineContainer == null) return;

        float t = Mathf.Clamp01(distance / _splineLength);
        _splineContainer.Evaluate(t, out var position, out var tangent, out var up);
        position.y += transform.localScale.y;

        Vector3 safeTangent = (Vector3)tangent;
        Vector3 safeUp = (Vector3)up;

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
            if (segmentToDestroy == null || !_playableSegments.Contains(segmentToDestroy)) continue;

            yield return _recoilCoroutine = StartCoroutine(PerformRecoil(segmentToDestroy));
        }

        _isRecoiling = false;
    }

    private IEnumerator PerformRecoil(SnakeSegment segmentToDestroy)
    {
        int targetIndex = _playableSegments.IndexOf(segmentToDestroy);
        if (targetIndex == -1) yield break;

        var allSegments = new List<SnakeSegment>(_playableSegments);
        var startPositions = new float[allSegments.Count + 1];

        startPositions[0] = _splinePosition;
        for (int i = 0; i < allSegments.Count; i++)
        {
            startPositions[i + 1] = _splinePosition - _segmentDistance - _segmentRollback - _snakeRollback - (_segmentDistance * i);
        }

        float timer = 0f;
        while (timer < _recoilDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _recoilDuration;
            float smoothT = _recoilCurve.Evaluate(t);

            float headTargetPos = startPositions[0] - _snakeRollback;
            _splinePosition = Mathf.Lerp(startPositions[0], headTargetPos, smoothT);
            UpdateHeadPosition();

            for (int i = 0; i < allSegments.Count; i++)
            {
                if (allSegments[i] != null)
                {
                    float segmentTargetPos = startPositions[i + 1] - _snakeRollback;
                    float currentPos = Mathf.Lerp(startPositions[i + 1], segmentTargetPos, smoothT);
                    PlaceOnSpline(allSegments[i].transform, currentPos);
                }
            }

            yield return null;
        }

        float finalHeadPos = startPositions[0] - _snakeRollback;
        _splinePosition = finalHeadPos;
        UpdateHeadPosition();

        for (int i = 0; i < allSegments.Count; i++)
        {
            if (allSegments[i] != null)
            {
                float finalSegmentPos = startPositions[i + 1] - _snakeRollback;
                PlaceOnSpline(allSegments[i].transform, finalSegmentPos);
            }
        }

        if (segmentToDestroy.gameObject != null)
        {
            _playableSegments.Remove(segmentToDestroy);
            segmentToDestroy.gameObject.SetActive(false);
        }

        SegmentsCountChanged?.Invoke(_playableSegments.Count, _startSegmentsCount);

        _recoilCoroutine = null;
    }
}