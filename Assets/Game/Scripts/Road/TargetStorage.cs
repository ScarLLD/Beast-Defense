using Game.Scripts.SnakeCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Road
{
    public class TargetStorage : MonoBehaviour
    {
        private List<SnakeSegment> _segments;

        private void Awake()
        {
            _segments = new List<SnakeSegment>();
        }

        public void AddTarget(SnakeSegment segment)
        {
            _segments.Add(segment);
        }

        public bool TryGetTarget(Color color, out SnakeSegment snakeSegment)
        {
            snakeSegment = _segments.FirstOrDefault(segment => segment.IsCurrentColor(color) && segment.IsTarget == false);

            if (snakeSegment != null)
            {
                snakeSegment.SetIsTarget(true);
                return true;
            }

            return false;
        }

        public void Cleanup()
        {
            if (_segments != null && _segments.Count > 0)
            {
                foreach (var segment in _segments)
                {
                    segment.SetIsTarget(false);
                }

                _segments.Clear();
            }
        }
    }
}