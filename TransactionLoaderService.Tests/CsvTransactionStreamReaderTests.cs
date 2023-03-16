using System.Globalization;
using Microsoft.Extensions.Logging;
using Moq;
using TransactionLoaderService.Core.TransactionStreamReaders;

namespace TransactionLoaderService.Tests;

public class CsvTransactionStreamReaderTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void LoaderShouldReadGoodTransactions()
    {
        var logger = new Mock<ILogger<CsvTransactionStreamReader>>();
        var loader = new CsvTransactionStreamReader(logger.Object);

        using var stream = File.Open("GoodTransactions.csv", FileMode.Open, FileAccess.Read);
        
        loader.SetStream(stream);
        Assert.That(loader.CanRead, Is.True);
        Assert.That(loader.CanRead, Is.True);
        Assert.That(loader.CanRead, Is.True);

        var transactions = loader.ReadTransactions(out var errors);
        Assert.That(errors, Is.Empty);
        Assert.That(transactions, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void LoaderShouldDetectBadTransactionId()
    {
        var logger = new Mock<ILogger<CsvTransactionStreamReader>>();
        var loader = new CsvTransactionStreamReader(logger.Object);

        using var stream = File.Open("BadTransactions.csv", FileMode.Open, FileAccess.Read);
        
        loader.SetStream(stream);
        Assert.That(loader.CanRead, Is.True);

        var transactions = loader.ReadTransactions(out var errors);
        Assert.That(errors, Has.Count.EqualTo(2));
        Assert.That(transactions, Has.Count.EqualTo(0));
    }
}