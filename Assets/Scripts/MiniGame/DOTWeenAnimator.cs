using UnityEngine;
using DG.Tweening;

public class DOTWeenAnimator : MonoBehaviour
{
    public void DoScaleUp(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void DoScaleDown(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuart);
    }
}
