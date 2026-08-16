using UnityEngine;

#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG.Utils.Lang
{
    public class UtilsLang : MonoBehaviour
    {
        public static string UnauthorizedTextTranslate()
            => UnauthorizedTextTranslate(YG2.lang);

        public static string UnauthorizedTextTranslate(string language)
            => language switch
            {
                "Ru" => "неавторизованный",
                "En" => "unauthorized",
                "Tr" => "yetkisiz",
                "az" => "icazəsiz",
                "be" => "неаўтарызаваны",
                "et" => "loata",
                "fr" => "non autorisé",
                "kk" => "рұқсат етілмеген",
                "ky" => "уруксатсыз",
                "lt" => "neleistinas",
                "lv" => "neatļauts",
                "ro" => "neautorizat",
                "tg" => "беиҷозат",
                "tk" => "yetkisiz",
                "uk" => "несанкціонований",
                "uz" => "ruxsatsiz",
                "es" => "no autorizado",
                "pt" => "não autorizado",
                "id" => "tidak sah",
                "it" => "autorizzato",
                "de" => "nicht autorisiert",
                _ => "---"
            };

        public static string IsHiddenTextTranslate()
            => IsHiddenTextTranslate(YG2.lang);

        public static string IsHiddenTextTranslate(string language)
            => language switch
            {
                "Ru" => "скрыт",
                "En" => "is hidden",
                "Tr" => "gizli",
                "az" => "gizlidir",
                "be" => "схаваны",
                "et" => "on peidetud",
                "fr" => "est caché",
                "kk" => "жасырылған",
                "ky" => "жашыруун",
                "lt" => "yra paslėpta",
                "lv" => "ir paslēpts",
                "ro" => "este ascuns",
                "tg" => "пинҳон аст",
                "tk" => "gizlenendir",
                "uk" => "прихований",
                "uz" => "yashiringan",
                "es" => "está oculto",
                "pt" => "está escondido",
                "id" => "tersembunyi",
                "it" => "è nascosto",
                "de" => "ist versteckt",
                _ => "---"
            };
    }
}