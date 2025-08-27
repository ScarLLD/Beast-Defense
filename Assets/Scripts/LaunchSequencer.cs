using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LaunchSequencer : MonoBehaviour
{
    [SerializeField] private Game game;

    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private GridCreator _gridCreator;
    [SerializeField] private SplineCreator _splineCreator;
    [SerializeField] private RoadSpawner _roadSpawner;
    [SerializeField] private TargetDetector _detector;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private SnakeSpawner _snakeSpawner;
    [SerializeField] private BeastSpawner _bastSpawner;
    [SerializeField] private AvailabilityManagement _availabilityManagement;

    private void OnEnable()
    {
        game.Started += OnGameStarted;
    }

    private void OnDisable()
    {
        game.Started -= OnGameStarted;
    }

    private void OnGameStarted()
    {
        if (_boundaryMaker.TryGeneratePathMarkers() && _gridCreator.TryCreate() && _cubeCreator.TryCreate())
        {
            _availabilityManagement.UpdateAvailability();

            if (_roadSpawner.TrySpawn(out List<Vector3> road)
                && _splineCreator.TryCreateSplineWith90DegreeCorners(road, out SplineContainer splineContainer)
                && _snakeSpawner.TrySpawn(road, out SnakeHead snakeHead)
                && _bastSpawner.TrySpawn(road, snakeHead, out Beast beast))
            {
                _detector.transform.position = road[1];
                _detector.EnableTrigger();
                beast.Init(road, snakeHead, game);
            }
        }

    }
}
