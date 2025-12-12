using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHeart : MonoBehaviour
{
    [SerializeField] private int _maxCount;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Image _heartImage;
    [SerializeField] private AnimationCurve _deathAnimationCurve;
    [SerializeField] private float _changeDuration;
    [SerializeField] private float _changeDelay;

    private int _currentCount;
    private Animation _animation;

    public bool IsPossibleDecrease => _currentCount > 0;

    private void Awake()
    {
        _animation = GetComponent<Animation>();

        _currentCount = _maxCount;
        _countText.text = _currentCount.ToString();
        _heartImage.fillAmount = _currentCount;
    }

    public IEnumerator ChangeRoutine()
    {
        if (IsPossibleDecrease)
        {
            _currentCount--;

            float timer = 0f;
            float startFillAmount = _heartImage.fillAmount;
            float targetFillAmount = (float)_currentCount / (float)_maxCount;

            while (timer < _changeDuration)
            {
                timer += Time.deltaTime;
                float t = timer / _changeDuration;
                _heartImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, _deathAnimationCurve.Evaluate(t));
                yield return null;
            }

            _countText.text = _currentCount.ToString();
            yield return new WaitForSeconds(_changeDelay);
        }
    }

    public void PlayShakeAnimation()
    {
        if (_animation.isPlaying == false)
            _animation.Play();
    }
}
