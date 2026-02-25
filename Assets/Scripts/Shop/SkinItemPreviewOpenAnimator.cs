using DG.Tweening;
using UnityEngine;

public class SkinItemPreviewOpenAnimator : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _previewRectTransform;
    [SerializeField] private Vector3 _targetPreviewPosition;
    [SerializeField] private Vector3 _targetPreviewScale;

    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Ease _positionEase = Ease.OutBack;
    [SerializeField] private Ease _scaleEase = Ease.OutBounce;
    [SerializeField] private Ease _fadeEase = Ease.Linear;

    private Vector3 _startPreviewScale = Vector3.one * 0.1f;

    private void Awake()
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;
    }

    public void Open(Vector3 startPreviewPosition)
    {
        _previewRectTransform.position = startPreviewPosition;
        _previewRectTransform.localScale = _startPreviewScale;
        _canvasGroup.alpha = 0;

        // Отключаем взаимодействие во время анимации
        if (_canvasGroup != null)
            _canvasGroup.interactable = false;

        // Анимация прозрачности: 0 → 1
        _canvasGroup?.DOFade(1f, _animationDuration / 2)
            .SetEase(_fadeEase);

        // Анимация позиции: начальная → целевая
        _previewRectTransform.DOAnchorPos(_targetPreviewPosition, _animationDuration)
            .SetEase(_positionEase);

        // Анимация масштаба: маленький → целевой
        _previewRectTransform.DOScale(_targetPreviewScale, _animationDuration)
            .SetEase(_scaleEase)
            .OnComplete(() =>
            {
                // После завершения анимации включаем взаимодействие
                if (_canvasGroup != null)
                    _canvasGroup.interactable = true;
            });
    }
}
