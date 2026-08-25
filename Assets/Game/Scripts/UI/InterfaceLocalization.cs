namespace Game.Scripts.UI
{
    public class InterfaceLocalization
    {
        public string GetLocalizedSnakeType(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Змея",
                "en" => "Snake",
                "tr" => "Yılan",
                _ => "Snake",
            };
        }

        public string GetLocalizedBeastType(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Зверь",
                "en" => "BeastCore",
                "tr" => "Canavar",
                _ => "BeastCore",
            };
        }

        public string GetLocalizedFreeText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Бесплатно",
                "en" => "Free",
                "tr" => "Ücret",
                _ => "Free",
            };
        }

        public string GetLocalizedPurchasedText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Куплено",
                "en" => "Purchased",
                "tr" => "Satın alındı",
                _ => "Purchased",
            };
        }

        public string GetLocalizedMoneyText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "Монет",
                "en" => "Money",
                "tr" => "Para",
                _ => "Money",
            };
        }

        public string GetLocalizedNoMoneyText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "НЕ ХВАТАЕТ МОНЕТ",
                "en" => "NOT ENOUGH MONEY",
                "tr" => "YETERLİ BOZUK PARA YOK",
                _ => "NOT ENOUGH MONEY",
            };
        }

        public string GetLocalizedBuyText(string languageCode)
        {
            return languageCode switch
            {
                "ru" => "КУПИТЬ",
                "en" => "BUY",
                "tr" => "ALMAK",
                _ => "BUY",
            };
        }

        public string GetLocalizedTakeText(string languageCode)
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