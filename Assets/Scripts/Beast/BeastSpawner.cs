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

        _currentSkinId = PlayerPrefs.GetString("EquippedSkin", _skinData.GetDefaultSkinId());
    }

    public Beast Spawn()
    {
        if (_beast == null)
        {
            _beast = Instantiate(_beastPrefab, _transform);
            ApplyCurrentSkin();
        }

        return _beast;
    }

    public void UpdateSkin(string skinId)
    {
        _currentSkinId = skinId;

        if (_beast != null)
        {
            ApplyCurrentSkin();
        }
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
        //_beast.RebindAimator();
    }
}