using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MGSnake : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _steerSpeed = 180f;
    [SerializeField] private int _gap = 10;

    [Header("Body Settings")]
    [SerializeField] private GameObject _bodyContainer;
    [SerializeField] private MGCube _bodyPrefab;
    [SerializeField] private float _growInterval = 3f;
    [SerializeField] private float _tailPullback = 0.5f;

    [Header("Other")]
    [SerializeField] private DOTWeenAnimator _animator;
    [SerializeField] private DeathAnimator _deathAnimator;
    [SerializeField] private BeastCollector _collector;

    private List<GameObject> _bodyParts = new();
    private List<Vector3> _positionsHistory = new();
    private float _steerDirection;
    private Rigidbody _rb;
    private Coroutine _movementCoroutine;
    private Coroutine _growCoroutine;
    private bool _isMove;

    public event Action Died;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Beast beast))
        {
            _deathAnimator.KillRoutine(beast.transform, Color.white);
            _collector.IncreaseBeastCount();
        }
        else if (other.gameObject.TryGetComponent(out MGCube cube))
        {
            Die();
        }
    }

    public void ResetSettings()
    {
        StopAllCoroutines();
    }

    public void SetBodyColor(Color color)
    {
        _bodyPrefab.SetColor(color);
    }

    public void StartMove()
    {
        _growCoroutine ??= StartCoroutine(GrowSnakeRoutine());
        _movementCoroutine ??= StartCoroutine(MovementRoutine());
    }

    public void Die()
    {
        _isMove = false;

        StopCoroutine(_growCoroutine);
        StopCoroutine(_movementCoroutine);

        _rb.velocity = Vector3.zero;

        Died?.Invoke();
    }

    private IEnumerator MovementRoutine()
    {
        _isMove = true;

        while (_isMove)
        {
            yield return new WaitForFixedUpdate();

            Vector3 moveDirection = transform.forward * _moveSpeed;
            _rb.velocity = new Vector3(moveDirection.x, _rb.velocity.y, moveDirection.z);

            _steerDirection = Input.GetAxis("Horizontal");
            Quaternion turnRotation = Quaternion.Euler(0f, _steerDirection * _steerSpeed * Time.fixedDeltaTime, 0f);
            _rb.MoveRotation(_rb.rotation * turnRotation);

            _positionsHistory.Insert(0, transform.position);

            MoveBodyParts();
        }
    }

    private void MoveBodyParts()
    {
        int index = 0;
        foreach (var body in _bodyParts)
        {
            if (body == null) continue;

            int historyIndex = (index + 1) * _gap;
            int pullbackIndex = Mathf.FloorToInt(historyIndex + (historyIndex + _tailPullback));

            if (pullbackIndex < _positionsHistory.Count)
            {
                Vector3 targetPoint = _positionsHistory[pullbackIndex];
                body.transform.position = targetPoint;

                if (pullbackIndex + 1 < _positionsHistory.Count)
                {
                    Vector3 nextPoint = _positionsHistory[pullbackIndex + 1];
                    Vector3 direction = nextPoint - targetPoint;
                    if (direction.magnitude > 0.001f)
                    {
                        body.transform.rotation = Quaternion.LookRotation(direction);
                    }
                }
            }
            else
            {
                Vector3 targetPoint = _positionsHistory[_positionsHistory.Count - 1];
                Vector3 direction = body.transform.position - targetPoint;
                if (direction.magnitude > 0.001f)
                {
                    body.transform.rotation = Quaternion.LookRotation(direction);
                }
            }

            index++;
        }
    }

    private IEnumerator GrowSnakeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_growInterval);
            GrowSnake();
        }
    }

    private void GrowSnake()
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation = transform.rotation;

        if (_bodyParts.Count > 0)
        {
            GameObject lastSegment = _bodyParts[_bodyParts.Count - 1];
            spawnPosition = lastSegment.transform.position - lastSegment.transform.forward * 1.5f;
            spawnRotation = lastSegment.transform.rotation;
        }
        else
        {
            spawnPosition = transform.position - transform.forward * 2f;
            spawnRotation = transform.rotation;
        }

        spawnPosition.y = transform.position.y;

        GameObject body = Instantiate(_bodyPrefab.gameObject, spawnPosition, spawnRotation);
        body.transform.parent = _bodyContainer.transform;
        _bodyParts.Add(body);
        _animator.DoScaleUp(body);
    }
}
