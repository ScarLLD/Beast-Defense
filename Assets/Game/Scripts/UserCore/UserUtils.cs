using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UserCore
{
    public static class UserUtils
    {
        public static int GetIntRandomNumber(int minNumber, int maxNumber)
        {
            return Random.Range(minNumber, maxNumber);
        }

        public static List<T> ShuffleList<T>(List<T> list)
        {
            var random = new System.Random();
            var count = list.Count;

            for (var i = count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }
    }
}