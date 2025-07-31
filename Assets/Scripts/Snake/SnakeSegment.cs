using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SnakeMover), typeof(SnakeRotator))]
public class SnakeSegment : MonoBehaviour
{
    private int currentCubesCount = 0;
    private SnakeHead _snakeHead;
    private SnakeRotator _snakeRotator;
    private SnakeMover _snakeMover;
    private Queue<Cube> _cubes;

    public SnakeRotator SnakeRotator => _snakeRotator;
    public SnakeMover SnakeMover => _snakeMover;
    public Material Material { get; private set; }
    public bool IsTarget { get; private set; } = false;
    public bool IsNullHead => _snakeHead == null;

    private void Awake()
    {
        _snakeMover = GetComponent<SnakeMover>();
        _snakeRotator = GetComponent<SnakeRotator>();
        _cubes = new Queue<Cube>();
    }

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
        _snakeMover.Init(snakeHead);
        _snakeRotator.Init(snakeHead);
    }

    public void StartRoutine()
    {
        _snakeMover.StartMoveRoutine();
        _snakeRotator.StartRotateRoutine();
    }

    public void SetIsTarget(bool isTarget)
    {
        IsTarget = isTarget;
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
        currentCubesCount++;

        _cubes.Enqueue(cube);
    }

    public bool IsCurrectColor(Color color)
    {
        return Material != null && Material.color == color;
    }

    public void TryDestroy()
    {
        currentCubesCount--;

        if (currentCubesCount == 0)
            _snakeHead.DeleteSegment(this);
    }

    public void ActivateCubes(Material material)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Cube cube))
            {
                cube.Init(material);
                AddCube(cube);
                cube.gameObject.SetActive(true);
            }
        }
    }
}
