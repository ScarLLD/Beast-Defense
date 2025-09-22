using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LaunchSequencer : MonoBehaviour
{
    [SerializeField] private Game _game;

    [Header("Spawners")]
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private PlaceSpawner _placeSpawner;
    [SerializeField] private GridCreator _gridCreator;
    [SerializeField] private SplineCreator _splineCreator;
    [SerializeField] private RoadSpawner _roadSpawner;
    [SerializeField] private SplineRoad _splineRoad;
    [SerializeField] private TargetDetector _detector;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private SnakeSpawner _snakeSpawner;
    [SerializeField] private BeastSpawner _beastSpawner;
    [SerializeField] private SmoothBarSlider _slider;
    [SerializeField] private AvailabilityManagement _availabilityManagement;

    private void OnEnable()
    {
        _game.Started += Launch;
    }

    private void OnDisable()
    {
        _game.Started -= Launch;
    }

    public void Launch()
    {
        if (_boundaryMaker.TryGeneratePathMarkers() && _gridCreator.TryCreate(out Vector3 cubeScale)
            && _placeSpawner.TryGeneratePlaces(cubeScale) && _cubeCreator.TryCreate())
        {
            _availabilityManagement.UpdateAvailability();

            if (_roadSpawner.TrySpawn(out List<Vector3> road)
                && _splineCreator.TryCreateSpline(road, out SplineContainer splineContainer)
                && _splineRoad.TryGenerateRoadFromSpline(splineContainer))
            {
                _snakeSpawner.Spawn(_cubeStorage.GetStacks(), splineContainer, _game, out Snake snake);
                _beastSpawner.Spawn(snake, splineContainer);
                _slider.Init(snake);

                _detector.transform.position = road[1] + Vector3.up * snake.transform.localScale.y;
                _detector.gameObject.SetActive(true);
                _detector.EnableTrigger();
            }
        }
    }
}
