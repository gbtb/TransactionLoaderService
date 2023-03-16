using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using NMoneys;
using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Core.TransactionStreamReaders;

public class XmlTransactionStreamReader: ITransactionStreamReader
{
    private StringListLogger _logger;
    private Stream? _stream;

    public XmlTransactionStreamReader(ILogger<XmlTransactionStreamReader> logger)
    {
        _logger = new StringListLogger(logger);
    }

    public void SetStream(Stream stream)
    {
        _stream = stream;
    }

    public bool CanRead
    {
        get
        {
            try
            {
                if (_stream == null)
                    return false;
                
                var reader = XmlReader.Create(_stream);

                if (!reader.IsStartElement() || reader.Name != "Transactions")
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _stream?.Seek(0, SeekOrigin.Begin);
            }
        }
    }

    public TransactionFileFormat SupportedFormat => TransactionFileFormat.Xml;

    public List<Transaction> ReadTransactions(out List<string> errors)
    {
        errors = new List<string>();
        var transactions = new List<Transaction>();
        try
        {
            if (_stream == null)
            {
                _logger.LogError("Reader was not properly initialized");
                errors = _logger.Errors;
                return transactions;
            }
            
            var reader = XmlReader.Create(_stream);
            
            var rootEl = XElement.Load(reader);
            var tranCounter = 0;
            foreach (var tranEl in rootEl.Elements())
            {
                tranCounter += 1;
                if (tranEl is {Name.LocalName: "Transaction"} &&
                    tranEl.Attribute("id")?.Value is { } tranId)
                {
                    if (tranId.Length != 50)
                    {
                        _logger.LogError("Transaction id is not valid. Should be a string with 50 characters. Position: {TransactionPosition}. Value: {TransactionId}",
                            tranCounter, tranId);
                        continue;
                    }

                    //format string and date from example are not compatible, so I chose to modify format str and made example xml parseable
                    if (tranEl.Element("TransactionDate")?.Value is not { } dateStr
                        || !DateTime.TryParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal, out var tranDate))
                    {
                        _logger.LogError(
                            "Transaction date is not valid. Should be an <TransactionDate> element with value in yyyy-MM-ddTHH:mm:ss format. TransactionId: {TransactionId}. Value: {TransactionDate}",
                            tranId, tranEl.Element("TransactionDate")?.Value);
                        continue;
                    }

                    if (tranEl.Element("PaymentDetails") is not { } payDetailsEl)
                    {
                        _logger.LogError("Transaction payment details are not valid. Should be a <PaymentDetails> element with nested elements");
                        continue;
                    }
                    
                    if (!TryGetPaymentDetails(payDetailsEl, tranId, out var paymentDetails)) 
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
            _logger.LogError(ex, "Unexpected exception during reading of xml document");
        }

        errors = _logger.Errors;
        return transactions;
    }

    private bool TryGetPaymentDetails(XElement payDetailsEl, string tranId, [NotNullWhen(true)]out PaymentDetails? values)
    {
        values = null;

        if (payDetailsEl.Element("Amount")?.Value is not { } amountStr ||
            !CurrencyAmountParser.TryParse(amountStr, out var amount))
        {
            _logger.LogError(
                "Transaction amount is not valid. Should be an <Amount> element with decimal value. TransactionId: {TransactionId}. Value: {AmountStr}",
                tranId, payDetailsEl.Element("Amount")?.Value);
            return false;
        }

        if (payDetailsEl.Element("CurrencyCode")?.Value is not { } currencyCode ||
            !Currency.Code.TryParse(currencyCode, out var curCode))
        {
            _logger.LogError(
                "Transaction currency code is not valid. Should be an <CurrencyCode> element with ISO4217 Currency Code value. TransactionId: {TransactionId}. Value: {CurrencyCode}",
                tranId, payDetailsEl.Element("CurrencyCode")?.Value);
            return false;
        }
        
        values = new PaymentDetails(amount, curCode ?? 0); //suppressing nullability error with ??, because Currency.Code.TryParse is not annotated. 0 is unreachable

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