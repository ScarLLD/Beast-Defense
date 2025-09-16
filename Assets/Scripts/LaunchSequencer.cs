using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LaunchSequencer : MonoBehaviour
{
    [SerializeField] private Game _game;

    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private PlaceSpawner _placeSpawner;
    [SerializeField] private GridCreator _gridCreator;
    [SerializeField] private SplineCreator _splineCreator;
    [SerializeField] private RoadSpawner _roadSpawner;
    [SerializeField] private SplineRoad _splineRoad;
    [SerializeField] private TargetDetector _detector;
    [SerializeField] private PlayerCubeSpawner _playerCubeSpawner;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private SnakeSpawner _snakeSpawner;
    [SerializeField] private BeastSpawner _beastSpawner;
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
        if (_boundaryMaker.TryGeneratePathMarkers() && _playerCubeSpawner.TryMoveToCenterScreenBottom()
            && _gridCreator.TryCreate(out Vector3 cubeScale) && _placeSpawner.TryGeneratePlaces(cubeScale) && _cubeCreator.TryCreate())
        {
            _availabilityManagement.UpdateAvailability();

            if (_roadSpawner.TrySpawn(out List<Vector3> road)
                && _splineCreator.TryCreateSplineWith90DegreeCorners(road, out SplineContainer splineContainer)
                && _splineRoad.TryGenerateRoadFromSpline(splineContainer))
            {
                _snakeSpawner.Spawn(_cubeStorage.GetStacks(), splineContainer, out Snake snake);
                _beastSpawner.Spawn(snake, splineContainer);

                _detector.transform.position = road[1] + Vector3.up * snake.transform.localScale.y;
                _detector.gameObject.SetActive(true);
                _detector.EnableTrigger();
            }
        }
    }
}
