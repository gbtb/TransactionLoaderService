namespace TransactionLoaderService.Core.TransactionStreamReaders;

public interface ITransactionStreamReader
{
    void SetStream(Stream stream);
    
    bool CanRead { get; }
    List<Transaction> ReadTransactions(out List<string> errors);
}