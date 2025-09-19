using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private List<GameObject> menu = new List<GameObject>();

    private void OnEnable()
    {
        _game.Ended += OnGameEnded;
    }

    private void OnDisable()
    {
        _game.Ended -= OnGameEnded;
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

    private void OnGameEnded()
    {
        SwitchVisible(true);
    }
}
