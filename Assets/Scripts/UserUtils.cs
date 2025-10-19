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
}
