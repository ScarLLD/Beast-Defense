using UnityEngine;

namespace Game.Scripts.UI.Menu
{
    public class PlayMenu : Window
    {
        [SerializeField] private MapGenerator.Game _game;

        private void Awake()
        {
            DisableMenu();
        }

        private void OnEnable()
        {
            _game.Started += OnGameStarted;
            _game.Leaved += OnGameLeaved;
        }

        private void OnDisable()
        {
            _game.Started -= OnGameStarted;
            _game.Leaved -= OnGameLeaved;
        }

        private void OnGameStarted() => EnableMenu();

        private void OnGameLeaved() => DisableMenu();
    }
}