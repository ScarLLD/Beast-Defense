using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerCubeAnimator : BaseAnimator
    {
        private static readonly int IsAvailable = Animator.StringToHash("isAvailable");

        protected override int IsWalkHash => Animator.StringToHash("isWalk");

        public void SetAvailableTrigger()
        {
            _animator.SetTrigger(IsAvailable);
        }
    }
}