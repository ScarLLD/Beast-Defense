using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private List<GameObject> menu = new List<GameObject>();

    private void OnEnable()
    {
        _game.Over += OnGameOver;
    }

    private void OnDisable()
    {
        _game.Over -= OnGameOver;
    }

    private void Awake()
    {
        SwitchVisible(false);
    }

    public void SwitchVisible(bool isActive)
    {
        foreach (GameObject gameObject in menu)
        {
            gameObject.SetActive(isActive);
        }
    }

    private void OnGameOver()
    {
        SwitchVisible(true);
    }
}
