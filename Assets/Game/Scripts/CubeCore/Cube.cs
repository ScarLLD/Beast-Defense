using Game.Scripts.SnakeCore;
using UnityEngine;

namespace Game.Scripts.CubeCore
{
    public class Cube : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private SnakeSegment _snakeSegment;

        public bool IsDestroyed { get; private set; } = false;

        public Material Material => _meshRenderer.material;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Init(Material material)
        {
            _meshRenderer.material = material;
            gameObject.SetActive(false);
        }

        public void InitSegment(SnakeSegment snakeSegment)
        {
            _snakeSegment = snakeSegment;
        }

        public void Hit()
        {
            if (IsDestroyed)
                return;

            Deactivate();
            IsDestroyed = true;
            _snakeSegment.NotifyDeath();

        }

        public void Deactivate()
        {
            if (!IsDestroyed)
                gameObject.SetActive(false);
        }
    }
}