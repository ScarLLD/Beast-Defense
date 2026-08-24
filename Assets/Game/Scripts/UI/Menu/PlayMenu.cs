using UnityEngine;

namespace Game.Scripts.UI.Menu
{
    public class PlayMenu : Window
    {
        [SerializeField] private MapGenerator.Game _game;

        private void OnEnable()
        {
            _game.Started += EnableMenu;
            _game.Leaved += DisableMenu;
        }

        private void OnDisable()
        {
            _game.Started -= EnableMenu;
            _game.Leaved += DisableMenu;
        }

        private void Awake()
        {
            DisableMenu();
        }
    }
}