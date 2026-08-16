using UnityEngine;
using Shop;

namespace UI
{
    public class InterfaceLocalization : MonoBehaviour
    {
        public string GetLocalizedSnakeType(string languageCode)
        {
            return languageCode switch
            {
                "Ru" => "Змея",
                "En" => "Snake",
                "Tr" => "Yılan",
                _ => "Snake",
            };
        }

        public string GetLocalizedBeastType(string languageCode)
        {
                return languageCode switch
                {
                    "Ru" => "Зверь",
                    "En" => "BeastCore",
                    "Tr" => "Canavar",
                    _ => "BeastCore",
                };
        }

        public string GetLocalizedFreeText(string languageCode)
        {
            return languageCode switch
            {
                "Ru" => "Бесплатно",
                "En" => "Free",
                "Tr" => "Ücret",
                _ => "Free",
            };
        }

        public string GetLocalizedPurchasedText(string languageCode)
        {
            return languageCode switch
            {
                "Ru" => "Куплено",
                "En" => "Purchased",
                "Tr" => "Satın alındı",
                _ => "Purchased",
            };
        }

        public string GetLocalizedMoneyText(string languageCode)
        {
            return languageCode switch
            {
                "Ru" => "Монет",
                "En" => "Money",
                "Tr" => "Para",
                _ => "Money",
            };
        }

        public string GetLocalizedNoMoneyText(string languageCode)
        {
            return languageCode switch
            {
                "Ru" => "НЕ ХВАТАЕТ МОНЕТ",
                "En" => "NOT ENOUGH MONEY",
                "Tr" => "YETERLİ BOZUK PARA YOK",
                _ => "NOT ENOUGH MONEY",
            };
        }

        public string GetLocalizedBuyText(string languageCode)
        {
            return languageCode switch
            {
                "Ru" => "КУПИТЬ",
                "En" => "BUY",
                "Tr" => "ALMAK",
                _ => "BUY",
            };
        }

        public string GetLocalizedTakeText(string languageCode)
        {
            return languageCode switch
            {
                "Ru" => "ВЫБРАТЬ",
                "En" => "CHOOSE",
                "Tr" => "SEÇMEK",
                _ => "CHOOSE",
            };
        }
    }
}