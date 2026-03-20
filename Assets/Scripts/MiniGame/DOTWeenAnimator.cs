using UnityEngine;
using DG.Tweening;

public class DOTWeenAnimator : MonoBehaviour
{
    private float _scaleDuration = 0.5f;

    public float GetDuration => _scaleDuration;

    public void DoScaleUp(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack);
    }

    public void DoScaleDown(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.DOScale(Vector3.zero, _scaleDuration).SetEase(Ease.InCubic);
    }
}
