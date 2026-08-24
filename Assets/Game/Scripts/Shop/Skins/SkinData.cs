using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Shop.Skins
{
    [CreateAssetMenu(fileName = "SkinData", menuName = "Game/Skin Skins")]
    public class SkinData : ScriptableObject
    {
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
                    "ru" => SkinNameTranslations.Ru,
                    "en" => SkinNameTranslations.En,
                    "tr" => SkinNameTranslations.Tr,
                    _ => SkinNameTranslations.En,
                };
            }
        }
    }
}