using Game.Scripts.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.CubeCore
{
    public class CubeStorage : MonoBehaviour
    {
        private readonly List<PlayerCube> _cubes = new();

        public void Add(PlayerCube cube)
        {
            _cubes.Add(cube);
        }

        public List<CubeStack> GetStacks()
        {
            return _cubes.Select(cube => cube.GetStack).ToList();
        }

        public void Clear()
        {
            _cubes.Clear();
        }
    }
}