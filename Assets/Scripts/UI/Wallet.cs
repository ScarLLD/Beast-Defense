using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private int _victoryRewardCount = 5;

    private const string MONEY_KEY = "PlayerMoney";

    private int _money;

    public int GetMoneyCount() => _money;
    public int GetRewardMoneyCount() => _victoryRewardCount;

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
        LoadMoney();
        CountChanged?.Invoke();
    }

    public bool CanAfford(int amount)
    {
        return _money >= amount;
    }

    public void IncreaseMoney(int count)
    {
        if (count < 0)
            return;

        _money += count;
        SaveMoney();
        CountChanged?.Invoke();
    }

    public void DecreaseMoney(int count)
    {
        if (count < 0)
            return;

        if (_money >= count)
        {
            _money -= count;
            SaveMoney();
            CountChanged?.Invoke();
        }
    }

    private void OnGameCompleted()
    {
        IncreaseMoney(_victoryRewardCount);
    }

    private void LoadMoney()
    {
        _money = PlayerPrefs.GetInt(MONEY_KEY, 5);
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt(MONEY_KEY, _money);
        PlayerPrefs.Save();
    }
}
