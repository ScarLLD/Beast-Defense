using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : Window
{
    [SerializeField] private Transition _transition;
    [SerializeField] private Material _shopMaterial;

    [SerializeField] private Button _exitButton;

    public event Action Opened;
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
        if (_transition.IsTransiting == false)
            StartCoroutine(OpenShop());
    }

    private IEnumerator OpenShop()
    {
        _transition.SetText("Загрузка");
        yield return StartCoroutine(_transition.StartTransitionRoutine(_shopMaterial.color));
        EnableMenu();
        Opened?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransitionRoutine());
    }

    private void OnExitButtonClick()
    {
        if (_transition.IsTransiting == false)
            StartCoroutine(CloseShopRoutine());
    }

    private IEnumerator CloseShopRoutine()
    {
        if (_transition.IsTransiting == false)
        {
            _transition.SetText("Выход");
            yield return StartCoroutine(_transition.StartBackTransitionRoutine(_shopMaterial.color));
            Closed?.Invoke();
            DisableMenu();
            yield return StartCoroutine(_transition.ContinueBackTransitionRoutine());
        }
    }
}
