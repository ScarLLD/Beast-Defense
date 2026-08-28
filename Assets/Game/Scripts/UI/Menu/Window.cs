using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI.Menu
{
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] protected List<GameObject> menu = new();

        public static event Action ButtonClicked;

        public bool IsActive { get; private set; }

        private void SwitchVisible(bool isActive)
        {
            foreach (var target in menu)
            {
                target.SetActive(isActive);
            }
        }
        
        protected static void CallClickEvent()
        {
            ButtonClicked?.Invoke();
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
        
        protected void OnGameTransited()
        {
            DisableMenu();
        }
    }
}