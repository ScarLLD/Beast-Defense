using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShootingPlacesHolder : MonoBehaviour
{
    [SerializeField] private List<ShootingPlace> _shootingPlaces;

    private void Awake()
    {
        _shootingPlaces = new List<ShootingPlace>();
    }

    public bool TryGetPlace(out ShootingPlace place)
    {
        place = _shootingPlaces.Where(place => place.IsEmpty == false).FirstOrDefault();

        return place != null;
    }

    public void ReturnPlace(ShootingPlace place)
    {
        _shootingPlaces.Add(place);
    }
}
