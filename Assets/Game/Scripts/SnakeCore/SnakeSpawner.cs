using Game.Scripts.BeastCore;
using Game.Scripts.CubeCore;
using Game.Scripts.LifeCycle;
using Game.Scripts.Road;
using Game.Scripts.Shop;
using Game.Scripts.Shop.Skins;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using YG;

namespace Game.Scripts.SnakeCore
{
    public class SnakeSpawner : MonoBehaviour
    {
        [SerializeField] private Snake _snakePrefab;
        [SerializeField] private SkinController _skinController;
        [SerializeField] private TargetStorage _targetStorage;

        private Snake _snake;
        private Transform _transform;

        public SkinData.Skin GetCurrentSkin =>
            _skinController.CurrentSkin;

        private void Awake()
        {
            _transform = transform;

            _skinController.Load(
                YG2.saves.EquippedSnakeSkin);
        }

        public Snake Spawn(
            List<CubeStack> stacks,
            SplineContainer splineContainer,
            DeathModule deathModule,
            Beast beast)
        {
            if (!_snake)
            {
                _snake = Instantiate(_snakePrefab, _transform);

                _skinController.Apply(
                    _snake.ModelContainer,
                    "snakeModel");
            }

            _snake.InitializeSnake(
                stacks,
                splineContainer,
                deathModule,
                beast);

            return _snake;
        }

        public void UpdateSkin(string skinId)
        {
            if (!_skinController.SetSkin(skinId))
                return;

            _skinController.Apply(
                _snake.ModelContainer,
                "snakeModel");

            YG2.saves.EquippedSnakeSkin =
                _skinController.CurrentSkinId;

            YG2.SaveProgress();
        }
    }
}