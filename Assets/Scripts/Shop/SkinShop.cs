using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class SkinShop : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SkinData _skinData;

    [Header("UI References")]
    [SerializeField] private Transform _skinsContainer;
    [SerializeField] private SkinItemUI _skinItemPrefab;
    [SerializeField] private Button _closePreviewButton;

    [Header("Preview")]
    [SerializeField] private GameObject _preview;
    [SerializeField] private Image _selectedSkinImage;
    [SerializeField] private TMP_Text _selectedSkinName;
    [SerializeField] private TMP_Text _selectedSkinPrice;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private TMP_Text _actionButtonText;

    private List<SkinItemUI> _skinItems = new List<SkinItemUI>();
    private string _selectedSkinId;
    private string _equippedSkinId;

    private const string COINS_KEY = "PlayerCoins";
    private const string EQUIPPED_SKIN_KEY = "EquippedSkin";
    private const string PURCHASED_SKINS_KEY = "PurchasedSkins";

    private void OnEnable()
    {
        InitializeShop();

        _buyButton.onClick.AddListener(OnBuyButtonClick);
        _selectButton.onClick.AddListener(OnSelectButtonClick);
        _closePreviewButton.onClick.AddListener(OnClosePreviewButtonClick);
    }

    private void OnDisable()
    {
        _buyButton.onClick.RemoveListener(OnBuyButtonClick);
        _selectButton.onClick.RemoveListener(OnSelectButtonClick);
        _closePreviewButton.onClick.RemoveListener(OnSelectButtonClick);
    }

    private void InitializeShop()
    {
        // Очищаем контейнер
        foreach (Transform child in _skinsContainer)
        {
            Destroy(child.gameObject);
        }
        _skinItems.Clear();

        // Создаем элементы скинов
        foreach (var skin in _skinData.skins)
        {
            SkinItemUI skinItem = Instantiate(_skinItemPrefab, _skinsContainer);
            skinItem.Initialize(skin, this);
            _skinItems.Add(skinItem);
        }

        // Загружаем сохраненные данные
        _equippedSkinId = PlayerPrefs.GetString(EQUIPPED_SKIN_KEY, GetDefaultSkinId());
        LoadPurchasedSkins();

        // Выбираем первый скин по умолчанию
        if (_skinData.skins.Count > 0)
        {
            SelectSkin(_skinData.skins[0].skinId);
        }
    }

    public void OpenPreview()
    {
        _preview.SetActive(true);
    }

    public void SelectSkin(string skinId)
    {
        _selectedSkinId = skinId;
        var skin = GetSkinById(skinId);

        if (skin != null)
        {
            // Обновляем превью
            _selectedSkinImage.sprite = skin.icon;
            _selectedSkinName.text = skin.skinName;
            _selectedSkinPrice.text = skin.isDefault ? "Бесплатно" : $"{skin.price} Монет";

            // Проверяем статус скина
            bool isPurchased = IsSkinPurchased(skinId) || skin.isDefault;
            bool isEquipped = skinId == _equippedSkinId;

            // Обновляем кнопки
            _buyButton.gameObject.SetActive(!isPurchased);
            _selectButton.gameObject.SetActive(isPurchased && !isEquipped);

            if (isEquipped)
            {
                _actionButtonText.text = "Выбрано";
            }
            else if (isPurchased)
            {
                _actionButtonText.text = "Выбрать";
            }
            else
            {
                _actionButtonText.text = "Купить";
            }

            // Подсвечиваем выбранный скин
            foreach (var item in _skinItems)
            {
                item.SetSelected(item.SkinId == skinId);
            }
        }
    }

    private void OnBuyButtonClick()
    {
        var skin = GetSkinById(_selectedSkinId);
        if (skin != null && !skin.isDefault)
        {
            BuySkin(_selectedSkinId);
        }
    }

    private void OnSelectButtonClick()
    {
        EquipSkin(_selectedSkinId);
    }

    private void OnClosePreviewButtonClick()
    {
        _preview.SetActive(false);
    }

    private void BuySkin(string skinId)
    {
        var skin = GetSkinById(skinId);
        if (skin == null || skin.isDefault) return;

        int playerCoins = GetPlayerCoins();

        if (playerCoins >= skin.price)
        {
            SpendCoins(skin.price);
            SavePurchasedSkin(skinId);
            SelectSkin(skinId);

            Debug.Log($"Купленый скин: {skin.skinName}");
        }
        else
        {
            Debug.Log("Недостаточно монет!");
        }
    }

    private void EquipSkin(string skinId)
    {
        if (IsSkinPurchased(skinId) || GetSkinById(skinId).isDefault)
        {
            _equippedSkinId = skinId;
            PlayerPrefs.SetString(EQUIPPED_SKIN_KEY, skinId);
            PlayerPrefs.Save();

            SelectSkin(skinId);

            Debug.Log($"Выбраный скин: {GetSkinById(skinId).skinName}");
            ApplySkinToPlayer(skinId);
        }
    }

    private void ApplySkinToPlayer(string skinId)
    {
        var skin = GetSkinById(skinId);
        if (skin != null && skin.model != null)
        {
            Debug.Log($"Применен: {skin.skinName}");
        }
    }

    private SkinData.Skin GetSkinById(string skinId)
    {
        return _skinData.skins.Find(skin => skin.skinId == skinId);
    }

    private string GetDefaultSkinId()
    {
        var defaultSkin = _skinData.skins.Find(skin => skin.isDefault);
        return defaultSkin?.skinId ?? _skinData.skins[0].skinId;
    }

    private bool IsSkinPurchased(string skinId)
    {
        string purchasedSkins = PlayerPrefs.GetString(PURCHASED_SKINS_KEY, "");
        return purchasedSkins.Contains(skinId) || GetSkinById(skinId).isDefault;
    }

    private void SavePurchasedSkin(string skinId)
    {
        string purchasedSkins = PlayerPrefs.GetString(PURCHASED_SKINS_KEY, "");
        if (!purchasedSkins.Contains(skinId))
        {
            purchasedSkins += (string.IsNullOrEmpty(purchasedSkins) ? "" : ",") + skinId;
            PlayerPrefs.SetString(PURCHASED_SKINS_KEY, purchasedSkins);
            PlayerPrefs.Save();
        }
    }

    private void LoadPurchasedSkins()
    {
        foreach (var skin in _skinData.skins)
        {
            if (skin.isDefault)
            {
                SavePurchasedSkin(skin.skinId);
            }
        }
    }

    private int GetPlayerCoins()
    {
        return PlayerPrefs.GetInt(COINS_KEY, 1000);
    }

    private void SpendCoins(int amount)
    {
        int currentCoins = GetPlayerCoins();
        PlayerPrefs.SetInt(COINS_KEY, currentCoins - amount);
        PlayerPrefs.Save();
    }
}