using UnityEngine;
using YG;

namespace Game.Scripts.UI
{
    public class LanguageInitializer : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            YG2.onCorrectLang += OnChangeLanguage;
        }

        private static void OnChangeLanguage(string language)
        {
            YG2.SwitchLanguage(language);
        }

        private void OnDisable()
        {
            YG2.onCorrectLang -= OnChangeLanguage;
        }
    }
}