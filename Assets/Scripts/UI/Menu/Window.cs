using System.Collections.Generic;
using UnityEngine;

public abstract class Window : MonoBehaviour
{
    [SerializeField] protected Game _game;
    [SerializeField] protected List<GameObject> menu = new();

    protected void SwitchVisible(bool isActive)
    {
        foreach (GameObject gameObject in menu)
        {
            gameObject.SetActive(isActive);
        }
    }

}
