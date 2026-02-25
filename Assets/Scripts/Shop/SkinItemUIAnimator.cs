using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkinItemUIAnimator : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private Vector3 _animTargetScale;
    [SerializeField] private float _animDuration = 0.2f;
    [SerializeField] private Ease _easeType = Ease.OutBack;

    private Vector3 _originScale;

    private void Awake()
    {
        _originScale = _rectTransform.localScale;
        _animTargetScale = _originScale * 0.95f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _rectTransform.DOScale(_animTargetScale, _animDuration / 2)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _rectTransform.DOScale(_originScale * 1.05f, _animDuration / 3)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() =>
                    {
                        _rectTransform.DOScale(_originScale, _animDuration / 4)
                            .SetEase(Ease.OutBack);
                    });
            });
    }
}