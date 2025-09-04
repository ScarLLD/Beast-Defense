using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] private List<Cube> _cubes;
    private int _currentCubeIndex = 0;
    private bool _isDestroyed = false;
    private Snake _snake;

    public Material Material { get; private set; }
    public bool IsTarget { get; private set; } = false;

    public void Init(Material material, Snake snake)
    {
        _snake = snake;
        Material = material;
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
            if (_cubes[i].IsDestroyed == false)
            {
                _cubes[i].gameObject.SetActive(true);
            }
            else
            {
                _cubes[i].gameObject.SetActive(false);
            }
        }
    }

    public bool TryGetCube(out Cube cube)
    {
        cube = null;

        if (_currentCubeIndex < _cubes.Count)
        {
            cube = _cubes[_currentCubeIndex];
            _currentCubeIndex++;
            return true;
        }

        return false;
    }


    public void TryDestroy()
    {
        if (_isDestroyed == false)
        {
            foreach (var cube in _cubes)
            {
                if (cube.IsDestroyed == false)
                {                    
                    return;
                }
            }

            _isDestroyed = true;
            _snake?.DestroySegment(this);
            gameObject.SetActive(false);
        }
    }

    public void SetActiveSegment(bool active)
    {

        if (_isDestroyed == false)
        {
            gameObject.SetActive(active);

            if (active)
            {
                ActivateVisibleCubes();
            }
        }
    }
}
