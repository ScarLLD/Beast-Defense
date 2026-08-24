using System;
using UnityEngine;
using YG;

namespace Game.Scripts.UI
{
    public class Wallet : MonoBehaviour
    {
        [SerializeField] private Adv _adv;
        [SerializeField] private MapGenerator.Game _game;
        [SerializeField] private int _victoryRewardCount = 5;

        public event Action CountChanged;

        public int RewardMoneyCount => _victoryRewardCount;

        private void OnEnable()
        {
            _adv.WinRewardDoubled += OnRewardDoubled;
            _game.Completed += OnGameCompleted;
        }

        private void OnDisable()
        {
            _adv.WinRewardDoubled -= OnRewardDoubled;
            _game.Completed -= OnGameCompleted;
        }

        private void Awake()
        {
            CountChanged?.Invoke();
        }

        public bool CanAfford(int count)
        {
            return YG2.saves.Money >= count;
        }

        public void IncreaseMoney(int count)
        {
            if (count < 0)
                return;

            YG2.saves.Money += count;
            YG2.SaveProgress();
            CountChanged?.Invoke();
        }

        public void DecreaseMoney(int count)
        {
            if (count < 0)
                return;

            if (YG2.saves.Money >= count)
            {
                YG2.saves.Money -= count;
                YG2.SaveProgress();
                CountChanged?.Invoke();
            }
        }

        private void OnGameCompleted()
        {
            IncreaseMoney(_victoryRewardCount);
        }

        private void OnRewardDoubled()
        {
            IncreaseMoney(_victoryRewardCount);
        }
    }
}