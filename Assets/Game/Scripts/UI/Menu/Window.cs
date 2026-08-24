using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI.Menu
{
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] protected List<GameObject> menu = new();

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

        protected void OnGameStarted()
        {
            DisableMenu();
        }

        protected void OnGameLeaved()
        {
            EnableMenu();
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

        protected void OnLeaderBoardOpened()
        {
            DisableMenu();
        }

        protected void OnLeaderBoardClosed()
        {
            EnableMenu();
        }

        protected void OnMiniGameStarted()
        {
            DisableMenu();
        }

        protected void OnMiniGameLeaved()
        {
            EnableMenu();
        }

        protected void OnGameTransited()
        {
            DisableMenu();
        }

        protected void OnShopOpened()
        {
            DisableMenu();
        }

        protected void OnShopClosed()
        {
            EnableMenu();
        }
    }
}