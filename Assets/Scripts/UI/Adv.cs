using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Adv : MonoBehaviour
{
    [SerializeField] private Button _advButton;
    [SerializeField] private AdvAction _advAction;
    [SerializeField] private string _text;

    public event Action Watched;

    private void OnEnable()
    {
        _advButton.onClick.AddListener(SkipLevelAdvShow);
    }

    private void OnDisable()
    {
        _advButton.onClick.RemoveListener(SkipLevelAdvShow);
    }

    public void SkipLevelAdvShow()
    {
        YG2.RewardedAdvShow(_text, () =>
        {
            Watched?.Invoke();
        });
    }
}
