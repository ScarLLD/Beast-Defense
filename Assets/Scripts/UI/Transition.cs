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

    private Coroutine _transitionCoroutine;
    private Coroutine _moveCoroutine;

    public bool IsTransiting { get; private set; }

    public event Action BackTransited;

    private void Awake()
    {
        IsTransiting = false;
        SetSpriteOptions();
        _sleep = new WaitForSeconds(_holdTime);
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public IEnumerator StartTransitionRoutine(Color color)
    {
        yield return _transitionCoroutine ??= StartCoroutine(LeftToCenterTransitionRoutine(color));
    }

    public IEnumerator StartBackTransitionRoutine(Color color)
    {
        yield return _transitionCoroutine ??= StartCoroutine(RightToCenterTransitionRoutine(color));
    }

    public IEnumerator ContinueTransitionRoutine()
    {
        yield return _transitionCoroutine ??= StartCoroutine(CenterToRightTransitionRoutine());
    }

    public IEnumerator ContinueBackTransitionRoutine()
    {
        yield return _transitionCoroutine ??= StartCoroutine(CenterToLeftTransitionRoutine());
    }

    private IEnumerator LeftToCenterTransitionRoutine(Color color)
    {
        IsTransiting = true;

        _spriteImage.color = color;
        _spriteImage.enabled = true;
        _text.enabled = true;

        yield return _moveCoroutine ??= StartCoroutine(TransitionRoutine(AnimationCurve.EaseInOut(1, 1, 0, 0), _spriteLeftPosition, _canvas.transform.position));
        yield return _sleep;

        _transitionCoroutine = null;
        IsTransiting = false;
    }

    private IEnumerator RightToCenterTransitionRoutine(Color color)
    {
        IsTransiting = true;

        _spriteImage.color = color;
        _spriteImage.enabled = true;
        _text.enabled = true;

        yield return _moveCoroutine ??= StartCoroutine(TransitionRoutine(AnimationCurve.EaseInOut(1, 1, 0, 0), _spriteRightPosition, _canvas.transform.position));
        yield return _sleep;

        _transitionCoroutine = null;
        BackTransited?.Invoke();
        IsTransiting = false;
    }

    private IEnumerator CenterToLeftTransitionRoutine()
    {
        IsTransiting = true;

        if (_spriteImage.enabled == true)
        {
            yield return _sleep;
            yield return _moveCoroutine ??= StartCoroutine(TransitionRoutine(AnimationCurve.EaseInOut(0, 0, 1, 1), _canvas.transform.position, _spriteLeftPosition));

            _spriteImage.enabled = false;
        }
        else
        {
            _text.enabled = false;
        }

        _transitionCoroutine = null;
        IsTransiting = false;
    }

    private IEnumerator CenterToRightTransitionRoutine()
    {
        IsTransiting = true;

        if (_spriteImage.enabled == true)
        {
            yield return _sleep;
            yield return _moveCoroutine ??= StartCoroutine(TransitionRoutine(AnimationCurve.EaseInOut(0, 0, 1, 1), _canvas.transform.position, _spriteRightPosition));

            _spriteImage.enabled = false;
        }
        else
        {
            _text.enabled = false;
        }

        _transitionCoroutine = null;
        IsTransiting = false;
    }

    private IEnumerator TransitionRoutine(AnimationCurve animationCurve, Vector3 startPosition, Vector3 targetPosition)
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
        _text.enabled = false;
    }
}
