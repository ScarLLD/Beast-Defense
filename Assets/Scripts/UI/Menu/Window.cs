using System.Collections.Generic;
using UnityEngine;

public abstract class Window : MonoBehaviour
{
    [SerializeField] protected List<GameObject> menu = new();

    public bool IsActive { private set; get; }

    protected void SwitchVisible(bool isActive)
    {
        foreach (GameObject gameObject in menu)
        {
            gameObject.SetActive(isActive);
        }
    }

    protected void EnableMenu()
    {
        SwitchVisible(true);
        IsActive = true;
    }

    protected void DisableMenu()
    {
        SwitchVisible(false);
        IsActive = false;
    }
}