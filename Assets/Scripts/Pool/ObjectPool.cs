using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private T _prefab;
    private Transform _container;
    private List<T> _pool;

    public ObjectPool(T prefab, Transform container)
    {
        _prefab = prefab;
        _container = container;
        _pool = new List<T>();
    }

    private T CreateObject()
    {
        var tempObject = Object.Instantiate(_prefab, _container);
        _pool.Add(tempObject);

        return tempObject;
    }

    public T GetObject()
    {
        foreach (var obj in _pool)
        {
            if (obj.gameObject.activeInHierarchy == false)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        return CreateObject();
    }

    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
    }
}
