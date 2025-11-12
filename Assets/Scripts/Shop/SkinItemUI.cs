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
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private GameObject _purchasedOverlay;
    [SerializeField] private GameObject _equippedBadge;

    private SkinShop _shop;
    private SkinData.Skin _skin;

    public string SkinId => _skin?.skinId;

    public void Initialize(SkinData.Skin skinData, SkinShop skinShop)
    {
        _skin = skinData;
        _shop = skinShop;

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
            _priceText.color = Color.yellow;
            _purchasedOverlay.SetActive(false);
        }

        SetSelected(false);
        UpdateEquippedState();
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

    public void UpdateEquippedState()
    {
        if (_equippedBadge != null)
        {
            string equippedSkinId = PlayerPrefs.GetString("EquippedSkin", "");
            _equippedBadge.SetActive(_skin.skinId == equippedSkinId);
        }
    }
}