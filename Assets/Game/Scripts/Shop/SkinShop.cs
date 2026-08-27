using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using Game.Scripts.BeastCore;
using Game.Scripts.Effects;
using Game.Scripts.SnakeCore;
using Game.Scripts.UI;
using TMPro;
using YG;
using Game.Scripts.Shop.Skins;

namespace Game.Scripts.Shop
{
    public class SkinShop : MonoBehaviour
    {
        private readonly List<SkinItemUI> _beastSkinItems = new();
        private readonly List<SkinItemUI> _snakeSkinItems = new();
        private readonly Color _greenColor = new(0.004f, 0.78f, 0.57f);
        private readonly Color _redColor = new(1f, 0.3f, 0.25f);

        [Header("Skins")] [SerializeField] private SkinData _beastSkinData;
        [SerializeField] private SkinData _snakeSkinData;
        [SerializeField] private Wallet _wallet;
        [SerializeField] private BeastSpawner _beastSpawner;
        [SerializeField] private SnakeSpawner _snakeSpawner;
        [SerializeField] private LanguageInitializer _language;

        [Header("UI References")] [SerializeField]
        private Transform _beastSkinsContainer;

        [SerializeField] private Transform _snakeSkinsContainer;
        [SerializeField] private SkinItemUI _skinItemPrefab;
        [SerializeField] private Button _closePreviewButton;

        [Header("Section Headers")] [SerializeField]
        private TMP_Text _beastSectionHeader;

        [SerializeField] private TMP_Text _snakeSectionHeader;

        [Header("Preview")] [SerializeField] private SkinItemPreviewOpenAnimator _previewAnimator;
        [SerializeField] private GameObject _preview;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _selectedSkinImage;
        [SerializeField] private Image _buyButtonImage;
        [SerializeField] private TMP_Text _selectedSkinName;
        [SerializeField] private TMP_Text _selectedSkinTypeText;
        [SerializeField] private TMP_Text _selectedSkinPrice;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private TMP_Text _buyButtonText;
        [SerializeField] private TMP_Text _selectButtonText;

        private string _selectedSkinId;
        private SkinType _selectedSkinType;
        private string _equippedBeastSkinId;
        private string _equippedSnakeSkinId;

        public event Action Purchased;
        public event Action Selected;

        public enum SkinType
        {
            Beast,
            Snake
        }

        private void OnEnable()
        {
            _buyButton.onClick.AddListener(OnBuyButtonClick);
            _selectButton.onClick.AddListener(OnSelectButtonClick);
            _closePreviewButton.onClick.AddListener(OnClosePreviewButtonClick);

            UpdateItemsUI();
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
            ClearContainers();

            _equippedBeastSkinId = !string.IsNullOrEmpty(YG2.saves.EquippedBeastSkin)
                ? YG2.saves.EquippedBeastSkin
                : GetDefaultSkinId(_beastSkinData);

            _equippedSnakeSkinId = !string.IsNullOrEmpty(YG2.saves.EquippedSnakeSkin)
                ? YG2.saves.EquippedSnakeSkin
                : GetDefaultSkinId(_snakeSkinData);

            LoadPurchasedSkins();

            foreach (var skin in _beastSkinData.Skins)
            {
                var skinItem = Instantiate(_skinItemPrefab, _beastSkinsContainer);
                skinItem.Initialize(skin, this, _wallet, SkinType.Beast, _greenColor, _redColor);
                _beastSkinItems.Add(skinItem);
                skinItem.UpdateEquippedState(_equippedBeastSkinId, SkinType.Beast);
            }

            foreach (var skin in _snakeSkinData.Skins)
            {
                var skinItem = Instantiate(_skinItemPrefab, _snakeSkinsContainer);
                skinItem.Initialize(skin, this, _wallet, SkinType.Snake, _greenColor, _redColor);
                _snakeSkinItems.Add(skinItem);
                skinItem.UpdateEquippedState(_equippedSnakeSkinId, SkinType.Snake);
            }

            SelectFirstSkin();
        }

        private void UpdateItemsUI()
        {
            foreach (var item in _beastSkinItems)
            {
                var isPurchased = IsSkinPurchased(item.SkinId, SkinType.Beast);
                item.UpdatePurchaseState(isPurchased);
                item.UpdateEquippedState(_equippedBeastSkinId, SkinType.Beast);
            }

            foreach (var item in _snakeSkinItems)
            {
                var isPurchased = IsSkinPurchased(item.SkinId, SkinType.Snake);
                item.UpdatePurchaseState(isPurchased);
                item.UpdateEquippedState(_equippedSnakeSkinId, SkinType.Snake);
            }
        }

        private void ClearContainers()
        {
            foreach (Transform child in _beastSkinsContainer)
            {
                Destroy(child.gameObject);
            }

            _beastSkinItems.Clear();

            foreach (Transform child in _snakeSkinsContainer)
            {
                Destroy(child.gameObject);
            }

            _snakeSkinItems.Clear();
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

        public bool TryOpenPreview(string skinId, SkinType skinType, Vector3 startPosition)
        {
            if (_preview.activeInHierarchy) return false;

            _preview.SetActive(true);
            SelectSkin(skinId, skinType);
            _previewAnimator.Open(startPosition);

            return true;
        }

        public void SelectSkin(string skinId, SkinType skinType)
        {
            _selectedSkinId = skinId;
            _selectedSkinType = skinType;

            var skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;
            var skin = skinData.GetSkinById(skinId);

            if (skin == null) return;

            _selectedSkinImage.sprite = skin.Icon;
            _selectedSkinName.text = skin.GetLocalizedName(YG2.lang);
            _selectedSkinTypeText.text = skinType == SkinType.Snake
                ? InterfaceLocalization.GetLocalizedSnakeType(YG2.lang)
                : InterfaceLocalization.GetLocalizedBeastType(YG2.lang);


            var isPurchased = IsSkinPurchased(skinId, skinType) || skin.IsDefault;
            var isEquipped = IsSkinEquipped(skinId, skinType);

            if (skin.IsDefault)
                _selectedSkinPrice.text = InterfaceLocalization.GetLocalizedFreeText(YG2.lang);
            else if (isPurchased)
                _selectedSkinPrice.text = InterfaceLocalization.GetLocalizedPurchasedText(YG2.lang);
            else
                _selectedSkinPrice.text = $"{skin.Price} {InterfaceLocalization.GetLocalizedMoneyText(YG2.lang)}";

            _buyButton.gameObject.SetActive(!isPurchased);
            _selectButton.gameObject.SetActive(isPurchased && !isEquipped);

            if (isPurchased)
            {
                _selectButton.interactable = true;
                _selectButtonText.text = InterfaceLocalization.GetLocalizedTakeText(YG2.lang);
                _backgroundImage.color = _greenColor;
            }
            else
            {
                _backgroundImage.color = _redColor;

                if (Wallet.CanAfford(skin.Price))
                {
                    _buyButtonImage.color = Color.white;
                    _buyButtonText.text = InterfaceLocalization.GetLocalizedBuyText(YG2.lang);
                    _buyButton.interactable = true;
                }
                else
                {
                    _buyButtonImage.color = Color.black;
                    _buyButtonText.text = InterfaceLocalization.GetLocalizedNoMoneyText(YG2.lang);
                    _buyButton.interactable = false;
                }
            }
        }

        private bool IsSkinEquipped(string skinId, SkinType skinType)
        {
            return skinType == SkinType.Beast ? skinId == _equippedBeastSkinId : skinId == _equippedSnakeSkinId;
        }

        public bool IsSkinPurchased(string skinId, SkinType skinType)
        {
            var list = skinType == SkinType.Beast
                ? YG2.saves.PurchasedBeastSkins
                : YG2.saves.PurchasedSnakeSkins;

            var skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;
            var isDefault = skinData.GetSkinById(skinId)?.IsDefault ?? false;

            var result = isDefault || (!string.IsNullOrEmpty(list) && list.Split(',').Contains(skinId));

            return result;
        }

        private static string GetDefaultSkinId(SkinData skinData)
        {
            var defaultSkin = skinData.Skins.Find(skin => skin.IsDefault);
            return defaultSkin?.SkinId ?? skinData.Skins[0].SkinId;
        }

        private static void SavePurchasedSkin(string skinId, SkinType skinType)
        {
            var purchasedSkins = skinType == SkinType.Beast
                ? YG2.saves.PurchasedBeastSkins
                : YG2.saves.PurchasedSnakeSkins;

            if (!string.IsNullOrEmpty(purchasedSkins) && purchasedSkins.Contains(skinId))
                return;

            purchasedSkins += string.IsNullOrEmpty(purchasedSkins) ? skinId : $",{skinId}";

            if (skinType == SkinType.Beast)
                YG2.saves.PurchasedBeastSkins = purchasedSkins;
            else
                YG2.saves.PurchasedSnakeSkins = purchasedSkins;

            YG2.SaveProgress();
        }

        private void OnBuyButtonClick()
        {
            var skinData = _selectedSkinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;
            var skin = skinData.GetSkinById(_selectedSkinId);

            if (skin is not { IsDefault: false }) return;

            BuySkin(_selectedSkinId, _selectedSkinType);
            UpdateUIAfterPurchase();
            OnSelectButtonClick();
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
            var skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;
            var skin = skinData.GetSkinById(skinId);

            if (!Wallet.CanAfford(skin.Price)) return;

            _wallet.DecreaseMoney(skin.Price);
            SavePurchasedSkin(skinId, skinType);
            Purchased?.Invoke();
        }

        private void EquipSkin(string skinId, SkinType skinType)
        {
            var skinData = skinType == SkinType.Beast ? _beastSkinData : _snakeSkinData;

            if (!IsSkinPurchased(skinId, skinType) && !skinData.GetSkinById(skinId).IsDefault) return;

            if (skinType == SkinType.Beast)
            {
                _equippedBeastSkinId = skinId;
                YG2.saves.EquippedBeastSkin = skinId;
                _beastSpawner.UpdateSkin(skinId);
            }
            else
            {
                _equippedSnakeSkinId = skinId;
                YG2.saves.EquippedSnakeSkin = skinId;
                _snakeSpawner.UpdateSkin(skinId);
            }

            YG2.SaveProgress();
            Selected?.Invoke();
        }

        private void UpdateUIAfterPurchase()
        {
            SelectSkin(_selectedSkinId, _selectedSkinType);

            foreach (var item in _beastSkinItems)
            {
                var isPurchased = IsSkinPurchased(item.SkinId, SkinType.Beast);
                item.UpdatePurchaseState(isPurchased);
                item.UpdateEquippedState(_equippedBeastSkinId, SkinType.Beast);
            }

            foreach (var item in _snakeSkinItems)
            {
                var isPurchased = IsSkinPurchased(item.SkinId, SkinType.Snake);
                item.UpdatePurchaseState(isPurchased);
                item.UpdateEquippedState(_equippedSnakeSkinId, SkinType.Snake);
            }
        }

        private void UpdateUIAfterSelection()
        {
            SelectSkin(_selectedSkinId, _selectedSkinType);

            if (_selectedSkinType == SkinType.Beast)
            {
                foreach (var item in _beastSkinItems)
                {
                    item.UpdateEquippedState(_equippedBeastSkinId, SkinType.Beast);
                }
            }
            else
            {
                foreach (var item in _snakeSkinItems)
                {
                    item.UpdateEquippedState(_equippedSnakeSkinId, SkinType.Snake);
                }
            }
        }

        private void LoadPurchasedSkins()
        {
            foreach (var skin in _beastSkinData.Skins.Where(skin => skin.IsDefault))
            {
                SavePurchasedSkin(skin.SkinId, SkinType.Beast);
            }

            foreach (var skin in _snakeSkinData.Skins.Where(skin => skin.IsDefault))
            {
                SavePurchasedSkin(skin.SkinId, SkinType.Snake);
            }
        }
    }
}