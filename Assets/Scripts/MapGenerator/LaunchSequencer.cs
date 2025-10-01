using System.Collections;
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

    private Snake _snake;
    private Beast _beast;
    private SplineContainer _spline;

    private void OnEnable()
    {
        _game.Started += Launch;
        _game.Restarted += Relaunch;
    }

    private void OnDisable()
    {
        _game.Started -= Launch;
        _game.Restarted -= Relaunch;
    }

    private void Launch()
    {
        if (_boundaryMaker.TryGeneratePathMarkers() && _gridCreator.TryCreate(out Vector3 cubeScale)
            && _placeSpawner.TryGeneratePlaces(cubeScale) && _cubeCreator.TryCreate())
        {
            _availabilityManagement.UpdateAvailability();

            if (_roadSpawner.TrySpawn(out List<Vector3> road)
                && _splineCreator.TryCreateSpline(road, out _spline)
                && _splineRoad.TryGenerateRoadFromSpline(_spline))
            {
                _beast = _beastSpawner.Spawn();
                _snake = _snakeSpawner.Spawn(_cubeStorage.GetStacks(), _spline, _beast, _game);
                _slider.Init(_snake);

                _beast.Init(_snake.MoveSpeed, _spline);

                _detector.transform.position = road[1] + Vector3.up * _snake.transform.localScale.y;
                _detector.gameObject.SetActive(true);
                _detector.EnableTrigger();
            }
        }
    }

    private void Relaunch()
    {
        if (_snake != null && _beast != null)
        {
            StartCoroutine(RelaunchRoutine());
        }
    }

    private IEnumerator RelaunchRoutine()
    {
        yield return StartCoroutine(_snake.GetBackToStart());
        _snake.SetDefaultSetting();
        _beast.SetDefaultSettings(_spline);
        _snake.StartMove();
    }
}
