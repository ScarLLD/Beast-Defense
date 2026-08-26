using Game.Scripts.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.MapGenerator
{
    public class PlaceStorage : MonoBehaviour
    {
        [SerializeField] private List<Vector3> _escapePlaces;

        private List<ShootingPlace> _shootingPlaces;

        private void Awake()
        {
            _shootingPlaces = new List<ShootingPlace>();
        }

        public void Clear()
        {
            foreach (var place in _shootingPlaces)
            {
                Destroy(place.gameObject);
            }

            _shootingPlaces.Clear();
        }

        public bool TryGetPlace(PlayerCube cube, out ShootingPlace shootingPlace, out Vector3 escapePlace)
        {
            escapePlace = Vector3.zero;

            shootingPlace = _shootingPlaces
                .OrderBy(place => Vector3.Distance(place.transform.position, cube.transform.position))
                .FirstOrDefault(place => place.IsEmpty == true);

            if (!shootingPlace) return shootingPlace && escapePlace != null;
            {
                shootingPlace.ChangeEmptyStatus(false);
                var tempShootingPlace = shootingPlace;

                escapePlace = _escapePlaces
                    .OrderBy(place => Vector3.Distance(place, tempShootingPlace.transform.position))
                    .FirstOrDefault();
            }

            return shootingPlace && escapePlace != null;
        }

        public void PutPlace(ShootingPlace place)
        {
            _shootingPlaces.Add(place);
        }

        public void SetDefaultSettings()
        {
            if (_shootingPlaces.Count <= 0) return;
            
            foreach (var place in _shootingPlaces)
            {
                place.ChangeEmptyStatus(true);
            }
        }
    }
}