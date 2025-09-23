using UnityEngine;

public class PauseMenu : Window
{
    private void OnEnable()
    {
        _game.Paused += OnGamePaused;
    }

    private void OnGamePaused()
    {
        SwitchVisible(true);
    }
}
