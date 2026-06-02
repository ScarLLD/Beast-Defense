using UnityEngine;
using YG;

public class LanguageInitializer : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        YG2.onCorrectLang += On—hangeLang;
    }

    private void Awake()
    {
        Debug.Log(YG2.lang);
    }

    private void OnDisable()
    {
        YG2.onCorrectLang -= On—hangeLang;
    }

    private static void On—hangeLang(string language)
    {
        YG2.SwitchLanguage("en");
    }
}
