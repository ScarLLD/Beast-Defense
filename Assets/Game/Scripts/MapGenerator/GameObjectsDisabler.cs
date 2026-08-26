using UnityEngine;

namespace Game.Scripts.MapGenerator
{
    public class GameObjectsDisabler : MonoBehaviour
    {
        [SerializeField] private Game _game;
        [SerializeField] private GameObject _objectsParent;

        private void OnEnable()
        {
            _game.Leaved += DisableObjects;
        }

        private void OnDisable()
        {
            _game.Leaved -= DisableObjects;
        }

        public void EnableObjects()
        {
            _objectsParent.SetActive(true);
        }

        private void DisableObjects()
        {
            _objectsParent.SetActive(false);
        }
    }
}