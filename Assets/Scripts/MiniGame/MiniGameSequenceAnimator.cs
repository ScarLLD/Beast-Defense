using DG.Tweening;
using System;
using UnityEngine;

public class MiniGameSequenceAnimator : MonoBehaviour
{
    [SerializeField] private GameObject _walls;
    [SerializeField] private GameObject _platform;
    [SerializeField] private GameObject _snake;
    [SerializeField] private float _duration;

    public event Action Closed;

    public void StartAnimation()
    {
        if (_platform == null || _snake == null)
        {
            Debug.LogError("Один или несколько объектов не назначены в инспекторе!");
            return;
        }

        _walls.SetActive(false);

        _platform.transform.localScale = Vector3.zero;
        _snake.transform.localScale = Vector3.zero;

        _platform.transform.DOScale(Vector3.one, _duration)
            .SetEase(Ease.OutBack);

        _snake.transform.DOScale(Vector3.one, _duration)
            .SetDelay(0.2f)
            .SetEase(Ease.OutBack);

        Invoke("ActivateWalls", _duration);
    }

    private void ActivateWalls()
    {
        _walls.SetActive(true);
    }

    public void CloseAnimation()
    {
        if (_platform == null || _snake == null)
        {
            Debug.LogError("Один или несколько объектов не назначены в инспекторе!");
            return;
        }

        _walls.SetActive(false);

        _platform.transform.DOScale(Vector3.zero, _duration).SetEase(Ease.OutBack);
        _snake.transform.DOScale(Vector3.zero, _duration).SetDelay(0.2f).SetEase(Ease.OutBack);

        Closed?.Invoke();
    }
}
