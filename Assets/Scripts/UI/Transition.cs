using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private float _transitionDuration;
    [SerializeField] private float _holdTime;
    [SerializeField] private Transform _sprite;
    [SerializeField] private Image _spriteImage;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Canvas _canvas;

    private Vector3 _spriteLeftPosition;
    private Vector3 _spriteRightPosition;
    private AnimationCurve _animationCurve;
    private WaitForSeconds _sleep;
    private Coroutine _moveCoroutine;

    public event Action BackTransited;

    private void Awake()
    {
        SetSpriteOptions();
        _sleep = new WaitForSeconds(_holdTime);
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public IEnumerator StartTransition(Color color)
    {
        _spriteImage.color = color;
        _spriteImage.enabled = true;
        _text.enabled = true;

        yield return _moveCoroutine ??= StartCoroutine(StartMove(AnimationCurve.EaseInOut(1, 1, 0, 0), _spriteLeftPosition, _canvas.transform.position));
        yield return _sleep;
    }

    public IEnumerator StartBackTransition(Color color)
    {
        _spriteImage.color = color;
        _spriteImage.enabled = true;
        _text.enabled = true;

        yield return _moveCoroutine ??= StartCoroutine(StartMove(AnimationCurve.EaseInOut(1, 1, 0, 0), _spriteRightPosition, _canvas.transform.position));
        yield return _sleep;
        BackTransited?.Invoke();
    }

    public IEnumerator ContinueBackTransition()
    {
        if (_spriteImage.enabled == true)
        {
            yield return _sleep;
            yield return _moveCoroutine ??= StartCoroutine(StartMove(AnimationCurve.EaseInOut(0, 0, 1, 1), _canvas.transform.position, _spriteLeftPosition));

            _spriteImage.enabled = false;
        }
        else
        {
            _text.enabled = false;
        }
    }

    public IEnumerator ContinueTransition()
    {
        if (_spriteImage.enabled == true)
        {
            yield return _sleep;
            yield return _moveCoroutine ??= StartCoroutine(StartMove(AnimationCurve.EaseInOut(0, 0, 1, 1), _canvas.transform.position, _spriteRightPosition));

            _spriteImage.enabled = false;
        }
        else
        {
            _text.enabled = false;
        }
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

        _spriteImage.enabled = false;
    }
}
