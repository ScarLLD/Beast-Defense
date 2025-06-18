using System.Collections.Generic;
using UnityEngine;

public class ColorsHolder : MonoBehaviour
{
    [SerializeField]
    private IReadOnlyList<Color> availableColors = new List<Color>()
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta
    };

    public Color GetRandomColor() => availableColors[Random.Range(0, availableColors.Count)];
}
