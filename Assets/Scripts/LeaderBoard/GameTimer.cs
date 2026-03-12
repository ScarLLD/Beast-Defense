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
        Debug.Log("Таймер запущен.");
    }

    public void StopTimer()
    {
        if (!isRunning)
        {
            Debug.LogWarning("Таймер не был запущен!");
        }

        isRunning = false;
        elapsedTime = Time.time - startTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = elapsedTime % 60;

        Debug.Log($"Уровень пройден за: {minutes} мин {seconds:F2} сек");
        Debug.Log($"Общее время в секундах: {elapsedTime:F3} сек");

        Stopped?.Invoke(elapsedTime);
    }
}
