using System;
using UnityEngine;

public class MiniGame : MonoBehaviour
{
    public event Action Started;
    public event Action Finished;


    public void OnStartButtonClick()
    {
        Started?.Invoke();
    }
}
