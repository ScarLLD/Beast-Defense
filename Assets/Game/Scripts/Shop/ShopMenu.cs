using System;
using System.Collections;
using Game.Scripts.UI;
using Game.Scripts.UI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Shop
{
    public class ShopMenu : Window
    {
        [SerializeField] private Transition _transition;
        [SerializeField] private float _transitionDuration = 0.4f;
        [SerializeField] private Material _shopMaterial;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            DisableMenu();
        }

        private void OnEnable()
        {
            _exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        public new event Action Opened;
        public event Action Closed;

        public void Open()
        {
            if (!_transition.IsTransiting)
                StartCoroutine(OpenShop());
        }

        private IEnumerator OpenShop()
        {
            _transition.StartTransition(_shopMaterial.color, _transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            EnableMenu();
            Opened?.Invoke();

            _transition.ContinueTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);
        }

        private void OnExitButtonClick()
        {
            CallClickEvent();

            if (!_transition.IsTransiting)
                StartCoroutine(CloseShopRoutine());
        }

        private IEnumerator CloseShopRoutine()
        {
            if (_transition.IsTransiting) yield break;

            _transition.StartBackTransition(_shopMaterial.color, _transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            Closed?.Invoke();
            DisableMenu();

            _transition.ContinueBackTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);
        }
    }
}