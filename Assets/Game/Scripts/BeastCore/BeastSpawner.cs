using Game.Scripts.Shop;
using Game.Scripts.Shop.Skins;
using UnityEngine;
using YG;

namespace Game.Scripts.BeastCore
{
    public class BeastSpawner : MonoBehaviour
    {
        [SerializeField] private Beast _beastPrefab;
        [SerializeField] private SkinController _skinController;

        private Beast _beast;

        public SkinData.Skin GetCurrentSkin =>
            _skinController.CurrentSkin;

        private void Awake()
        {
            _skinController.Load(
                YG2.saves.EquippedBeastSkin);
        }

        public Beast Spawn()
        {
            if (_beast) return _beast;
            
            _beast = Instantiate(
                _beastPrefab,
                transform);

            _skinController.Apply(
                _beast.transform,
                "beastModel");

            return _beast;
        }

        public void UpdateSkin(string skinId)
        {
            if (!_skinController.SetSkin(skinId))
                return;

            if (_beast)
            {
                _skinController.Apply(
                    _beast.transform,
                    "beastModel");
            }

            YG2.saves.EquippedBeastSkin =
                _skinController.CurrentSkinId;

            YG2.SaveProgress();
        }
    }
}