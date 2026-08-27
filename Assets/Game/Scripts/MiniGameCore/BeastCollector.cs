using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.MiniGameCore
{
    public class BeastCollector : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private MiniGame _miniGame;
        [SerializeField] private MGBeastSpawner _beastSpawner;
        [SerializeField] private MGSnake _snake;

        private int _beastCollectedCount;
        private int _maxBeastCollectedCount = 10;

        public bool IsBeastsFull => _beastCollectedCount == _maxBeastCollectedCount;

        private void Awake()
        {
            _beastCollectedCount = 0;
        }

        public void IncreaseBeastCount()
        {
            _beastCollectedCount += 1;
            DisplayCount();

            if (_beastCollectedCount != _maxBeastCollectedCount) return;

            _snake.Die();
            _miniGame.VictoryGame();
        }

        public void ResetSettings()
        {
            _beastCollectedCount = 0;
            DisplayCount();
        }

        public void SetNewMaxBeastCount(int count)
        {
            _maxBeastCollectedCount = count;
            DisplayCount();
        }

        private void DisplayCount()
        {
            _text.text = $"{_beastCollectedCount}/{_maxBeastCollectedCount}";
        }
    }
}