using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "SkinData", menuName = "Game/Skin Data")]
    public class SkinData : ScriptableObject
    {   
        public List<Skin> Skins = new ();

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

            return string.Empty;
        }

        [Serializable]
        public class Skin
        {
            public string SkinId;

            [Serializable]
            public class LocalizedName
            {
                public string Ru;
                public string En;
                public string Tr;
            }

            public LocalizedName SkinNameTranslations;

            public int Price;
            public Sprite Icon;
            public GameObject Model;
            public Color Color;
            public bool IsDefault = false;

            public string GetLocalizedName(string languageCode)
            {
                return languageCode switch
                {
                    "Ru" => SkinNameTranslations.Ru,
                    "En" => SkinNameTranslations.En,
                    "Tr" => SkinNameTranslations.Tr,
                    _ => SkinNameTranslations.En,
                };
            }
        }
    }
}