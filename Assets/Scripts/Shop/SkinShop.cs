using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class SkinShop : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SkinData _skinData;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private BeastSpawner _beastSpawner;

    [Header("UI References")]
    [SerializeField] private Transform _skinsContainer;
    [SerializeField] private SkinItemUI _skinItemPrefab;
    [SerializeField] private Button _closePreviewButton;

    [Header("Preview")]
    [SerializeField] private GameObject _preview;
    [SerializeField] private Image _selectedSkinImage;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private TMP_Text _selectedSkinName;
    [SerializeField] private TMP_Text _selectedSkinPrice;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private TMP_Text _buyButtonText;
    [SerializeField] private TMP_Text _selectButtonText;
    [SerializeField] private TMP_Text _actionButtonText;

    private readonly List<SkinItemUI> _skinItems = new();
    private string _selectedSkinId;
    private string _equippedSkinId;

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
        _closePreviewButton.onClick.RemoveListener(OnClosePreviewButtonClick);
    }

    private void InitializeShop()
    {
        foreach (Transform child in _skinsContainer)
        {
            Destroy(child.gameObject);
        }

        _skinItems.Clear();

        _equippedSkinId = PlayerPrefs.GetString(EQUIPPED_SKIN_KEY, GetDefaultSkinId());
        LoadPurchasedSkins();

        foreach (var skin in _skinData.Skins)
        {
            SkinItemUI skinItem = Instantiate(_skinItemPrefab, _skinsContainer);
            skinItem.Initialize(skin, this, _wallet);
            _skinItems.Add(skinItem);

            skinItem.UpdateEquippedState(_equippedSkinId);
        }

        var equppiedSkinId = PlayerPrefs.GetInt(EQUIPPED_SKIN_KEY);

        if (equppiedSkinId < 0)
            SelectFirstSkin();
    }

    private void SelectFirstSkin()
    {
        if (_skinData.Skins.Count > 0)
        {
            SelectSkin(_skinData.Skins[0].SkinId);
        }
    }

    public void OpenPreview()
    {
        _preview.SetActive(true);
    }

    public void SelectSkin(string skinId)
    {
        _selectedSkinId = skinId;
        var skin = _skinData.GetSkinById(skinId);

        if (skin != null)
        {
            _selectedSkinImage.sprite = skin.Icon;
            _selectedSkinName.text = skin.SkinName;

            bool isPurchased = IsSkinPurchased(skinId) || skin.IsDefault;
            bool isEquipped = skinId == _equippedSkinId;

            if (skin.IsDefault)
                _selectedSkinPrice.text = "Бесплатно";
            else if (isPurchased)
                _selectedSkinPrice.text = "Куплено";
            else
                _selectedSkinPrice.text = $"{skin.Price} Монет";

            _buyButton.gameObject.SetActive(!isPurchased);
            _selectButton.gameObject.SetActive(isPurchased && !isEquipped);

            if (isPurchased)
            {
                _selectButton.interactable = true;
            }
            else
            {
                if (_wallet.CanAfford(skin.Price))
                {
                    _buyButtonImage.color = Color.white;
                    _buyButtonText.text = $"КУПИТЬ";
                    _buyButton.interactable = true;
                }
                else
                {
                    _buyButtonImage.color = Color.black;
                    _buyButtonText.text = $"НЕ ХВАТАЕТ МОНЕТ";
                    _buyButton.interactable = false;
                }
            }

            foreach (var item in _skinItems)
            {
                item.SetSelected(item.SkinId == skinId);
            }

            _beastSpawner.UpdateSkin(_selectedSkinId);
        }
    }

    public bool IsSkinPurchased(string skinId)
    {
        string purchasedSkins = PlayerPrefs.GetString(PURCHASED_SKINS_KEY, "");
        return purchasedSkins.Contains(skinId) || _skinData.GetSkinById(skinId).IsDefault;
    }

    private void OnBuyButtonClick()
    {
        var skin = _skinData.GetSkinById(_selectedSkinId);

        if (skin != null && !skin.IsDefault)
        {
            BuySkin(_selectedSkinId);
            UpdateUIAfterPurchase();
        }
    }

    private void OnSelectButtonClick()
    {
        EquipSkin(_selectedSkinId);
        UpdateUIAfterSelection();
    }

    private void OnClosePreviewButtonClick()
    {
        _preview.SetActive(false);
    }

    private void BuySkin(string skinId)
    {
        var skin = _skinData.GetSkinById(skinId);

        if (_wallet.CanAfford(skin.Price))
        {
            _wallet.DecreaseMoney(skin.Price);
            SavePurchasedSkin(skinId);

            Debug.Log($"Купленый скин: {skin.SkinName}");
        }
        else
        {
            Debug.Log("Недостаточно монет!");
        }
    }

    private void EquipSkin(string skinId)
    {
        if (IsSkinPurchased(skinId) || _skinData.GetSkinById(skinId).IsDefault)
        {
            _equippedSkinId = skinId;
            PlayerPrefs.SetString(EQUIPPED_SKIN_KEY, skinId);
            PlayerPrefs.Save();

            Debug.Log($"Выбраный скин: {_skinData.GetSkinById(skinId).SkinName}");
            ApplySkinToPlayer(skinId);
        }
    }

    private void UpdateUIAfterPurchase()
    {
        SelectSkin(_selectedSkinId);

        foreach (var item in _skinItems)
        {
            item.UpdatePurchaseState(IsSkinPurchased(item.SkinId));
            item.UpdateEquippedState(_equippedSkinId);
        }
    }

    private void UpdateUIAfterSelection()
    {
        SelectSkin(_selectedSkinId);
        foreach (var item in _skinItems)
        {
            item.UpdateEquippedState(_equippedSkinId);
        }
    }

    private void ApplySkinToPlayer(string skinId)
    {
        var skin = _skinData.GetSkinById(skinId);
        if (skin != null && skin.Model != null)
        {
            Debug.Log($"Применен: {skin.SkinName}");
        }
    }

    private string GetDefaultSkinId()
    {
        var defaultSkin = _skinData.Skins.Find(skin => skin.IsDefault);
        return defaultSkin?.SkinId ?? _skinData.Skins[0].SkinId;
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
        foreach (var skin in _skinData.Skins)
        {
            if (skin.IsDefault)
            {
                SavePurchasedSkin(skin.SkinId);
            }
        }
    }
}