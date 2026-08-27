using UnityEngine;

namespace Game.Scripts.Core
{
    [RequireComponent(typeof(Animator))]
    public abstract class BaseAnimator : MonoBehaviour
    {
        protected Animator animator;
        protected abstract int IsWalkHash { get; }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void ResetSettings()
        {
            SetWalkBool(false);
            animator.Rebind();
            animator.Update(0f);
        }

        public void EnableAnimator(bool value)
        {
            animator.enabled = value;
        }

        public void SetWalkBool(bool value)
        {
            animator.SetBool(IsWalkHash, value);
        }
    }
}