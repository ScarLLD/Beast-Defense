using Game.Scripts.MapGenerator;
using UnityEngine;
using YG;

namespace Game.Scripts.UI
{
    public class LevelHolder : MonoBehaviour
    {
        [SerializeField] private MapGenerator.Game _game;

        private void OnEnable()
        {
            _game.Completed += IncreaseLevel;
        }

        private void OnDisable()
        {
            _game.Completed -= IncreaseLevel;
        }

        public void IncreaseLevel()
        {
            YG2.saves.LevelNumber++;
            YG2.SaveProgress();
        }
    }
}