using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(RectTransform), typeof(SkinItemUIClickAnimator))]
public class SkinItemUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image _skinIcon;
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private GameObject _priceParent;
    [SerializeField] private GameObject _equippedBadge;

    private RectTransform _rectTransform;
    private SkinItemUIClickAnimator _clickAnimator;
    private SkinShop _shop;
    private SkinData.Skin _skin;
    private Wallet _wallet;
    private SkinShop.SkinType _skinType;

    private Color _greenColor;
    private Color _redColor;

    public string SkinId => _skin?.SkinId;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _clickAnimator = GetComponent<SkinItemUIClickAnimator>();
    }

    public void Initialize(SkinData.Skin skin, SkinShop shop, Wallet wallet, SkinShop.SkinType skinType, Color greenColor, Color redColor)
    {
        _wallet = wallet;
        _skin = skin;
        _shop = shop;
        _skinType = skinType;
        _skinIcon.sprite = skin.Icon;

        _greenColor = greenColor;
        _redColor = redColor;

        if (skin.IsDefault)
        {
            _priceText.text = "0";
            _priceText.color = Color.green;
            _priceParent.SetActive(false);
        }
        else
        {
            _priceText.text = $"{skin.Price}";

            _priceText.color = Color.white;
        }

        UpdatePurchaseState(_shop.IsSkinPurchased(skin.SkinId, skinType));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_shop.TryOpenPreview(_skin.SkinId, _skinType, _rectTransform.position))
            _clickAnimator.Interact();
    }

    public void UpdatePurchaseState(bool isPurchased)
    {
        if (isPurchased || _skin.IsDefault)
        {
            _priceParent.SetActive(false);

            if (_background != null)
                _background.color = _greenColor;
        }
        else
        {
            _priceParent.SetActive(true);

            if (_background != null)
                _background.color = _redColor;

            bool canAfford = _wallet.CanAfford(_skin.Price);
            _priceText.color = canAfford ? Color.green : Color.red;
        }
    }

    public void UpdateEquippedState(string equippedSkinId, SkinShop.SkinType type)
    {
        if (_equippedBadge != null)
        {
            _equippedBadge.SetActive(_skin.SkinId == equippedSkinId && _skinType == type);
        }
    }
}