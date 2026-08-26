using System;
using UnityEngine;

namespace Game.Scripts.MiniGameCore
{
    public class MiniGame : MonoBehaviour
    {
        [SerializeField] private BeastCollector _collector;
        [SerializeField] private MGSnake _snake;

        public event Action Started;
        public event Action Defeated;
        public event Action Won;

        public bool IsActive { get; private set; }

        private void Awake()
        {
            IsActive = false;
        }

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
            IsActive = false;
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
            Won?.Invoke();
        }

        private void DefeatGame()
        {
            IsActive = false;
            Defeated?.Invoke();
        }
    }
}