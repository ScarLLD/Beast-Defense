using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LaunchSequencer : MonoBehaviour
{
    [SerializeField] private Game _game;

    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private GridCreator _gridCreator;
    [SerializeField] private SplineCreator _splineCreator;
    [SerializeField] private RoadSpawner _roadSpawner;
    [SerializeField] private SplineRoad _splineRoad;
    [SerializeField] private TargetDetector _detector;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private SnakeSpawner _snakeSpawner;
    [SerializeField] private BeastSpawner _bastSpawner;
    [SerializeField] private AvailabilityManagement _availabilityManagement;

    private void OnEnable()
    {
        _game.Started += OnGameStarted;
    }

    private void OnDisable()
    {
        _game.Started -= OnGameStarted;
    }

    private void OnGameStarted()
    {
        if (_boundaryMaker.TryGeneratePathMarkers() && _gridCreator.TryCreate() && _cubeCreator.TryCreate())
        {
            _availabilityManagement.UpdateAvailability();

            if (_roadSpawner.TrySpawn(out List<Vector3> road)
                && _splineCreator.TryCreateSplineWith90DegreeCorners(road, out SplineContainer splineContainer)
                && _splineRoad.TryGenerateRoadFromSpline(splineContainer))
            {
                _detector.transform.position = road[1];
                _detector.EnableTrigger();

                _snakeSpawner.Spawn(_cubeStorage.GetStacks(), splineContainer, out Snake snake);
                _bastSpawner.Spawn(road, snake, out Beast beast);
            }
        }
    }
}
