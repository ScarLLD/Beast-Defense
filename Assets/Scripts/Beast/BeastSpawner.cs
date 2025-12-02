using UnityEngine;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Beast _beastPrefab;
    [SerializeField] private SkinData _skinData;

    private Beast _beast;
    private Transform _transform;
    private string _currentSkinId;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        LoadCurrentSkin();
    }

    private void LoadCurrentSkin()
    {
        string savedSkinId = PlayerPrefs.GetString("EquippedBeastSkin", "");

        if (string.IsNullOrEmpty(savedSkinId) == false)
        {
            var skin = _skinData.GetSkinById(savedSkinId);

            if (skin != null)
            {
                _currentSkinId = savedSkinId;
            }
            else
            {
                _currentSkinId = _skinData.GetDefaultSkinId();
            }
        }
        else
        {
            _currentSkinId = _skinData.GetDefaultSkinId();
        }
    }

    public Beast Spawn()
    {
        if (_beast == null)
            _beast = Instantiate(_beastPrefab, _transform);

        ApplyCurrentSkin();

        return _beast;
    }

    public void UpdateSkin(string skinId)
    {
        if (_currentSkinId == skinId)
            return;

        _currentSkinId = skinId;

        if (_beast != null)
        {
            ApplyCurrentSkin();
        }

        PlayerPrefs.SetString("EquippedSkin", _currentSkinId);
        PlayerPrefs.Save();
    }

    private void ApplyCurrentSkin()
    {
        var skin = _skinData.GetSkinById(_currentSkinId);

        if (skin != null && skin.Model != null)
        {
            ApplySkinModel(skin.Model);
        }
    }

    private void ApplySkinModel(GameObject skinModelPrefab)
    {
        foreach (Transform child in _beast.transform)
        {
            Destroy(child.gameObject);
        }

        var model = Instantiate(skinModelPrefab, _beast.transform);
        model.name = "beastModel";
    }
}