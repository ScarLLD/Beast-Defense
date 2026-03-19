using DG.Tweening;
using UnityEngine;

public class MiniGameOpenAnimator : MonoBehaviour
{
    [SerializeField] private GameObject _platform;
    [SerializeField] private GameObject _snake;

    public void StartAnimation()
    {
        if (_platform == null || _snake == null)
        {
            Debug.LogError("ќдин или несколько объектов не назначены в инспекторе!");
            return;
        }

        // »значально устанавливаем масштаб 0 дл€ всех объектов
        _platform.transform.localScale = Vector3.zero;
        _snake.transform.localScale = Vector3.zero;

        // јнимаци€ дл€ платформы (начинаетс€ сразу)
        _platform.transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack); // Ёффект "отскока" в конце

        // јнимаци€ дл€ змеи (начинаетс€ через 0.2 секунды после предыдущей)
        _snake.transform.DOScale(Vector3.one, 0.5f)
            .SetDelay(0.2f)
            .SetEase(Ease.OutBack);

    }
}
