using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.BeastCore
{
    public class BeastAnimator : BaseAnimator
    {
        protected override int IsWalkHash => Animator.StringToHash("isWalk");
    }
}