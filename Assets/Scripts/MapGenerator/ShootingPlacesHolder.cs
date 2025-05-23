using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShootingPlacesHolder : MonoBehaviour
{
    private List<ShootingPlace> _shootingPlaces;

    private void Awake()
    {
        _shootingPlaces = new List<ShootingPlace>();

        FillPlaces();
    }

    public bool TryGetPlace(out ShootingPlace place)
    {
        Debug.Log("tryingGetPlace");

        place = _shootingPlaces.Where(place => place.IsEmpty == true).FirstOrDefault();

        if (place != null)
            place.ChangeEmptyStatus();

        return place != null;
    }

    public void PutPlace(ShootingPlace place)
    {
        _shootingPlaces.Add(place);
    }

    private void FillPlaces()
    {
        int childsCount = transform.childCount;

        for (int i = 0; i < childsCount; i++)
        {
            ShootingPlace place = transform.GetChild(i).GetComponent<ShootingPlace>();

            if (place != null)
                PutPlace(place);
        }
    }
}
