using Game.Scripts.UI.Menu;
using Game.Scripts.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.Scripts.Shop
{
    public class ShopMenu : Window
    {
        [SerializeField] private Transition _transition;
        [SerializeField] private float _transitionDuration = 0.4f;
        [SerializeField] private Material _shopMaterial;
        [SerializeField] private Button _exitButton;

        public new event Action Opened;
        public event Action Closed;

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
            if (!_transition.IsTransiting)
            {
                _transition.StartBackTransition(_shopMaterial.color, _transitionDuration);
                yield return new WaitUntil(() => !_transition.IsTransiting);

                Closed?.Invoke();
                DisableMenu();

                _transition.ContinueBackTransition(_transitionDuration);
                yield return new WaitUntil(() => !_transition.IsTransiting);
            }
        }
    }
}
