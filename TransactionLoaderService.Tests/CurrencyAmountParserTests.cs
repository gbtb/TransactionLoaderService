using TransactionLoaderService.Core.TransactionStreamReaders;

namespace TransactionLoaderService.Tests;

[TestFixture]
public class CurrencyAmountParserTests
{
    [Test]
    public void CurrencyAmountParserShouldAcceptVariousFormats()
    {
        Assert.That(CurrencyAmountParser.TryParse("10000", out var amount1) && amount1 == 10000, Is.True);
        Assert.That(CurrencyAmountParser.TryParse("10000.00", out var amount2) && amount2 == 10000, Is.True);
        Assert.That(CurrencyAmountParser.TryParse("10000.0", out var amount3) && amount3 == 10000, Is.True);
        Assert.That(CurrencyAmountParser.TryParse("10,000.0", out var amount4) && amount4 == 10000, Is.True);
        Assert.That(CurrencyAmountParser.TryParse("10,000", out var amount5) && amount5 == 10000, Is.True);
    }
}