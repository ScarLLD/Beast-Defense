using System.Collections.Generic;
using UnityEngine;

public static class UserUtils
{
    public static int GetIntRandomNumber(int minNumber, int maxNumber)
    {
        return Random.Range(minNumber, maxNumber);
    }

    public static float GetFloatRandomNumber(float minNumber, float maxNumber)
    {
        return Random.Range(minNumber, maxNumber);
    }

    public static List<Vector3> GetRaisedRoad(List<Vector3> road, float halfObjectSize)
    {
        List<Vector3> RaisedRoad = new();

        for (int i = 0; i < road.Count; i++)
        {
            RaisedRoad.Add(new Vector3(road[i].x, road[i].y + halfObjectSize, road[i].z));
        }

        return RaisedRoad;
    }
}
