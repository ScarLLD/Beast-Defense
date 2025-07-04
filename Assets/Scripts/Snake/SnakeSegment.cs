using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SnakeMover), typeof(SnakeRotator))]
public class SnakeSegment : MonoBehaviour
{
    private SnakeRotator _snakeRotator;
    private SnakeMover _snakeMover;
    private Queue<Cube> _cubes;

    public Material Material { get; private set; }

    private void Awake()
    {
        _snakeMover = GetComponent<SnakeMover>();
        _snakeRotator = GetComponent<SnakeRotator>();
        _cubes = new Queue<Cube>();
    }

    private void Start()
    {
        _snakeRotator.SetStartRotation();
        _snakeMover.StartMoveRoutine();
        _snakeRotator.StartRotateRoutine();
    }

    public void Init(SnakeHead snakeHead)
    {
        _snakeMover.Init(snakeHead);
        _snakeRotator.Init(snakeHead);
    }

    public bool TryGetCube(out Cube cube)
    {
        cube = null;

        if (_cubes.Count > 0)
            cube = _cubes.Dequeue();

        return cube != null;
    }

    public void AddCube(Cube cube)
    {
        
            Material = cube.Material;

        _cubes.Enqueue(cube);
    }

    public bool IsCurrectColor(Color color)
    {
        return Material != null && Material.color == color;
    }

    public void TryDestroy()
    {
        if (_cubes.Count == 0)
        {
            //_snakeHead.ProcessLoss(this);
        }
    }
}
