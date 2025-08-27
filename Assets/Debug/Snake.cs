using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class Snake : MonoBehaviour
{
    private SplineContainer _splineContainer;

    [Header("Snake Settings")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private int _initialLength = 5;
    [SerializeField] private float _segmentDistance = 0.5f;
    [SerializeField] private float _rotationSmoothness = 5f;

    [Header("Prefabs")]
    [SerializeField] private GameObject _segmentPrefab;
    [SerializeField] private GameObject _headPrefab;

    private float _currentDistance;
    private Transform[] _segments;
    private Transform _head;
    private bool _reachedEnd = false;
    private float _splineLength;

    public void InitializeSnake(SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;

        if (_splineContainer != null && _splineContainer.Spline != null)
        {
            _splineLength = _splineContainer.Spline.GetLength();
        }

        _head = Instantiate(_headPrefab, transform).transform;
        _segments = new Transform[_initialLength];

        // Создаем сегменты тела
        for (int i = 0; i < _initialLength; i++)
        {
            _segments[i] = Instantiate(_segmentPrefab, transform).transform;
        }

        // Позиционируем голову в начале сплайна
        MoveHeadToStart();
    }

    void MoveHeadToStart()
    {
        if (_splineContainer == null) return;

        _splineContainer.Evaluate(0f, out float3 position, out float3 tangent, out float3 upVector);
        _head.position = position;

        if (((Vector3)tangent).magnitude > 0.1f)
        {
            _head.rotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
        }
    }

    void Update()
    {
        if (_splineContainer == null) return;

        HandleInput();
        MoveAlongSpline();
        UpdateSegments();
    }

    void HandleInput()
    {
        if (_reachedEnd) return;

        // Управление с клавиатуры
        float horizontal = Input.GetAxis("Horizontal");
        _moveSpeed += horizontal * _rotationSpeed * Time.deltaTime;
        _moveSpeed = Mathf.Clamp(_moveSpeed, 1f, 10f);
    }

    void MoveAlongSpline()
    {
        if (_splineContainer == null || _splineContainer.Spline == null || _head == null) return;

        // Двигаемся только если не достигли конца
        if (!_reachedEnd)
        {
            _currentDistance += _moveSpeed * Time.deltaTime;

            // Проверяем достижение конца
            if (_currentDistance >= _splineLength)
            {
                _currentDistance = _splineLength;
                _reachedEnd = true;
                Debug.Log("Змейка достигла конца пути!");
            }
        }

        UpdateHeadPosition();
    }

    void UpdateHeadPosition()
    {
        float normalizedDistance = Mathf.Clamp01(_currentDistance / _splineLength);

        _splineContainer.Evaluate(normalizedDistance,
            out float3 position, out float3 tangent, out float3 upVector);

        _head.position = position;

        if (((Vector3)tangent).magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
            _head.rotation = Quaternion.Slerp(_head.rotation, targetRotation,
                _rotationSmoothness * Time.deltaTime);
        }
    }

    void UpdateSegments()
    {
        if (_segments == null || _segments.Length == 0 || _splineContainer == null) return;

        for (int i = 0; i < _segments.Length; i++)
        {
            float segmentDist = _currentDistance - _segmentDistance * (i + 1);

            // Если сегмент еще не должен появиться
            if (segmentDist < 0)
            {
                _segments[i].gameObject.SetActive(false);
                continue;
            }

            _segments[i].gameObject.SetActive(true);

            // Обрабатываем зацикливание (если нужно)
            if (_reachedEnd)
            {
                segmentDist = Mathf.Clamp(segmentDist, 0, _splineLength);
            }
            else
            {
                segmentDist %= _splineLength;
            }

            float normalizedDistance = Mathf.Clamp01(segmentDist / _splineLength);

            _splineContainer.Evaluate(normalizedDistance,
                out float3 position, out float3 tangent, out float3 upVector);

            // Позиция
            _segments[i].position = position;

            // Поворот - каждый сегмент имеет свой собственный целевой поворот
            if (((Vector3)tangent).magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
                _segments[i].rotation = Quaternion.Slerp(_segments[i].rotation, targetRotation,
                    _rotationSmoothness * Time.deltaTime);
            }
        }
    }

    public void AddSegment()
    {
        // Увеличиваем массив сегментов
        Transform[] newSegments = new Transform[_segments.Length + 1];
        _segments.CopyTo(newSegments, 0);

        // Создаем новый сегмент
        newSegments[^1] = Instantiate(_segmentPrefab, transform).transform;
        newSegments[^1].gameObject.SetActive(false); // Сначала скрываем

        _segments = newSegments;
    }

    public void ResetSnake()
    {
        _currentDistance = 0f;
        _reachedEnd = false;
        _moveSpeed = 2f;
        MoveHeadToStart();

        // Активируем все сегменты
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
            // Рисуем сплайн
            Gizmos.color = Color.green;
            for (float t = 0; t < 1f; t += 0.02f)
            {
                _splineContainer.Evaluate(t, out float3 position, out _, out _);
                Gizmos.DrawSphere((Vector3)position, 0.1f);
            }

            // Рисуем позицию головы
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