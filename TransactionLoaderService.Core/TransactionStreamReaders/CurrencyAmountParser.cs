using System.Globalization;

namespace TransactionLoaderService.Core.TransactionStreamReaders;

public static class CurrencyAmountParser
{
    private static CultureInfo StandardCulture = CultureInfo.CreateSpecificCulture("en_US");
    
    public static bool TryParse(string s, out decimal amount)
    {
        return decimal.TryParse(s, NumberStyles.Currency, StandardCulture, out amount);
    }
}