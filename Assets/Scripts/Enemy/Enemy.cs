using System;
using UnityEngine;

public class Enemy : MonoBehaviour, ITarget
{
    public void Init(Transform targetTransform)
    {
        throw new NotImplementedException();
    }

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
}
