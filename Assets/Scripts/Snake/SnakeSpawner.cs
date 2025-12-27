using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private Snake _snakePrefab;
    [SerializeField] private SkinData _skinData;
    [SerializeField] private TargetStorage _targetStorage;

    private Snake _snake;
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
        string savedSkinId = PlayerPrefs.GetString("EquippedSnakeSkin", "");

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

    public Snake Spawn(List<CubeStack> stacks, SplineContainer splineContainer, DeathModule deathModule, Beast beast)
    {
        if (_snake == null)
        {
            _snake = Instantiate(_snakePrefab, _transform);
            ApplyCurrentSkin();
        }

        _snake.InitializeSnake(stacks, splineContainer, deathModule, beast);

        return _snake;
    }

    public void UpdateSkin(string skinId)
    {
        if (_currentSkinId == skinId)
            return;

        _currentSkinId = skinId;

        if (_snake != null)
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
        Transform modelContainer = _snake.ModelContainer;

        foreach (Transform child in modelContainer)
        {
            Destroy(child.gameObject);
        }

        var model = Instantiate(skinModelPrefab, modelContainer);
        model.name = "snakeModel";
    }
}
