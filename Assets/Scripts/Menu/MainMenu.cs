using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private List<GameObject> menu = new List<GameObject>();

    private void OnEnable()
    {
        _game.Started += OffMenu;
    }

    private void OnDisable()
    {
        _game.Started -= OffMenu;
    }

    private void Awake()
    {
        SwitchVisible(true);
    }

    public void SwitchVisible(bool isActive)
    {
        foreach (GameObject gameObject in menu)
        {
            gameObject.SetActive(isActive);
        }
    }

    public void OnGameStartButton()
    {
        _game.StartGame();
    }

    private void OffMenu()
    {
        SwitchVisible(false);
    }
}
