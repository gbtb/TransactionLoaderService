using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Core.TransactionStreamReaders;

public interface ITransactionStreamReader
{
    void SetStream(Stream stream);
    
    bool CanRead { get; }
    TransactionFileFormat SupportedFormat { get; }
    List<Transaction> ReadTransactions(out List<string> errors);
}