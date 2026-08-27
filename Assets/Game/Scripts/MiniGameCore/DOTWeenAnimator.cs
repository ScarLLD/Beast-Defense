using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.MiniGameCore
{
    public class DOTWeenAnimator : MonoBehaviour
    {
        private const float SCALE_DURATION = 0.5f;

        public static float GetDuration => SCALE_DURATION;

        public static void DoScaleUp(GameObject target)
        {
            if (!target)
                return;

            target.transform.localScale = Vector3.zero;
            target.transform.DOScale(Vector3.one, SCALE_DURATION).SetEase(Ease.OutBack);
        }

        public static void DoScaleDown(GameObject target)
        {
            if (!target)
                return;

            target.transform.localScale = Vector3.one;
            target.transform.DOScale(Vector3.zero, SCALE_DURATION).SetEase(Ease.InCubic);
        }
    }
}