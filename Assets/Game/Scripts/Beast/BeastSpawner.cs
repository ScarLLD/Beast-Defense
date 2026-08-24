using Shop;
using UnityEngine;
using YG;

namespace BeastCore
{
    public class BeastSpawner : MonoBehaviour
    {
        [SerializeField] private Beast _beastPrefab;
        [SerializeField] private SkinData _skinData;

        private Beast _beast;
        private string _currentSkinId;

        public SkinData.Skin GetCurrentSkin => _skinData.GetSkinById(_currentSkinId);

        private void Start()
        {
            LoadCurrentSkin();
        }

        private void LoadCurrentSkin()
        {
            string savedSkinId = YG2.saves.EquippedBeastSkin;
            bool isSkinMissing = string.IsNullOrEmpty(savedSkinId) || _skinData.GetSkinById(savedSkinId) == null;
            _currentSkinId = isSkinMissing ? _skinData.GetDefaultSkinId() : savedSkinId;
        }

        public Beast Spawn()
        {
            if (_beast == null)
                _beast = Instantiate(_beastPrefab, transform);

            ApplyCurrentSkin();

            return _beast;
        }

        public void UpdateSkin(string skinId)
        {
            if (_currentSkinId == skinId)
                return;

            _currentSkinId = skinId;

            if (_beast != null)
            {
                ApplyCurrentSkin();
            }

            YG2.saves.EquippedBeastSkin = _currentSkinId;
            YG2.SaveProgress();
        }

        private void ApplyCurrentSkin()
        {
            var skin = _skinData.GetSkinById(_currentSkinId);

            if (skin != null && skin.Model != null)
            {
                ApplySkinModel(skin.Model);
            }
        }

        private void ApplySkinModel(GameObject skinModelPrefab)
        {
            foreach (Transform child in _beast.transform)
            {
                Destroy(child.gameObject);
            }

            var model = Instantiate(skinModelPrefab, _beast.transform);
            model.name = "beastModel";
        }
    }
}