using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SkinShop : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SkinData _beastSkinData;
    [SerializeField] private SkinData _snakeSkinData;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private BeastSpawner _beastSpawner;
    [SerializeField] private SnakeSpawner _snakeSpawner;

    [Header("UI References")]
    [SerializeField] private Transform _beastSkinsContainer;
    [SerializeField] private Transform _snakeSkinsContainer;
    [SerializeField] private SkinItemUI _skinItemPrefab;
    [SerializeField] private Button _closePreviewButton;

    [Header("Section Headers")]
    [SerializeField] private TMP_Text _beastSectionHeader;
    [SerializeField] private TMP_Text _snakeSectionHeader;

    [Header("Preview")]
    [SerializeField] private GameObject _preview;
    [SerializeField] private Image _selectedSkinImage;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private TMP_Text _selectedSkinName;
    [SerializeField] private TMP_Text _selectedSkinTypeText;
    [SerializeField] private TMP_Text _selectedSkinPrice;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private TMP_Text _buyButtonText;
    [SerializeField] private TMP_Text _selectButtonText;

    private readonly List<SkinItemUI> _skinItems = new();
    private string _selectedSkinId;
    private SkinType _selectedSkinType;
    private string _equippedBeastSkinId;
    private string _equippedSnakeSkinId;

    private const string EQUIPPED_BEAST_SKIN_KEY = "EquippedBeastSkin";
    private const string EQUIPPED_SNAKE_SKIN_KEY = "EquippedSnakeSkin";
    private const string PURCHASED_BEAST_SKINS_KEY = "PurchasedBeastSkins";
    private const string PURCHASED_SNAKE_SKINS_KEY = "PurchasedSnakeSkins";

    private void OnEnable()
    {
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

    private void Start()
    {
        InitializeShop();
    }

    private void InitializeShop()
    {
        ClearContainer();

        _equippedBeastSkinId = PlayerPrefs.GetString(EQUIPPED_BEAST_SKIN_KEY, GetDefaultSkinId(_beastSkinData));
        _equippedSnakeSkinId = PlayerPrefs.GetString(EQUIPPED_SNAKE_SKIN_KEY, GetDefaultSkinId(_snakeSkinData));

        LoadPurchasedSkins();

        foreach (var skin in _beastSkinData.Skins)
        {
            SkinItemUI skinItem = Instantiate(_skinItemPrefab, _beastSkinsContainer);
            skinItem.Initialize(skin, this, _wallet, SkinType.Beast);
            _skinItems.Add(skinItem);
            skinItem.UpdateEquippedState(_equippedBeastSkinId, SkinType.Beast);
        }

        foreach (var skin in _snakeSkinData.Skins)
        {
            SkinItemUI skinItem = Instantiate(_skinItemPrefab, _snakeSkinsContainer);
            skinItem.Initialize(skin, this, _wallet, SkinType.Snake);
            _skinItems.Add(skinItem);
            skinItem.UpdateEquippedState(_equippedSnakeSkinId, SkinType.Snake);
        }

        SelectFirstSkin();
    }

    private void ClearContainer()
    {
        foreach (Transform child in _beastSkinsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in _snakeSkinsContainer)
        {
            Destroy(child.gameObject);
        }

        _skinItems.Clear();
    }

    private void SelectFirstSkin()
    {
        if (_beastSkinData.Skins.Count > 0)
        {
            SelectSkin(_beastSkinData.Skins[0].SkinId, SkinType.Beast);
        }
        else if (_snakeSkinData.Skins.Count > 0)
        {
            SelectSkin(_snakeSkinData.Skins[0].SkinId, SkinType.Snake);
        }
    }

    public void OpenPreview()
    {
        _preview.SetActive(true);
    }

    public void SelectSkin(string skinId, SkinType skinType)
    {
        _selectedSkinId = skinId;
        _selectedSkinType = skinType;

        SkinData skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;
        var skin = skinData.GetSkinById(skinId);

        if (skin != null)
        {
            _selectedSkinImage.sprite = skin.Icon;
            _selectedSkinName.text = skin.SkinName;
            _selectedSkinTypeText.text = skinType == SkinType.Beast ? "ЗВЕРЬ" : "ЗМЕЯ";

            bool isPurchased = IsSkinPurchased(skinId, skinType) || skin.IsDefault;
            bool isEquipped = IsSkinEquipped(skinId, skinType);

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
                _selectButtonText.text = "ВЫБРАТЬ";
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
                item.SetSelected(item.SkinId == skinId && item.SkinType == skinType);
            }
        }
    }

    private bool IsSkinEquipped(string skinId, SkinType skinType)
    {
        return skinType == SkinType.Beast ?
            skinId == _equippedBeastSkinId :
            skinId == _equippedSnakeSkinId;
    }

    public bool IsSkinPurchased(string skinId, SkinType skinType)
    {
        string key = skinType == SkinType.Beast ? PURCHASED_BEAST_SKINS_KEY : PURCHASED_SNAKE_SKINS_KEY;
        string purchasedSkins = PlayerPrefs.GetString(key, "");
        SkinData skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;

        return purchasedSkins.Contains(skinId) || skinData.GetSkinById(skinId).IsDefault;
    }

    private void OnBuyButtonClick()
    {
        SkinData skinData = _selectedSkinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;
        var skin = skinData.GetSkinById(_selectedSkinId);

        if (skin != null && !skin.IsDefault)
        {
            BuySkin(_selectedSkinId, _selectedSkinType);
            UpdateUIAfterPurchase();
        }
    }

    private void OnSelectButtonClick()
    {
        EquipSkin(_selectedSkinId, _selectedSkinType);
        UpdateUIAfterSelection();
    }

    private void OnClosePreviewButtonClick()
    {
        _preview.SetActive(false);
    }

    private void BuySkin(string skinId, SkinType skinType)
    {
        SkinData skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;
        var skin = skinData.GetSkinById(skinId);

        if (_wallet.CanAfford(skin.Price))
        {
            _wallet.DecreaseMoney(skin.Price);
            SavePurchasedSkin(skinId, skinType);

            Debug.Log($"Куплен скин: {skin.SkinName} для {skinType}");
        }
        else
        {
            Debug.Log("Недостаточно монет!");
        }
    }

    private void EquipSkin(string skinId, SkinType skinType)
    {
        SkinData skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;

        if (IsSkinPurchased(skinId, skinType) || skinData.GetSkinById(skinId).IsDefault)
        {
            if (skinType == SkinType.Beast)
            {
                _equippedBeastSkinId = skinId;
                PlayerPrefs.SetString(EQUIPPED_BEAST_SKIN_KEY, skinId);
            }
            else
            {
                _equippedSnakeSkinId = skinId;
                PlayerPrefs.SetString(EQUIPPED_SNAKE_SKIN_KEY, skinId);
            }

            PlayerPrefs.Save();

            Debug.Log($"Выбран скин: {skinData.GetSkinById(skinId).SkinName} для {skinType}");
            ApplySkinToPlayer(skinId, skinType);
        }
    }

    private void UpdateUIAfterPurchase()
    {
        SelectSkin(_selectedSkinId, _selectedSkinType);

        foreach (var item in _skinItems)
        {
            item.UpdatePurchaseState(IsSkinPurchased(item.SkinId, item.SkinType));
            item.UpdateEquippedState(
                item.SkinType == SkinType.Beast ? _equippedBeastSkinId : _equippedSnakeSkinId,
                item.SkinType
            );
        }
    }

    private void UpdateUIAfterSelection()
    {
        SelectSkin(_selectedSkinId, _selectedSkinType);
        foreach (var item in _skinItems)
        {
            item.UpdateEquippedState(
                item.SkinType == SkinType.Beast ? _equippedBeastSkinId : _equippedSnakeSkinId,
                item.SkinType
            );
        }
    }

    private void ApplySkinToPlayer(string skinId, SkinType skinType)
    {
        var skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;
        var skin = skinData.GetSkinById(skinId);

        if (skin != null && skin.Model != null)
        {
            if (skinType == SkinType.Beast)
            {
                _beastSpawner.UpdateSkin(skinId);
            }
            else if (skinType == SkinType.Snake)
            {
                //_snakeSpawner.UpdateSkin(skinId);
            }
            Debug.Log($"Применен скин: {skin.SkinName} для {skinType}");
        }
    }

    private string GetDefaultSkinId(SkinData skinData)
    {
        var defaultSkin = skinData.Skins.Find(skin => skin.IsDefault);
        return defaultSkin?.SkinId ?? skinData.Skins[0].SkinId;
    }

    private void SavePurchasedSkin(string skinId, SkinType skinType)
    {
        string key = skinType == SkinType.Beast ? PURCHASED_BEAST_SKINS_KEY : PURCHASED_SNAKE_SKINS_KEY;
        string purchasedSkins = PlayerPrefs.GetString(key, "");

        if (!purchasedSkins.Contains(skinId))
        {
            purchasedSkins += (string.IsNullOrEmpty(purchasedSkins) ? "" : ",") + skinId;
            PlayerPrefs.SetString(key, purchasedSkins);
            PlayerPrefs.Save();
        }
    }

    private void LoadPurchasedSkins()
    {
        foreach (var skin in _beastSkinData.Skins)
        {
            if (skin.IsDefault)
            {
                SavePurchasedSkin(skin.SkinId, SkinType.Beast);
            }
        }

        foreach (var skin in _snakeSkinData.Skins)
        {
            if (skin.IsDefault)
            {
                SavePurchasedSkin(skin.SkinId, SkinType.Snake);
            }
        }
    }

    public enum SkinType
    {
        Beast,
        Snake
    }
}