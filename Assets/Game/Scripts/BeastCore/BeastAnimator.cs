using UnityEngine;

namespace Game.Scripts.BeastCore
{
    [RequireComponent(typeof(Animator))]
    public class BeastAnimator : MonoBehaviour
    {
        private static readonly int IsWalk = Animator.StringToHash("isWalk");
        private Animator _animator;

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
            _animator.SetBool(IsWalk, value);
        }
    }
}