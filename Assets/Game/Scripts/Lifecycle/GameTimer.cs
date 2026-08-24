using System;
using UnityEngine;

namespace Game.Scripts.LifeCycle
{
    public class GameTimer : MonoBehaviour
    {
        private float startTime;
        private float elapsedTime = 0;

        public event Action<float> Stopped;

        public void StartTimer()
        {
            startTime = Time.time;
            elapsedTime = 0;
        }

        public void StopTimer(bool isVictory)
        {
            elapsedTime = Time.time - startTime;

            if (isVictory)
            {
                int minutes = Mathf.FloorToInt(elapsedTime / 60);
                float seconds = elapsedTime % 60;

                Stopped?.Invoke(elapsedTime);
            }
        }
    }
}