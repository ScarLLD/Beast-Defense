using UnityEngine;
using System;

namespace Game.Scripts.MiniGameCore
{
    public class MiniGame : MonoBehaviour
    {
        [SerializeField] private BeastCollector _collector;
        [SerializeField] private MGSnake _snake;

        public bool IsActive { get; private set; }

        public event Action Started;
        public event Action Defeated;
        public event Action Won;
        
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