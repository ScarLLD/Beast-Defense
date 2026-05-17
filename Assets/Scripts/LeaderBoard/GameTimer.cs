using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float startTime;
    private bool isRunning = false;
    private float elapsedTime = 0;

    public event Action<float> Stopped;

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
        elapsedTime = 0;
    }

    public void StopTimer(bool isVictory)
    {
        isRunning = false;
        elapsedTime = Time.time - startTime;

        if (isVictory)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            float seconds = elapsedTime % 60;

            Stopped?.Invoke(elapsedTime);
        }
    }
}
