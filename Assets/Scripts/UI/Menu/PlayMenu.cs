using UnityEngine;

public class PlayMenu : Window
{
    [SerializeField] private Game _game;

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
