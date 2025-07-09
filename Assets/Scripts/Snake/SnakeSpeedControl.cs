using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SnakeHead))]
public class SnakeSpeedControl : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 0f;          // ����������� �������� (0 ��� ������ ���������)
    [SerializeField] private int _thresholdSlowdown = 5;    // ���������, �� ������� �������� �����������
    [SerializeField] private int _minRoadCountToEnd = 3;

    private SnakeHead _snakeHead;
    private float _initialSpeed;
    private bool _isSlowingDown = false;
    private float _slowdownStartDistance;

    private void Awake()
    {
        _snakeHead = GetComponent<SnakeHead>();
        _initialSpeed = _snakeHead.Speed;
    }

    private IEnumerator ControlSpeed()
    {
        while (true)
        {
            if (_snakeHead.CompareRoadPoint(_minRoadCountToEnd, _thresholdSlowdown, out float distanceToEnd))
            {
                if (!_isSlowingDown)
                {
                    _isSlowingDown = true;
                    _slowdownStartDistance = distanceToEnd;
                }

                // ����������� ���������� ���������� (�� 1 �� 0)
                float progress = 1f - (distanceToEnd / _slowdownStartDistance);

                // ������ ��������� �������� (����� �������� �� SmoothStep ��� ����� ������� ����������)
                float newSpeed = Mathf.Lerp(_initialSpeed, _minSpeed, progress);
                _snakeHead.ChangeSpeed(newSpeed);

                // ���� �������� ����� ��� ����� ������������
                if (distanceToEnd <= 0.1f || newSpeed <= _minSpeed + 0.1f)
                {
                    _snakeHead.ChangeSpeed(_minSpeed);
                    _isSlowingDown = false;
                }
            }
            else if (_isSlowingDown)
            {
                _isSlowingDown = false;
                _snakeHead.ChangeSpeed(_initialSpeed);
            }

            yield return null;
        }
    }

    private void Start()
    {
        StartCoroutine(ControlSpeed());
    }
}