using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
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

    public T CreateObject()
    {
        var tempObject = Object.Instantiate(_prefab, _container);
        _pool.Add(tempObject);

        return tempObject;
    }

    public T GetObject()
    {
        foreach (var tempObject in _pool)
        {
            if (tempObject.gameObject.activeInHierarchy == false)
            {
                tempObject.gameObject.SetActive(true);
                return tempObject;
            }
        }

        return CreateObject();
    }
}
