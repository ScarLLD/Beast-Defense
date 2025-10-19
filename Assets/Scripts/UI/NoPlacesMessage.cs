using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoPlacesMessage : MonoBehaviour
{
    [SerializeField] private RectTransform _messageRectTransform;
    [SerializeField] private RawImage _messageImage;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private AnimationCurve _transitionAnimationCurve;
    [SerializeField] private AnimationCurve _reverseTransitionAnimationCurve;
    [SerializeField] private float _transitionDuration;
    [SerializeField] private float _sleepTime;
    [SerializeField] private float _distanceYOffset = 200f;

    private Coroutine _transitionCoroutine;
    private WaitForSeconds _sleep;
    private Vector3 _upperPosition;
    private Vector3 _centarPosition;
    private Vector3 _lowerPosition;

    private void Awake()
    {
        _sleep = new WaitForSeconds(_sleepTime);

        _upperPosition = Vector3.zero;
        _upperPosition.y += _distanceYOffset;

        _centarPosition = Vector3.zero;

        _lowerPosition = Vector3.zero;
        _lowerPosition.y -= _distanceYOffset;

        _messageRectTransform.gameObject.SetActive(false);
    }

    public void DisplayMessage()
    {
        _transitionCoroutine ??= StartCoroutine(DisplayMessageRoutine());
    }

    private IEnumerator DisplayMessageRoutine()
    {
        _messageRectTransform.gameObject.SetActive(true);
        yield return StartCoroutine(AnimatePosition(_transitionAnimationCurve, _lowerPosition, _centarPosition, 0, 1));
        yield return _sleep;
        yield return StartCoroutine(AnimatePosition(_reverseTransitionAnimationCurve, _centarPosition, _upperPosition, 1, 0));
        _messageRectTransform.gameObject.SetActive(false);

        _transitionCoroutine = null;
    }

    private IEnumerator AnimatePosition(AnimationCurve curve, Vector3 startPosition, Vector3 targetPosition, float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < _transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _transitionDuration;
            float curveValue = curve.Evaluate(progress);

            _messageRectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);

            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);

            Color imageColor = _messageImage.color;
            imageColor.a = currentAlpha;
            _messageImage.color = imageColor;

            Color textColor = _messageText.color;
            textColor.a = currentAlpha;
            _messageText.color = textColor;

            yield return null;
        }

        _messageRectTransform.anchoredPosition = targetPosition;
    }
}