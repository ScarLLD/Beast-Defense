using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private int _victoryRewardCount = 5;

    private int _money;

    public int Money => _money;

    public event Action CountChanged;

    private void OnEnable()
    {
        _game.Completed += OnGameCompleted;
    }

    private void OnDisable()
    {
        _game.Completed -= OnGameCompleted;
    }


    private void Start()
    {
        _money = 2;
        CountChanged?.Invoke();
    }

    public void IncreaseMoney(int count)
    {
        _money += count;

        CountChanged?.Invoke();
    }

    public void DecreaseMoney(int count)
    {
        if (_money >= count)
            _money -= count;

        if (_money < 0)
            _money = 0;

        CountChanged?.Invoke();
    }

    private void OnGameCompleted()
    {
        IncreaseMoney(_victoryRewardCount);
    }
}
