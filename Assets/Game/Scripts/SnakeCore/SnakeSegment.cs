using System;
using Game.Scripts.CubeCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.SnakeCore
{
    public class SnakeSegment : MonoBehaviour
    {
        [SerializeField] private List<Cube> _cubes;

        private int _currentCubeIndex;
        private bool _isDestroyed;
        private Snake _snake;

        private Material _material;
        public bool IsTarget { get; private set; }

        private void Awake()
        {
            IsTarget = false;
            _isDestroyed = false;
            _currentCubeIndex = 0;
        }

        public void Init(Material material, Snake snake)
        {
            _snake = snake;
            _material = material;
            _isDestroyed = false;

            foreach (var cube in _cubes)
            {
                cube.InitSegment(this);
                cube.Init(material);
                cube.Deactivate();
            }

            gameObject.SetActive(false);
        }

        public void SetIsTarget(bool isTarget)
        {
            IsTarget = isTarget;
        }

        public bool IsCurrentColor(Color color)
        {
            return _material && _material.color == color;
        }

        private void ActivateVisibleCubes()
        {
            foreach (var cube in _cubes)
            {
                cube.gameObject.SetActive(!cube.IsDestroyed);
            }
        }

        public bool TryGetCube(out Cube cube)
        {
            cube = null;

            if (_currentCubeIndex >= _cubes.Count) return false;

            cube = _cubes[_currentCubeIndex];
            _currentCubeIndex++;
            return true;
        }

        public void NotifyDeath()
        {
            if (!_snake)
                return;

            if (_isDestroyed)
            {
                return;
            }

            if (_cubes.Any(cube => !cube.IsDestroyed))
            {
                return;
            }

            _isDestroyed = true;

            _snake.DestroySegment(this);
            gameObject.SetActive(false);
        }

        public void SetActiveSegment(bool active)
        {
            if (_isDestroyed)
            {
                return;
            }

            gameObject.SetActive(active);

            if (active)
            {
                ActivateVisibleCubes();
            }
        }
    }
}