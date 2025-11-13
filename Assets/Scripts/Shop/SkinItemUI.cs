using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkinItemUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image _skinIcon;
    [SerializeField] private Image _background;
    [SerializeField] private Image _selectedFrame;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private GameObject _purchasedOverlay;
    [SerializeField] private GameObject _equippedBadge;

    private SkinShop _shop;
    private SkinData.Skin _skin;
    private Wallet _wallet;

    public string SkinId => _skin?.skinId;

    public void Initialize(SkinData.Skin skinData, SkinShop skinShop, Wallet wallet)
    {
        _skin = skinData;
        _shop = skinShop;
        _wallet = wallet;

        // Настройка внешнего вида
        _skinIcon.sprite = _skin.icon;

        if (_skin.isDefault)
        {
            _priceText.text = "0";
            _priceText.color = Color.green;
            _purchasedOverlay.SetActive(true);
        }
        else
        {
            _priceText.text = $"{_skin.price}";

            if (_wallet.Money >= _skin.price)
                _priceText.color = Color.green;
            else
                _priceText.color = Color.red;

            _purchasedOverlay.SetActive(false);
        }

        // Первоначальное состояние
        SetSelected(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _shop.SelectSkin(_skin.skinId);
        _shop.OpenPreview();
    }

    public void SetSelected(bool selected)
    {
        if (_selectedFrame != null)
            _selectedFrame.gameObject.SetActive(selected);

        if (_background != null)
            _background.color = selected ? new Color(0.8f, 0.8f, 1f) : Color.white;
    }

    public void UpdateEquippedState(string equippedSkinId)
    {
        if (_equippedBadge != null)
        {
            _equippedBadge.SetActive(_skin.skinId == equippedSkinId);
        }
    }

    public void UpdatePurchaseState(bool isPurchased)
    {
        if (_purchasedOverlay != null)
        {
            _purchasedOverlay.SetActive(isPurchased || _skin.isDefault);
        }

        if (_priceText != null)
        {
            if (isPurchased || _skin.isDefault)
            {
                _priceText.text = "КУПЛЕНО";
                _priceText.color = Color.green;
            }
            else if (_wallet.Money >= _skin.price)
            {
                _priceText.text = $"{_skin.price}";

                if (_wallet.Money >= _skin.price)
                    _priceText.color = Color.green;
                else
                    _priceText.color = Color.red;
            }
        }
    }
}