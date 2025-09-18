using System;
using System.Collections;
using UnityEngine;

public class Transition : MonoBehaviour
{
    [SerializeField] private float _transitionDuration;
    [SerializeField] private float _holdTime;
    [SerializeField] private Transform _sprite;
    [SerializeField] private Canvas _canvas;

    private AnimationCurve _animationCurve;
    private Vector3 _spriteLeftPosition;
    private Vector3 _spriteRightPosition;
    private Coroutine _moveCoroutine;
    private WaitForSeconds _sleep;

    public event Action Transited;
    public event Action BackTransited;

    private void Awake()
    {
        SetSpriteOptions();
        _sleep = new WaitForSeconds(_holdTime);
    }

    private void SetSpriteOptions()
    {
        _spriteLeftPosition =
            new(_canvas.transform.position.x - Camera.main.pixelWidth * 2,
            _canvas.transform.position.y,
            _canvas.transform.position.z);

        _spriteRightPosition =
            new(_canvas.transform.position.x + Camera.main.pixelWidth * 2,
            _canvas.transform.position.y,
            _canvas.transform.position.z);

        _sprite.transform.position = _spriteLeftPosition;

        _sprite.gameObject.SetActive(false);
    }

    public IEnumerator StartTransition()
    {
        _sprite.gameObject.SetActive(true);

        yield return _moveCoroutine ??= StartCoroutine(StartMove(AnimationCurve.EaseInOut(1, 1, 0, 0), _spriteLeftPosition, _canvas.transform.position));
        yield return _sleep;
    }

    public IEnumerator StartBackTransition()
    {
        _sprite.gameObject.SetActive(true);

        yield return _moveCoroutine ??= StartCoroutine(StartMove(AnimationCurve.EaseInOut(1, 1, 0, 0), _spriteRightPosition, _canvas.transform.position));
        yield return _sleep;
        BackTransited?.Invoke();
    }

    public IEnumerator ContinueBackTransition()
    {
        yield return _sleep;
        yield return _moveCoroutine ??= StartCoroutine(StartMove(AnimationCurve.EaseInOut(0, 0, 1, 1), _canvas.transform.position, _spriteLeftPosition));

        _sprite.gameObject.SetActive(false);
    }

    public IEnumerator ContinueTransition()
    {
        yield return _sleep;
        yield return _moveCoroutine ??= StartCoroutine(StartMove(AnimationCurve.EaseInOut(0, 0, 1, 1), _canvas.transform.position, _spriteRightPosition));

        _sprite.gameObject.SetActive(false);
    }

    private IEnumerator StartMove(AnimationCurve animationCurve, Vector3 startPosition, Vector3 targetPosition)
    {
        float timer = 0;
        _animationCurve = animationCurve;

        while (timer < _transitionDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / _transitionDuration);
            float smooth = _animationCurve.Evaluate(t);

            _sprite.transform.position = Vector3.Lerp(startPosition, targetPosition, smooth);

            yield return null;
        }

        _moveCoroutine = null;
    }
}
