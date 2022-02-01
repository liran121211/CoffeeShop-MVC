using System.Text.RegularExpressions;

namespace CoffeeShop.Models
{
    public enum CardType
    {
        Unknown = 0,
        MasterCard = 1,
        VISA = 2,
        Amex = 3,
        Discover = 4,
        DinersClub = 5,
        JCB = 6,
        enRoute = 7
    }

    // Class to hold credit card type information
    public class CreditCard
    {
        private static CreditCard[] _cardTypeInfo = {
            new CreditCard("^(51|52|53|54|55)", 16, CardType.MasterCard),
            new CreditCard("^(4)", 16, CardType.VISA),
            new CreditCard("^(4)", 13, CardType.VISA),
            new CreditCard("^(34|37)", 15, CardType.Amex),
            new CreditCard("^(6011)", 16, CardType.Discover),
            new CreditCard("^(300|301|302|303|304|305|36|38)",14, CardType.DinersClub),
            new CreditCard("^(3)", 16, CardType.JCB),
            new CreditCard("^(2131|1800)", 15, CardType.JCB),
            new CreditCard("^(2014|2149)", 15, CardType.enRoute) };

        public CreditCard(string regEx, int length, CardType type)
        {
            RegEx = regEx;
            Length = length;
            Type = type;
        }

        public static CardType GetCardType(string cardNumber)
        {
            foreach (CreditCard info in _cardTypeInfo)
            {
                if (cardNumber.Length == info.Length &&
                    Regex.IsMatch(cardNumber, info.RegEx))
                    return info.Type;
            }
            return CardType.Unknown;
        }

        public static bool IsCardNumberValid(string cardNumber)
        {
            int i, checkSum = 0;

            // Compute checksum of every other digit starting from right-most digit
            for (i = cardNumber.Length - 1; i >= 0; i -= 2)
                checkSum += (cardNumber[i] - '0');

            // Now take digits not included in first checksum, multiple by two,
            // and compute checksum of resulting digits
            for (i = cardNumber.Length - 2; i >= 0; i -= 2)
            {
                int val = ((cardNumber[i] - '0') * 2);
                while (val > 0)
                {
                    checkSum += (val % 10);
                    val /= 10;
                }
            }

            // Number is valid if sum of both checksums MOD 10 equals 0
            return ((checkSum % 10) == 0);
        }

        public string RegEx { get; set; }
        public int Length { get; set; }
        public CardType Type { get; set; }
    }
}