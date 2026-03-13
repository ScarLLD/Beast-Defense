using System;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
{
    
    [SerializeField] private Button _miniGameStartButton;

    public event Action Started;

    private void OnEnable()
    {
        _miniGameStartButton.onClick.AddListener(OnStartButtonClick);
    }

    private void OnDisable()
    {
        _miniGameStartButton.onClick.RemoveListener(OnStartButtonClick);
    }

    private void OnStartButtonClick()
    {
        Started?.Invoke();
    }
}
