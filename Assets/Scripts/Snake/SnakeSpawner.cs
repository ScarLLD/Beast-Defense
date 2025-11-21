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
        _currentSkinId = skinId;

        if (_snake != null)
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
        Transform modelContainer = _snake.ModelContainer;

        foreach (Transform child in modelContainer)
        {
            Destroy(child.gameObject);
        }

        Instantiate(skinModelPrefab, modelContainer);
    }
}
