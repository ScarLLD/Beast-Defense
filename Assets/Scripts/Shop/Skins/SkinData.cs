using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "Game/Skin Data")]
public class SkinData : ScriptableObject
{
    [System.Serializable]
    public class Skin
    {
        public string SkinId;
        public string SkinName;
        public int Price;
        public Sprite Icon;
        public GameObject Model;
        public bool IsDefault = false;
    }

    public List<Skin> Skins = new();

    public SkinData.Skin GetSkinById(string currentSkinId)
    {
        return Skins.Find(skin => skin.SkinId == currentSkinId);
    }

    public string GetDefaultSkinId()
    {
        foreach (var skin in Skins)
        {
            if (skin.IsDefault)
                return skin.SkinId;
        }

        if (Skins.Count > 0)
            return Skins[0].SkinId;

        return "";
    }
}