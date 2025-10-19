using System;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlaceSpawner : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Camera _camera;
    [SerializeField] private PlaceStorage _storage;
    [SerializeField] private ShootingPlace _placePrefab;
    [SerializeField] private float _distanceBetweenPlaces = 1;
    [SerializeField] private int _placesCount = 4;
    [SerializeField] private float _movingAwayFromShootingPlace = 10;
    [SerializeField] private float _scaleMultiplier = 0.9f;

    private int _initialPlacesCount;
    private Vector3 _spawnPoint;

    public bool PlacesIncreased { get; private set; }

    private void Awake()
    {
        _initialPlacesCount = _placesCount;
        PlacesIncreased = false;
    }

    private void OnEnable()
    {
        _game.Completed += SetDefaultSettings;
    }

    private void OnDisable()
    {
        _game.Completed -= SetDefaultSettings;
    }

    public bool TryGeneratePlaces(Vector3 cubeScale)
    {
        _placePrefab.transform.localScale = new(cubeScale.x * _scaleMultiplier, 0.01f, cubeScale.z * _scaleMultiplier);
        Vector3 cameraCenter = new(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0f);

        Ray ray = _camera.ScreenPointToRay(cameraCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            _spawnPoint = hit.point;
            transform.position = new Vector3(_spawnPoint.x, transform.position.y, 0);

            _storage.Clear();
            GenerateShootingPlaces();
            GenerateEscapePlaces();

            return true;
        }

        Debug.Log("Не удалось сгенерировать точки стрельбы.");
        return false;
    }

    public void IncreasePlace()
    {
        _placesCount++;
        PlacesIncreased = true;

        _storage.Clear();
        GenerateShootingPlaces();
        GenerateEscapePlaces();
    }

    public void SetDefaultSettings()
    {
        _placesCount = _initialPlacesCount;
        PlacesIncreased = false;
    }

    private void GenerateShootingPlaces()
    {
        float placeWidth = _placePrefab.transform.localScale.x;
        float totalWidth = (_placesCount - 1) * (placeWidth + _distanceBetweenPlaces);

        Vector3 startPoint = _spawnPoint;
        startPoint.x = _spawnPoint.x - totalWidth / 2;
        startPoint.y = _spawnPoint.y + 0.01f;

        for (int i = 0; i < _placesCount; i++)
        {
            Vector3 spawnPosition = startPoint;
            spawnPosition.x = startPoint.x + i * (placeWidth + _distanceBetweenPlaces);

            ShootingPlace place = Instantiate(_placePrefab, transform);
            place.transform.localPosition = spawnPosition;
            _storage.PutPlace(place);
        }
    }

    private void GenerateEscapePlaces()
    {
        Vector3 escapePlace;

        if (_storage.TryGetFirstPlace(out ShootingPlace firstPlace))
        {
            escapePlace = new Vector3(firstPlace.transform.position.x - _movingAwayFromShootingPlace,
                firstPlace.transform.position.y, firstPlace.transform.position.z + _movingAwayFromShootingPlace);

            _storage.PutEscapePlace(escapePlace);
        }

        if (_storage.TryGetLastPlace(out ShootingPlace lastPlace))
        {
            escapePlace = new Vector3(lastPlace.transform.position.x + _movingAwayFromShootingPlace,
                lastPlace.transform.position.y, lastPlace.transform.position.z + _movingAwayFromShootingPlace);

            _storage.PutEscapePlace(escapePlace);
        }
    }
}
