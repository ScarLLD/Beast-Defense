using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "Game/Skin Data")]
public class SkinData : ScriptableObject
{
    [System.Serializable]
    public class Skin
    {
        public string skinId;
        public string skinName;
        public int price;
        public Sprite icon;
        public GameObject model;
        public bool isDefault = false;
    }

    public List<Skin> skins = new();
}