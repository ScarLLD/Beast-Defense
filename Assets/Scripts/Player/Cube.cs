using System;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Vector3 _defensePosition;
    private ObjectPool<Cube> _pool;

    public void Init(ObjectPool<Cube> pool, Vector3 defensePosition)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool), $"pool не может быть null.");

        if (defensePosition == null)
            throw new ArgumentNullException(nameof(defensePosition), $"defensePosition не может быть null.");

        _pool = pool;
        _defensePosition = defensePosition;
    }
}
