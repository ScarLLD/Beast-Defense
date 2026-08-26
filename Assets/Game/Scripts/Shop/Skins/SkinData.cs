using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Shop.Skins
{
    [CreateAssetMenu(fileName = "SkinData", menuName = "Game/Skin Skins")]
    public class SkinData : ScriptableObject
    {
        public List<Skin> Skins = new();

        public Skin GetSkinById(string currentSkinId)
        {
            return Skins.Find(skin => skin.SkinId == currentSkinId);
        }

        public string GetDefaultSkinId()
        {
            foreach (var skin in Skins.Where(skin => skin.IsDefault))
            {
                return skin.SkinId;
            }

            return Skins.Count > 0 ? Skins[0].SkinId : string.Empty;
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