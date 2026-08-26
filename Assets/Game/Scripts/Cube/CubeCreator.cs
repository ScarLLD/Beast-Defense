using Game.Scripts.BulletCore;
using Game.Scripts.MapGenerator.Grid;
using Game.Scripts.MiniGameCore;
using Game.Scripts.Player;
using Game.Scripts.Road;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.CubeCore
{
    public class CubeCreator : MonoBehaviour
    {
        private readonly List<PlayerCube> _cubes = new();
        private readonly List<GridCell> _cells = new();

        [SerializeField] private GridStorage _gridStorage;
        [SerializeField] private PlayerCube _cubePrefab;
        [SerializeField] private DOTWeenAnimator _animator;

        private Transform _transform;

        [SerializeField]
        private List<Material> _colors = new();

        [SerializeField]
        private List<int> _counts = new();

        private void Awake()
        {
            _transform = transform;
        }

        public bool TryCreate(CubeStorage cubeStorage, BulletSpawner bulletSpawner, TargetStorage targetStorage)
        {
            int gridCount = _gridStorage.GridCount;

            if (gridCount <= 0)
                return false;

            cubeStorage.Clear();

            for (int i = 0; i < gridCount; i++)
            {
                int count = _counts[Random.Range(0, _counts.Count)];
                Material material = _colors[Random.Range(0, _colors.Count)];

                if (_gridStorage.TryGet(i, out GridCell gridCell) && gridCell.IsOccupied == false)
                {
                    Vector3 spawnPoint =
                        new(gridCell.transform.position.x,
                        gridCell.transform.position.y + _cubePrefab.transform.localScale.y / 2,
                        gridCell.transform.position.z);

                    PlayerCube playerCube = Instantiate(_cubePrefab, spawnPoint, Quaternion.identity, _transform);
                    playerCube.Init(gridCell, material, count, bulletSpawner, targetStorage);
                    playerCube.SetDefaultSettings();
                    _cubes.Add(playerCube);
                    cubeStorage.Add(playerCube);

                    gridCell.InitCube(playerCube);
                    _cells.Add(gridCell);
                }
            }

            return true;
        }

        public void Respawn()
        {
            foreach (PlayerCube playerCube in _cubes)
            {
                if (playerCube.isActiveAndEnabled == false)
                    playerCube.gameObject.SetActive(true);

                playerCube.SetDefaultSettings();
            }

            foreach (GridCell cell in _cells)
            {
                cell.SetDefaultSettings();
            }
        }

        public void Terminate()
        {
            foreach (var cube in _cubes)
            {
                Destroy(cube.gameObject);
            }

            _cubes.Clear();
        }
    }
}