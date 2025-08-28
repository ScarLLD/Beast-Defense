using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SnakeCreator : MonoBehaviour
{
    [SerializeField] private SplineCreator _splineCreator;
    [SerializeField] private Snake _snakePrefab;

    private void Start()
    {
        //_splineCreator.CreateSplineWith90DegreeCorners(out SplineContainer _splineContainer);
        //Snake snake = Instantiate(_snakePrefab, transform);
        //snake.InitializeSnake(_splineContainer);
    }
}
