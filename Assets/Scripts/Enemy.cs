using System.Collections;
using System.Collections.Generic;
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
}
