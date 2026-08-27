using UnityEngine;

namespace Game.Scripts.Core
{
    [RequireComponent(typeof(Animator))]
    public abstract class BaseAnimator : MonoBehaviour
    {
        protected Animator _animator;
        protected abstract int IsWalkHash { get; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void ResetSettings()
        {
            SetWalkBool(false);
            _animator.Rebind();
            _animator.Update(0f);
        }

        public void EnableAnimator(bool value)
        {
            _animator.enabled = value;
        }

        public void SetWalkBool(bool value)
        {
            _animator.SetBool(IsWalkHash, value);
        }
    }
}