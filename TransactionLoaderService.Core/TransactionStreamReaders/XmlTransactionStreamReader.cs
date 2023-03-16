using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using NMoneys;

namespace TransactionLoaderService.Core.TransactionStreamReaders;

public class XmlTransactionStreamReader: ITransactionStreamReader
{
    private XmlReader? _reader;
    private StringListLogger _logger;

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
            catch (Exception)
            {
                return false;
            }
        }
    }

    public List<Transaction> ReadTransactions(out List<string> errors)
    {
        errors = new List<string>();
        var transactions = new List<Transaction>();
        try
        {
            if (_reader == null)
            {
                _logger.LogError("{Reader} was not properly initialized", nameof(XmlTransactionStreamReader));
                errors = _logger.Errors;
                return transactions;
            }
            
            var rootEl = XElement.Load(_reader);
            var tranCounter = 0;
            foreach (var tranEl in rootEl.Elements())
            {
                tranCounter += 1;
                if (tranEl is {Name.LocalName: "Transaction"} &&
                    tranEl.Attribute("id")?.Value is { } tranId)
                {
                    if (tranId.Length != 50)
                    {
                        _logger.LogError("Transaction id is not valid. Should be a 50 character length string. Position: {TransactionPosition}. Value: {TransactionId}",
                            tranCounter, tranId);
                        continue;
                    }

                    //format string and date example are not compatible, so I chose to modify format so that example xml would be correct
                    if (tranEl.Element("TransactionDate")?.Value is not { } dateStr
                        || !DateTime.TryParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal, out var tranDate))
                    {
                        _logger.LogError(
                            "Transaction date is not valid. Should be an <TransactionDate> element with value in yyyy-MM-ddThh:mm:ss format. TransactionId: {TransactionId}. Value: {TransactionDate}",
                            tranId, tranEl.Element("TransactionDate")?.Value);
                        continue;
                    }

                    if (!TryGetPaymentDetails(tranEl, tranId, out var paymentDetails)) 
                        continue;
                    
                    if (tranEl.Element("Status")?.Value is not { } statusStr ||
                        !TryParseTranStatus(statusStr, out var status))
                    {
                        _logger.LogError(
                            "Transaction amount is not valid. Should be an <Status> element with status value. TransactionId: {TransactionId}. Value: {StatusStr}",
                            tranId, tranEl.Element("Status")?.Value);
                        continue;
                    }

                    var transaction = new Transaction(tranId, paymentDetails.Amount, paymentDetails.CurrencyCode, tranDate,
                        status);
                    transactions.Add(transaction);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception during loading of xml document");
        }

        errors = _logger.Errors;
        return transactions;
    }

    private bool TryGetPaymentDetails(XElement tranEl, string tranId, [NotNullWhen(true)]out PaymentDetails? values)
    {
        values = null;

        if (tranEl.Element("PaymentDetails") is not { } payDetailsEl)
        {
            return false;
        }
        
        
        if (payDetailsEl.Element("Amount")?.Value is not { } amountStr ||
            !decimal.TryParse(amountStr, out var amount))
        {
            _logger.LogError(
                "Transaction amount is not valid. Should be an <Amount> element with decimal value. TransactionId: {TransactionId}. Value: {AmountStr}",
                tranId, tranEl.Element("Amount")?.Value);
            return false;
        }

        if (payDetailsEl.Element("CurrencyCode")?.Value is not { } currencyCode ||
            !Currency.Code.TryParse(currencyCode, out var curCode))
        {
            _logger.LogError(
                "Transaction amount is not valid. Should be an <Status> element with status value. TransactionId: {TransactionId}. Value: {StatusStr}",
                tranId, tranEl.Element("Status")?.Value);
            return false;
        }
        
        values = new PaymentDetails(amount, curCode ?? 0); //suppressing nullability error, because Currency.Code.TryParse is not annotated. 0 is unreachable

        return true;
    }

    private record PaymentDetails(decimal Amount, CurrencyIsoCode CurrencyCode);
    
    private static bool TryParseTranStatus(string statusStr, out TransactionStatus tranStatus)
    {
        tranStatus = statusStr switch
        {
            "Approved" => TransactionStatus.Approved,
            "Rejected" => TransactionStatus.Rejected,
            "Done" => TransactionStatus.Done,
            _ => TransactionStatus.Unknown
        };

        return tranStatus != TransactionStatus.Unknown;
    }
}