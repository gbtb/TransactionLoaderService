using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace TransactionLoaderService.Core.TransactionStreamReaders;

public class XmlTransactionStreamReader: ITransactionStreamReader
{
    private XmlReader? _reader;
    private ILogger _logger;

    public XmlTransactionStreamReader(ILogger<XmlTransactionStreamReader> logger)
    {
        _logger = new StringListLogger(logger);
    }

    public void SetStream(Stream stream)
    {
        _reader = XmlReader.Create(stream);
    }

    public bool CanRead
    {
        get
        {
            try
            {
                if (_reader == null)
                    return false;
                
                if (!_reader.IsStartElement() || _reader.Name != "Transactions")
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public List<Transaction> ReadTransactions(out List<string> errors)
    {
        errors = new List<string>();
        try
        {
            var rootEl = XElement.Load(_reader); //todo: null check
            var transactionCounter = 0;
            foreach (var transactionEl in rootEl.Elements())
            {
                transactionCounter += 1;
                if (transactionEl is {Name.LocalName: "Transaction"} &&
                    transactionEl.Attribute("id")?.Value is { } transactionId)
                {
                    if (transactionId.Length != 50)
                        _logger.LogError("Transaction id is not valid. Should be a 50 character length string. Position: {TransactionPosition}. Value: {TransactionId}",
                            transactionCounter, transactionId);

                    if (transactionEl.Element("TransactionDate")?.Value is { } dateStr)
                }
            }
        }
        catch (Exception ex)
        {
            
        }
    }
}