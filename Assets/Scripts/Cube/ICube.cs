using UnityEngine;

public interface ICube
{
    public void Init(Material material, int count) { }
    public int Count { get; }
    public Material Material { get; }
}
