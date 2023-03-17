using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NMoneys;
using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Core.TransactionStreamReaders;

public class CsvTransactionStreamReader: ITransactionStreamReader
{
    //assuming that example csv format is the only acceptable one - meaning all values should be enclosed in quotes
    //also accept different quotes symbols (probably just pdf rendering feature) and spaces around commas
    //making one capturing group for content inside quotes, and non-capturing group for comma/EOL
    private readonly Regex _lineMatch = new Regex("[\"“]([^“\"”]*?)[\"”](?:[\\s]*,[\\s]*|$)", RegexOptions.Compiled);
    private Stream? _stream;
    private readonly StringListLogger _logger;

    public CsvTransactionStreamReader(ILogger<CsvTransactionStreamReader> logger)
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
                
                var reader = new StreamReader(_stream);
                var firstLine = reader.ReadLine();
                if (firstLine == null)
                    return false;

                var matches = _lineMatch.Matches(firstLine);
                
                if (matches.Count != 5)
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

    public TransactionFileFormat SupportedFormat => TransactionFileFormat.Csv;

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
            
            var reader = new StreamReader(_stream);

            var tranCounter = 0;
            while (reader.EndOfStream != true)
            {
                tranCounter += 1;
                if (reader.ReadLine() is not { } line || _lineMatch.Matches(line) is not { Count: 5 } matches)
                {
                    _logger.LogError(
                        "Line with transaction has incorrect format. Expected exactly 5 values. LineNumber: {LineNumber}",
                         tranCounter);
                    continue;
                }

                var tranId = GetValue(matches, 0);
                if (tranId.Length != 50)
                {
                    _logger.LogError(
                        "Transaction id is not valid. Should be a quoted string with 50 characters. Position: {TransactionPosition}. Value: {TransactionId}",
                        tranCounter, tranId);
                    continue;
                }

                if (!CurrencyAmountParser.TryParse(GetValue(matches, 1), out var amount))
                {
                    _logger.LogError(
                        "Transaction amount is not valid. Should be a quoted string with decimal value in a 2nd column. TransactionId: {TransactionId}. Value: {AmountStr}",
                        tranId, GetValue(matches, 1));
                    continue;
                }

                if (!Currency.Code.TryParse(GetValue(matches, 2), out var curCode))
                {
                    _logger.LogError(
                        "Transaction currency code is not valid. Should be a quoted string with ISO4217 Currency Code value. TransactionId: {TransactionId}. Value: {CurrencyCode}",
                        tranId, GetValue(matches, 2));
                    continue;
                }

                //format string and date from example are not compatible, so I chose to modify format str and made example xml parseable
                if (!DateTime.TryParseExact(GetValue(matches, 3), "dd/MM/yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var tranDate))
                {
                    _logger.LogError(
                        "Transaction date is not valid. Should be a quoted date string with value in dd/MM/yyyy HH:mm:ss format. TransactionId: {TransactionId}. Value: {TransactionDate}",
                        tranId, GetValue(matches, 3));
                    continue;
                }

                if (!TryParseTranStatus(GetValue(matches, 4), out var status))
                {
                    _logger.LogError(
                        "Transaction amount is not valid. Should be an <Status> element with status value. TransactionId: {TransactionId}. Value: {StatusStr}",
                        tranId, GetValue(matches, 4));
                    continue;
                }

                //suppressing nullability error with ??, because Currency.Code.TryParse is not annotated. 0 is unreachable
                var transaction = new Transaction(tranId, amount, curCode ?? 0, tranDate, status);
                transactions.Add(transaction);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception during reading of csv document");
        }

        errors = _logger.Errors;
        return transactions;
    }

    private static string GetValue(MatchCollection matches, int n)
    {
        return matches[n].Groups[1].Value;
    }
    
    private bool TryParseTranStatus(object statusStr, out TransactionStatus tranStatus)
    {
        tranStatus = statusStr switch
        {
            "Approved" => TransactionStatus.Approved,
            "Failed" => TransactionStatus.Rejected,
            "Finished" => TransactionStatus.Done,
            _ => TransactionStatus.Unknown
        };

        return tranStatus != TransactionStatus.Unknown;
    }
}