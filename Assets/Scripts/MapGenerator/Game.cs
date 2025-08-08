using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public event Action Started;

    private void Start()
    {
        Started?.Invoke();        
    }
}
