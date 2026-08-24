using UnityEngine;
using YG;

namespace UI
{
    public class LanguageInitializer : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            YG2.onCorrectLang += OnСhangeLang;
        }

        private static void OnСhangeLang(string language)
        {
            YG2.SwitchLanguage(language);
        }

        private void OnDisable()
        {
            YG2.onCorrectLang -= OnСhangeLang;
        }
    }
}