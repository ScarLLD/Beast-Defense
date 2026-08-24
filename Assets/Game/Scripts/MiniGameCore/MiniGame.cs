using System;
using UnityEngine;

namespace Game.Scripts.MiniGameCore
{
    public class MiniGame : MonoBehaviour
    {
        [SerializeField] private BeastCollector _collector;
        [SerializeField] private MGSnake _snake;

        public event Action Started;
        public event Action Defeat;
        public event Action Victory;

        public bool IsActive { get; private set; } = false;

        private void OnEnable()
        {
            _snake.Died += DefeatGame;
        }

        private void OnDisable()
        {
            _snake.Died -= DefeatGame;
        }

        public void ResetSettings()
        {
            _collector.ResetSettings();
        }

        public void StartGame()
        {
            IsActive = true;
            Started?.Invoke();
        }

        public void VictoryGame()
        {
            IsActive = false;
            Victory?.Invoke();
        }

        public void DefeatGame()
        {
            IsActive = false;
            Defeat?.Invoke();
        }
    }
}