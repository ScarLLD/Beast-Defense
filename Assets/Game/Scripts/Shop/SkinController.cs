using Game.Scripts.Shop.Skins;
using UnityEngine;

namespace Game.Scripts.Shop
{
    public class SkinController : MonoBehaviour
    {
        [SerializeField] private SkinData _skinData;

        public SkinData.Skin CurrentSkin =>
            _skinData.GetSkinById(CurrentSkinId);

        public string CurrentSkinId { get; private set; }

        public void Load(string savedSkinId)
        {
            if (!string.IsNullOrEmpty(savedSkinId) &&
                _skinData.GetSkinById(savedSkinId) != null)
            {
                CurrentSkinId = savedSkinId;
            }
            else
            {
                CurrentSkinId = _skinData.GetDefaultSkinId();
            }
        }

        public bool SetSkin(string skinId)
        {
            if (CurrentSkinId == skinId)
                return false;

            if (_skinData.GetSkinById(skinId) == null)
                return false;

            CurrentSkinId = skinId;

            return true;
        }

        public void Apply(Transform modelContainer, string modelName)
        {
            var skin = CurrentSkin;

            if (skin == null || !skin.Model)
                return;

            foreach (Transform child in modelContainer)
            {
                Destroy(child.gameObject);
            }

            var model = Instantiate(skin.Model, modelContainer);
            model.name = modelName;
        }
    }
}