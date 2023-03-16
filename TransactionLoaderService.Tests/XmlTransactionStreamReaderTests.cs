using System.Globalization;
using Microsoft.Extensions.Logging;
using Moq;
using TransactionLoaderService.Core.TransactionStreamReaders;

namespace TransactionLoaderService.Tests;

public class XmlTransactionStreamReaderTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void LoaderShouldReadGoodTransactions()
    {
        var logger = new Mock<ILogger<XmlTransactionStreamReader>>();
        var loader = new XmlTransactionStreamReader(logger.Object);

        using var stream = File.Open("GoodTransactions.xml", FileMode.Open, FileAccess.Read);
        
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
    {;
        var logger = new Mock<ILogger<XmlTransactionStreamReader>>();
        var loader = new XmlTransactionStreamReader(logger.Object);


        using var stream = File.Open("BadTransactionIds.xml", FileMode.Open, FileAccess.Read);
        
        loader.SetStream(stream);
        Assert.That(loader.CanRead, Is.True);

        var transactions = loader.ReadTransactions(out var errors);
        Assert.That(errors, Has.Count.EqualTo(2));
        Assert.That(transactions, Has.Count.EqualTo(0));
    }

    [Test]
    public void LoadersShouldNotMutateStream()
    {
        var logger1 = new Mock<ILogger<XmlTransactionStreamReader>>();
        var logger2 = new Mock<ILogger<CsvTransactionStreamReader>>();
        var readerXml = new XmlTransactionStreamReader(logger1.Object);
        var readerCsv = new CsvTransactionStreamReader(logger2.Object);
        
        using var stream = File.Open("GoodTransactions.xml", FileMode.Open, FileAccess.Read);
        
        readerCsv.SetStream(stream);
        Assert.That(readerCsv.CanRead, Is.False);
        
        readerXml.SetStream(stream);
        Assert.That(readerXml.CanRead, Is.True);
        
        using var stream2 = File.Open("GoodTransactions.csv", FileMode.Open, FileAccess.Read);
        
        readerXml.SetStream(stream2);
        Assert.That(readerXml.CanRead, Is.False);
        
        readerCsv.SetStream(stream2);
        Assert.That(readerCsv.CanRead, Is.True);
    }
}