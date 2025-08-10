using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceStorage : MonoBehaviour
{
    private List<ShootingPlace> _shootingPlaces;
    private List<Vector3> _escapePlaces;

    private void Awake()
    {
        _shootingPlaces = new List<ShootingPlace>();
        _escapePlaces = new List<Vector3>();
    }

    public bool TryGetPlace(PlayerCube cube, out ShootingPlace shootingPlace, out Vector3 escapePlace)
    {
        escapePlace = Vector3.zero;

        shootingPlace = _shootingPlaces
            .OrderBy(place => Vector3.Distance(place.transform.position, cube.transform.position))
            .FirstOrDefault(place => place.IsEmpty == true);

        if (shootingPlace != null)
        {
            shootingPlace.ChangeEmptyStatus(false);
            var tempShootingPlace = shootingPlace;

            escapePlace = _escapePlaces
                .OrderBy(place => Vector3.Distance(place, tempShootingPlace.transform.position))
                .FirstOrDefault();
        }

        return shootingPlace != null && escapePlace != null;
    }

    public void PutPlace(ShootingPlace place)
    {
        _shootingPlaces.Add(place);
    }

    public bool TryGetFirstPlace(out ShootingPlace place)
    {
        place = null;

        if (_shootingPlaces.Count > 0)
        {
            place = _shootingPlaces.First();
        }

        return place != null;
    }

    public bool TryGetLastPlace(out ShootingPlace place)
    {
        place = null;

        if (_shootingPlaces.Count > 0)
        {
            place = _shootingPlaces.Last();
        }

        return place != null;
    }

    public void PutEscapePlace(Vector3 escapePlace)
    {
        _escapePlaces.Add(escapePlace);
    }
}
