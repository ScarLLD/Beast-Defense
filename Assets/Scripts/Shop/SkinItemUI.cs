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
    [SerializeField] private GameObject _priceParent;
    [SerializeField] private GameObject _purchasedOverlay;
    [SerializeField] private GameObject _equippedBadge;

    private SkinShop _shop;
    private SkinData.Skin _skin;
    private Wallet _wallet;

    public string SkinId => _skin?.SkinId;

    public void Initialize(SkinData.Skin skinData, SkinShop skinShop, Wallet wallet)
    {
        _skin = skinData;
        _shop = skinShop;
        _wallet = wallet;

        _skinIcon.sprite = _skin.Icon;

        if (_skin.IsDefault || _shop.IsSkinPurchased(_skin.SkinId))
        {
            _purchasedOverlay.SetActive(true);
            _priceParent.SetActive(false);
        }
        else
        {
            _priceText.text = $"{_skin.Price}";

            if (_wallet.CanAfford(_skin.Price))
                _priceText.color = Color.green;
            else
                _priceText.color = Color.red;

            _purchasedOverlay.SetActive(false);
        }

        SetSelected(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _shop.SelectSkin(_skin.SkinId);
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
            _equippedBadge.SetActive(_skin.SkinId == equippedSkinId);
        }
    }

    public void UpdatePurchaseState(bool isPurchased)
    {
        if (_purchasedOverlay != null)
        {
            Debug.Log("PurshaseStateUpdated");
            _purchasedOverlay.SetActive(isPurchased || _skin.IsDefault);
            _priceParent.SetActive(isPurchased == false && _skin.IsDefault == false);
        }

        if (_priceText != null)
        {
            if (isPurchased || _skin.IsDefault)
            {
                _priceText.text = "Есть";
                _priceText.color = Color.green;
            }
            else
            {
                _priceText.text = $"{_skin.Price}";

                if (_wallet.CanAfford(_skin.Price))
                    _priceText.color = Color.green;
                else
                    _priceText.color = Color.red;
            }
        }
    }
}