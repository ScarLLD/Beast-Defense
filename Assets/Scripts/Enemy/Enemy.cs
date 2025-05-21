using System;
using UnityEngine;

public class Enemy : MonoBehaviour, ITarget
{
    public bool IsDetected { get; private set; }
    public bool IsCaptured { get; private set; }


    public void ChangeCapturedStatus()
    {
        IsCaptured = !IsCaptured;
    }

    public void ChangeDetectedStatus()
    {
        IsDetected = !IsDetected;
    }

    internal void Init(ObjectPool<Enemy> pool, Transform targetTransform)
    {
        throw new NotImplementedException();
    }
}
