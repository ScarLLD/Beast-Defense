using UnityEngine;

public static class UserUtils
{
    public static int GetIntRandomNumber(int minNumber, int maxNumber)
    {
        return Random.Range(minNumber, maxNumber);
    }
}
