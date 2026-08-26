using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Pool
{
    public class ObjectPool<T> 
        where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly List<T> _pool;

        public ObjectPool(T prefab, Transform container)
        {
            _prefab = prefab;
            _container = container;
            _pool = new List<T>();
        }

        public T GetObject()
        {
            foreach (var tempObject in _pool.Where(tempObject => !tempObject.gameObject.activeInHierarchy))
            {
                tempObject.gameObject.SetActive(true);
                return tempObject;
            }

            return CreateObject();
        }
        
        private T CreateObject()
        {
            var tempObject = Object.Instantiate(_prefab, _container);
            _pool.Add(tempObject);

            return tempObject;
        }
    }
}