using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsDisabler : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private GameObject _objectsParent;

    private void OnEnable()
    {
        _game.Leaved += DisableObjects;
    }

    private void OnDisable()
    {
        _game.Leaved -= DisableObjects;
    }

    public void DisableObjects()
    {
        _objectsParent.SetActive(false);
    }

    public void EnableObjects()
    {
        _objectsParent.SetActive(true);
    }
}
