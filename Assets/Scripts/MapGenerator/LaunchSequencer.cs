using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LaunchSequencer : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private SliderLevelViewer _levelViewer;
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
    [SerializeField] private Adv _adv;

    private Snake _snake;
    private Beast _beast;
    private SplineContainer _splineContainer;
    private Coroutine _advLevelCreationCoroutine;

    private void OnEnable()
    {
        _game.Started += OnGameStarted;
        _game.Continued += OnGameContinued;
        _game.Restarted += OnGameRestarted;
        _adv.Regenerated += OnAdvWatched;
    }

    private void OnDisable()
    {
        _game.Started -= OnGameStarted;
        _game.Continued -= OnGameContinued;
        _game.Restarted -= OnGameRestarted;
        _adv.Regenerated -= OnAdvWatched;

        if (_advLevelCreationCoroutine != null)
        {
            StopCoroutine(_advLevelCreationCoroutine);
            _advLevelCreationCoroutine = null;
        }
    }

    private void OnGameStarted()
    {
        _disabler.EnableObjects();

        if (_game.HasCompleted)
            StartNewLevel();
        else if (_game.HasStarted)
            ContinueCurrentLevel();
        else
            StartNewLevel();
    }

    private void OnGameContinued()
    {
        _disabler.EnableObjects();

        if (_game.HasCompleted)
            StartNewLevel();
        else
            RestartCurrentLevel();
    }

    private void OnGameRestarted()
    {
        _disabler.EnableObjects();
        RestartCurrentLevel();
    }

    private void OnAdvWatched()
    {
        if (_advLevelCreationCoroutine != null)
        {
            StopCoroutine(_advLevelCreationCoroutine);
        }

        _advLevelCreationCoroutine = StartCoroutine(AdvLevelCreationRoutine());
    }

    private IEnumerator AdvLevelCreationRoutine()
    {
        yield return StartNewLevelRoutine();

        _game.ContinueGame();

        _advLevelCreationCoroutine = null;
    }

    private void StartNewLevel()
    {
        StartCoroutine(StartNewLevelRoutine());
    }

    private void ContinueCurrentLevel()
    {
        StartCoroutine(ContinueCurrentLevelRoutine());
    }

    private void RestartCurrentLevel()
    {
        StartCoroutine(RestartCurrentLevelRoutine());
    }

    private IEnumerator StartNewLevelRoutine()
    {
        yield return StartCoroutine(CleanupRoutine());

        _gridCreator.Terminate();
        _cubeCreator.Terminate();

        if (TryGenerateLevel())
        {
            _levelViewer.DisplayText();
            InitializeGameplay();
        }
    }

    private IEnumerator ContinueCurrentLevelRoutine()
    {
        yield return StartCoroutine(RestartCurrentLevelRoutine());
    }

    private IEnumerator RestartCurrentLevelRoutine()
    {
        yield return StartCoroutine(CleanupRoutine());
        _placeSpawner.TryGeneratePlaces();
        _cubeCreator.Respawn();
        _availabilityManagement.UpdateAvailability();
        ResumeGameplay();
    }

    private IEnumerator CleanupRoutine()
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

    private bool TryGenerateLevel()
    {
        bool success = _gridCreator.TryCreate()
            && _placeSpawner.TryGeneratePlaces()
            && _cubeCreator.TryCreate(_cubeStorage, _bulletSpawner, _targetStorage)
            && _roadSpawner.TrySpawn(out List<Vector3> road)
            && _splineCreator.TryCreateSpline(road, out _splineContainer)
            && _splineVisualizer.TryGenerateRoadFromSpline(_splineContainer);

        if (success)
            _availabilityManagement.UpdateAvailability();

        return success;
    }

    private void InitializeGameplay()
    {
        SpawnCharacters();
        SetupDetector();
        StartGameplay();
    }

    private void ResumeGameplay()
    {
        _snake.CreateSegmentsFromStacks(_cubeStorage.GetStacks());
        StartGameplay();
    }

    private void SpawnCharacters()
    {
        _beast = _beastSpawner.Spawn();
        _snake = _snakeSpawner.Spawn(_cubeStorage.GetStacks(), _splineContainer, _deathModule, _beast);
        _slider.Init(_snake);
        _beast.Init(_snake.MoveSpeed, _splineContainer);
    }

    private void SetupDetector()
    {
        if (_roadSpawner.LastSpawnedRoad != null && _roadSpawner.LastSpawnedRoad.Count > 1)
        {
            _detector.transform.position = _roadSpawner.LastSpawnedRoad[1] + Vector3.up * _snake.transform.localScale.y;
            _detector.gameObject.SetActive(true);
            _detector.EnableTrigger();
        }
    }

    private void StartGameplay()
    {
        _snake.StartMove();
    }
}