using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Adv : MonoBehaviour
{
    [SerializeField] private Transition _transition;
    [SerializeField] private Button _advSkipLevelButton;
    [SerializeField] private Button _advDoubleRewardButton;
    [SerializeField] private string _skipLevelID = "Skip";
    [SerializeField] private string _doubleRewardID = "Double";

    public event Action Regenerated;
    public event Action Doubled;

    private void OnEnable()
    {
        _advSkipLevelButton.onClick.AddListener(RegenerateLevelAdvShow);
        _advDoubleRewardButton.onClick.AddListener(DoubleRewardAdvShow);
    }

    private void OnDisable()
    {
        _advSkipLevelButton.onClick.RemoveListener(RegenerateLevelAdvShow);
        _advDoubleRewardButton.onClick.RemoveListener(DoubleRewardAdvShow);
    }

    private void RegenerateLevelAdvShow()
    {
        if (_transition.IsTransiting == false)
        {
            YG2.RewardedAdvShow(_skipLevelID, () =>
            {
                Regenerated?.Invoke();
            });
        }
    }

    private void DoubleRewardAdvShow()
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
