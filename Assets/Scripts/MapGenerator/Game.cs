using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public event Action Started;

    private void Start()
    {
        Started?.Invoke();        
    }
}
