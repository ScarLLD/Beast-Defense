using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LaunchSequencer : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private GameObjectsDisabler _disabler;

    [Header("Spawners")]
    [SerializeField] private PlaceSpawner _placeSpawner;
    [SerializeField] private RoadSpawner _roadSpawner;
    [SerializeField] private SnakeSpawner _snakeSpawner;
    [SerializeField] private BeastSpawner _beastSpawner;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private GridCreator _gridCreator;
    [SerializeField] private SplineCreator _splineCreator;
    [SerializeField] private CubeCreator _cubeCreator;

    [Header("Storages")]
    [SerializeField] private PlaceStorage _placeStorage;
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private TargetStorage _targetStorage;

    [Header("Other dependencies")]
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private SplineVisualizer _splineVisualizer;
    [SerializeField] private TargetDetector _detector;
    [SerializeField] private SmoothBarSlider _slider;
    [SerializeField] private DeathModule _deathModule;
    [SerializeField] private AvailabilityManagement _availabilityManagement;

    private Snake _snake;
    private Beast _beast;
    private SplineContainer _splineContainer;

    private void OnEnable()
    {
        _game.Started += Launch;
        _game.Continued += Launch;
        _game.Restarted += Relaunch;
    }

    private void OnDisable()
    {
        _game.Started -= Launch;
        _game.Continued -= Launch;
        _game.Restarted -= Relaunch;
    }

    private void Launch()
    {
        _disabler.EnableObjects();

        if (_snake == null && _beast == null
            && _boundaryMaker.TryGeneratePathMarkers() && _gridCreator.TryCreate(out Vector3 cubeScale)
            && _placeSpawner.TryGeneratePlaces(cubeScale, _placeStorage)
            && _cubeCreator.TryCreate(_boundaryMaker, _cubeStorage, _bulletSpawner, _targetStorage))
        {
            _availabilityManagement.UpdateAvailability();

            if (_roadSpawner.TrySpawn(out List<Vector3> road)
                && _splineCreator.TryCreateSpline(road, out _splineContainer)
                && _splineVisualizer.TryGenerateRoadFromSpline(_splineContainer))
            {
                _beast = _beastSpawner.Spawn();
                _snake = _snakeSpawner.Spawn(_cubeStorage.GetStacks(), _splineContainer, _deathModule, _beast);
                _slider.Init(_snake);

                _beast.Init(_snake.MoveSpeed, _splineContainer);

                _detector.transform.position = road[1] + Vector3.up * _snake.transform.localScale.y;
                _detector.gameObject.SetActive(true);
                _detector.EnableTrigger();
            }
        }
        else
        {
            Relaunch();
        }
    }

    private void Relaunch()
    {
        _disabler.EnableObjects();

        if (_snake != null && _beast != null)
        {
            StartCoroutine(RelaunchRoutine());
        }
    }

    private IEnumerator RelaunchRoutine()
    {
        yield return StartCoroutine(_snake.GetBackToStart());
        _bulletSpawner.Cleanup();
        _placeStorage.SetDefaultSettings();
        _targetStorage.Cleanup();
        _snake.SetDefaultSetting();
        _beast.SetDefaultSettings();
        _cubeCreator.Respawn();
        _availabilityManagement.UpdateAvailability();
        _snake.CreateSegmentsFromStacks(_cubeStorage.GetStacks());
        _snake.StartMove();
    }
}
