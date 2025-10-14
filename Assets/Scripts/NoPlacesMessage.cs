using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoPlacesMessage : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRectTransform;
    [SerializeField] private RectTransform _messageRectTransform;
    [SerializeField] private AnimationCurve _transitionAnimationCurve;
    [SerializeField] private float _transitionDuration;
    [SerializeField] private float _distanceOutsideScreen = 200f;

    private Vector3 _centerPosition;
    private Vector3 _externalPosition;

    private void Awake()
    {
        _centerPosition = Vector3.zero;

        _externalPosition = Vector3.zero;
        _externalPosition.y = -_canvasRectTransform.rect.height - _distanceOutsideScreen;
    }

    private void Start()
    {
        GoToCenter();
    }

    private void GoToCenter()
    {
        StartCoroutine(AnimatePosition(_externalPosition, _centerPosition));
    }

    private void GoOutside()
    {
        StartCoroutine(AnimatePosition(_centerPosition, _externalPosition));
    }

    private IEnumerator AnimatePosition(Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < _transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _transitionDuration;
            float curveValue = _transitionAnimationCurve.Evaluate(progress);

            _messageRectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null;
        }

        _messageRectTransform.anchoredPosition = targetPosition;
    }
}