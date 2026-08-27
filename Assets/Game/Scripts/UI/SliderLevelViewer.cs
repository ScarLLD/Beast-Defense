using TMPro;
using UnityEngine;
using YG;

namespace Game.Scripts.UI
{
    public class SliderLevelViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _sliderLevelViewer;

        private void Start()
        {
            DisplayText();
        }

        public void DisplayText()
        {
            _sliderLevelViewer.text = $"{YG2.saves.LevelNumber}";
        }
    }
}