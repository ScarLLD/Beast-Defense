using System;
using UnityEngine;

namespace Game.Scripts.LifeCycle
{
    public class GameTimer : MonoBehaviour
    {
        private float _startTime;
        private float _elapsedTime = 0;

        public event Action<float> Stopped;

        public void StartTimer()
        {
            _startTime = Time.time;
            _elapsedTime = 0;
        }

        public void StopTimer(bool isVictory)
        {
            _elapsedTime = Time.time - _startTime;

            if (isVictory)
            {
                int minutes = Mathf.FloorToInt(_elapsedTime / 60);
                float seconds = _elapsedTime % 60;

                Stopped?.Invoke(_elapsedTime);
            }
        }
    }
}