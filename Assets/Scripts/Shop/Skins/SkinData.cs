using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "Game/Skin Data")]
public class SkinData : ScriptableObject
{
    [Serializable]
    public class Skin
    {
        public string SkinId;

        [Serializable]
        public class LocalizedName
        {
            public string ru;
            public string en;
            public string tr;
           
        }
        public LocalizedName SkinNameTranslations;

        public int Price;
        public Sprite Icon;
        public GameObject Model;
        public Color Color;
        public bool IsDefault = false;

        public string GetLocalizedName(string languageCode)
        {
            switch (languageCode)
            {
                case "ru": 
                    return SkinNameTranslations.ru;
                case "en": 
                    return SkinNameTranslations.en;
                case "tr": 
                    return SkinNameTranslations.tr;
                default: 
                    return SkinNameTranslations.en;
            }
        }
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