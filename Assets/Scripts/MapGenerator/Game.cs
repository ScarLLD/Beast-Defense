using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public event Action Started;
    public event Action Over;

    private void Start()
    {
        Started?.Invoke();
        Debug.Log("Игра началась!");
    }

    public void AnnounceGameOver(string text)
    {
        Over?.Invoke();
        Debug.Log($"Игра окончена! {text}");
    }
}
