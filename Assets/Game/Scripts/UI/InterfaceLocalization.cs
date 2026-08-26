namespace Game.Scripts.UI
{
    public class InterfaceLocalization
    {
        public static string GetLocalizedSnakeType(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Змея",
                "en" => "Snake",
                "tr" => "Yılan",
                _ => "Snake",
            };
        }

        public static string GetLocalizedBeastType(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Зверь",
                "en" => "BeastCore",
                "tr" => "Canavar",
                _ => "BeastCore",
            };
        }

        public static string GetLocalizedFreeText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Бесплатно",
                "en" => "Free",
                "tr" => "Ücret",
                _ => "Free",
            };
        }

        public static string GetLocalizedPurchasedText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Куплено",
                "en" => "Purchased",
                "tr" => "Satın alındı",
                _ => "Purchased",
            };
        }

        public static string GetLocalizedMoneyText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Монет",
                "en" => "Money",
                "tr" => "Para",
                _ => "Money",
            };
        }

        public static string GetLocalizedNoMoneyText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "НЕ ХВАТАЕТ МОНЕТ",
                "en" => "NOT ENOUGH MONEY",
                "tr" => "YETERLİ BOZUK PARA YOK",
                _ => "NOT ENOUGH MONEY",
            };
        }

        public static string GetLocalizedBuyText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "КУПИТЬ",
                "en" => "BUY",
                "tr" => "ALMAK",
                _ => "BUY",
            };
        }

        public static string GetLocalizedTakeText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "ВЫБРАТЬ",
                "en" => "CHOOSE",
                "tr" => "SEÇMEK",
                _ => "CHOOSE",
            };
        }
    }
}