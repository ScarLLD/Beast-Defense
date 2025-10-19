using System;
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
        _game.Started += FirstLaunch;
        _game.Continued += ContinueLaunch;
        _game.Restarted += Relaunch;
        _game.Completed += TerminateGameObjects;
    }

    private void OnDisable()
    {
        _game.Started -= FirstLaunch;
        _game.Continued -= ContinueLaunch;
        _game.Restarted -= Relaunch;
        _game.Completed -= TerminateGameObjects;
    }

    private void FirstLaunch()
    {
        _disabler.EnableObjects();

        if (_game.HasStarted == false)
            Launch();
        else
            ContinueLaunch();
    }

    private void ContinueLaunch()
    {
        _disabler.EnableObjects();

        Debug.Log("Continue Launch");

        if (_game.HasCompleted)
            Launch();
        else
            Relaunch();
    }

    private void TryGenerateGame()
    {
        if (_boundaryMaker.TryGeneratePathMarkers() && _gridCreator.TryCreate(out Vector3 cubeScale)
                    && _placeSpawner.TryGeneratePlaces(cubeScale)
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
    }

    private void Relaunch()
    {
        _disabler.EnableObjects();

        if (_snake != null && _beast != null)
        {
            StartCoroutine(RelaunchRoutine());
        }
    }

    private void Launch()
    {
        _disabler.EnableObjects();

        StartCoroutine(LaunchRoutine());
    }

    private IEnumerator ClearRoutine()
    {
        if (_snake != null && _beast != null)
        {
            yield return StartCoroutine(_snake.GetBackToStart());
            _snake.SetDefaultSetting();
            _beast.SetDefaultSettings();
        }

        _bulletSpawner.Cleanup();
        _placeStorage.SetDefaultSettings();
        _targetStorage.Cleanup();
    }

    private IEnumerator LaunchRoutine()
    {
        yield return StartCoroutine(ClearRoutine());
        TryGenerateGame();
    }

    private IEnumerator RelaunchRoutine()
    {
        yield return StartCoroutine(ClearRoutine());
        _cubeCreator.Respawn();
        _availabilityManagement.UpdateAvailability();
        _snake.CreateSegmentsFromStacks(_cubeStorage.GetStacks());
        _snake.StartMove();
    }

    private void TerminateGameObjects()
    {
        Destroy(_snake.gameObject);
        _snake = null;

        Destroy(_beast.gameObject);
        _beast = null;

        _gridCreator.Terminate();
        _cubeCreator.Terminate();
    }
}
