using UnityEngine;

public class PlaceSpawner : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private ShootingPlace _placePrefab;
    [SerializeField] private float _distanceBetweenPlaces = 15f;
    [SerializeField] private int _placesCount;
    [SerializeField] private float _leftBound;
    [SerializeField] private float _rightBound;
    [SerializeField] private float _movingAwayFromShootingPlace;

    [SerializeField] private float _scaleMultiplier = 0.9f;

    public bool TryGeneratePlaces(Vector3 cubeScale, PlaceStorage storage)
    {
        _placePrefab.transform.localScale = new(cubeScale.x * _scaleMultiplier, 0.01f, cubeScale.z * _scaleMultiplier);
        Vector3 cameraCenter = new(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0f);

        Ray ray = _camera.ScreenPointToRay(cameraCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, 0);

            storage.Clear();
            GenerateShootingPlaces(hit.point, storage);
            GenerateEscapePlaces(storage);

            return true;
        }

        Debug.Log("Не удалось сгенерировать точки стрельбы.");
        return false;
    }

    private void GenerateShootingPlaces(Vector3 hit, PlaceStorage storage)
    {
        float placeWidth = _placePrefab.transform.localScale.x;
        float totalWidth = (_placesCount - 1) * (placeWidth + _distanceBetweenPlaces);

        Vector3 startPoint = hit;
        startPoint.x = hit.x - totalWidth / 2;
        startPoint.y = hit.y + 0.01f;

        for (int i = 0; i < _placesCount; i++)
        {
            Vector3 spawnPosition = startPoint;
            spawnPosition.x = startPoint.x + i * (placeWidth + _distanceBetweenPlaces);

            ShootingPlace place = Instantiate(_placePrefab, transform);
            place.transform.localPosition = spawnPosition;
            storage.PutPlace(place);
        }
    }

    private void GenerateEscapePlaces(PlaceStorage storage)
    {
        Vector3 escapePlace;

        if (storage.TryGetFirstPlace(out ShootingPlace firstPlace))
        {
            escapePlace = new Vector3(firstPlace.transform.position.x - _movingAwayFromShootingPlace,
                firstPlace.transform.position.y, firstPlace.transform.position.z + _movingAwayFromShootingPlace);

            storage.PutEscapePlace(escapePlace);
        }

        if (storage.TryGetLastPlace(out ShootingPlace lastPlace))
        {
            escapePlace = new Vector3(lastPlace.transform.position.x + _movingAwayFromShootingPlace,
                lastPlace.transform.position.y, lastPlace.transform.position.z + _movingAwayFromShootingPlace);

            storage.PutEscapePlace(escapePlace);
        }
    }
}
