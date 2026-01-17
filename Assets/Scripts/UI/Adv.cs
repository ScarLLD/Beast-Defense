using System;
using UnityEngine;
using YG;

public class Adv : MonoBehaviour
{
    [SerializeField] private Transition _transition; 
    [SerializeField] private string _skipLevelID = "Skip";
    [SerializeField] private string _doubleRewardID = "Double";

    public event Action Regenerated;
    public event Action Doubled;

    public void RegenerateLevelAdvShow()
    {
        if (_transition.IsTransiting == false)
        {
            YG2.RewardedAdvShow(_skipLevelID, () =>
            {
                Regenerated?.Invoke();
            });
        }
    }

    public void DoubleRewardAdvShow()
    {
        if (_transition.IsTransiting == false)
        {
            YG2.RewardedAdvShow(_doubleRewardID, () =>
            {
                Doubled?.Invoke();
            });
        }
    }
}
