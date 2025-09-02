using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] private List<Cube> _cubes;
    private int _currentCubeIndex = 0;
    private Snake _snake;

    public Material Material { get; private set; }
    public bool IsTarget { get; private set; } = false;

    private int _maxVisibleCubes = 20;
    private bool _isDestroyed = false; 

    public void Init(Material material, Snake snake)
    {
        _snake = snake;
        Material = material;
        _currentCubeIndex = 0;
        IsTarget = false;
        _isDestroyed = false;

        foreach (var cube in _cubes)
        {
            cube.InitSegment(this);
            cube.Init(material);
            cube.Deactivate();
        }

        gameObject.SetActive(false);
    }

    public void SetIsTarget(bool isTarget)
    {
        IsTarget = isTarget;
    }

    public bool IsCurrectColor(Color color)
    {
        return Material != null && Material.color == color;
    }

    private void ActivateVisibleCubes()
    {
        for (int i = 0; i < _cubes.Count; i++)
        {
            _cubes[i].gameObject.SetActive(i >= _currentCubeIndex && i < _currentCubeIndex + _maxVisibleCubes);
        }
    }

    public bool TryGetCube(out Cube cube)
    {
        cube = null;
        if (_currentCubeIndex < _cubes.Count)
        {
            cube = _cubes[_currentCubeIndex];
            _currentCubeIndex++;
            ActivateVisibleCubes();
            return true;
        }
        return false;
    }


    public void TryDestroy()
    {
        if (_isDestroyed) return;
                
        if (_currentCubeIndex >= _cubes.Count)
        {
            _isDestroyed = true;
            _snake?.DestroySegment(this);
        }
    }

    public void SetActiveSegment(bool active)
    {        
        if (_isDestroyed) return;

        gameObject.SetActive(active);
        if (active) ActivateVisibleCubes();
    }

    public bool IsDestroyed() => _isDestroyed;
}
