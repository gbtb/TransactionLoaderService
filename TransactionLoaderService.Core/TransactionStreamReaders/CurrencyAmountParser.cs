using System.Globalization;

namespace TransactionLoaderService.Core.TransactionStreamReaders;

public static class CurrencyAmountParser
{
    private static CultureInfo StandardCulture = CultureInfo.CreateSpecificCulture("en_US");
    
    /// <summary>
    /// Parses string with decimal amount, tolerates , as thousands delimeter and . as decimal separator
    /// </summary>
    /// <param name="s"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static bool TryParse(string s, out decimal amount)
    {
        return decimal.TryParse(s, NumberStyles.Currency, StandardCulture, out amount);
    }
}