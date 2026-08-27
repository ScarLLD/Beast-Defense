using System;
using UnityEngine;

namespace Game.Scripts.Lifecycle
{
    public class GameTimer : MonoBehaviour
    {
        private float _startTime;
        private float _elapsedTime;

        public event Action<float> Stopped;

        public void StartTimer()
        {
            _startTime = Time.time;
            _elapsedTime = 0;
        }

        public void StopTimer(bool isVictory)
        {
            _elapsedTime = Time.time - _startTime;

            if (!isVictory) return;

            Stopped?.Invoke(_elapsedTime);
        }
    }
}