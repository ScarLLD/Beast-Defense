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

        if (_canvasGroup != null)
            _canvasGroup.interactable = false;

        _canvasGroup?.DOFade(1f, _animationDuration / 2).SetEase(_fadeEase);

        _previewRectTransform.DOAnchorPos(_targetPreviewPosition, _animationDuration).SetEase(_positionEase);

        _previewRectTransform.DOScale(_targetPreviewScale, _animationDuration).SetEase(_scaleEase).OnComplete(() =>
            {
                if (_canvasGroup != null)
                    _canvasGroup.interactable = true;
            });
    }
}
