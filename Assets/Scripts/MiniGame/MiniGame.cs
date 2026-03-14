using System;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
{    
    public event Action Started;

    public void OnStartButtonClick()
    {
        Started?.Invoke();
    }
}
