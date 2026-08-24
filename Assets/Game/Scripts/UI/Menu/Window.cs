using System;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] protected List<GameObject> menu = new ();

        public static event Action ButtonClicked;
        public event Action Opened;

        public bool IsActive { get; private set; }

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
            Opened?.Invoke();
        }

        protected void DisableMenu()
        {
            SwitchVisible(false);
            IsActive = false;
        }

        protected void CallClickEvent()
        {
            ButtonClicked?.Invoke();
        }
    }
}