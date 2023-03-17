using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Core.TransactionStreamReaders;

/// <summary>
/// Interface for a reader of transactions data
/// </summary>
public interface ITransactionStreamReader
{
    /// <summary>
    /// Sets stream to read from
    /// </summary>
    /// <param name="stream">Stream with transactions</param>
    void SetStream(Stream stream);
    
    /// <summary>
    /// Can a reader read this stream?
    /// </summary>
    bool CanRead { get; }
    
    /// <summary>
    /// File format which this reader supports
    /// </summary>
    TransactionFileFormat SupportedFormat { get; }
    
    /// <summary>
    /// Performs reading and parsing transaction data from stream
    /// </summary>
    /// <param name="errors">List of errors happened during parsing</param>
    /// <returns>List of successfully parsed transactions</returns>
    List<Transaction> ReadTransactions(out List<string> errors);
}