using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Adv : MonoBehaviour
{
    [SerializeField] private Transition _transition;
    [SerializeField] private string _skipLevelID = "Skip";
    [SerializeField] private string _doubleRewardID = "Double";

    [SerializeField] private Button _beginButton;

    public event Action Regenerated;
    public event Action Doubled;

    private void OnEnable()
    {
        _beginButton.onClick.AddListener(ShowInterstitialAdv);
    }

    private void OnDisable()
    {
        _beginButton.onClick.RemoveListener(ShowInterstitialAdv);
    }

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

    private void ShowInterstitialAdv()
    {
        YG2.InterstitialAdvShow();
    }
}
