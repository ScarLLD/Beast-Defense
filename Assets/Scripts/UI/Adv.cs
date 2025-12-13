using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Adv : MonoBehaviour
{
    [SerializeField] private Button _advSkipLevelButton;
    [SerializeField] private Button _advDoubleRewardButton;
    [SerializeField] private string _skipLevelID = "Skip";
    [SerializeField] private string _doubleRewardID = "Double";

    public event Action Skipped;
    public event Action Doubled;

    private void OnEnable()
    {
        _advSkipLevelButton.onClick.AddListener(SkipLevelAdvShow);
        _advDoubleRewardButton.onClick.AddListener(DoubleRewardAdvShow);
    }

    private void OnDisable()
    {
        _advSkipLevelButton.onClick.RemoveListener(SkipLevelAdvShow);
        _advDoubleRewardButton.onClick.RemoveListener(DoubleRewardAdvShow);
    }

    private void SkipLevelAdvShow()
    {
        YG2.RewardedAdvShow(_skipLevelID, () =>
        {
            Skipped?.Invoke();
        });
    }

    private void DoubleRewardAdvShow()
    {
        YG2.RewardedAdvShow(_doubleRewardID, () =>
        {
            Doubled?.Invoke();
        });
    }
}
