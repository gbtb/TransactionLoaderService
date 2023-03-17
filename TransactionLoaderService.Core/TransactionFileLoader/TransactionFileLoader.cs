using TransactionLoaderService.Core.TransactionStreamReaders;

namespace TransactionLoaderService.Core.TransactionFileLoader;

public class TransactionFileLoader: ITransactionFileLoader
{
    private readonly IEnumerable<ITransactionStreamReader> _transactionStreamReaders;
    private readonly ITransactionRepository _transactionRepository;

    public TransactionFileLoader(IEnumerable<ITransactionStreamReader> transactionStreamReaders, ITransactionRepository transactionRepository)
    {
        _transactionStreamReaders = transactionStreamReaders;
        _transactionRepository = transactionRepository;
    }
    
    public async Task<LoadFileResult> LoadFileAsync(Stream readStream, TransactionFileFormat fileFormatGuess, CancellationToken token)
    {
        List<Transaction>? transactions = null;
        
        foreach (var transactionStreamReader in _transactionStreamReaders.OrderByDescending(r => r.SupportedFormat == fileFormatGuess))
        {
            transactionStreamReader.SetStream(readStream);
            if (!transactionStreamReader.CanRead)
                continue;

            transactions = transactionStreamReader.ReadTransactions(out var errors);
            if (errors.Count > 0)
                return new LoadFileResult(false, errors);
        }

        if (transactions == null)
            return LoadFileResult.InvalidFileFormat;

        await _transactionRepository.SaveAsync(transactions, token); //todo: decide on error handling strategy
        
        return new LoadFileResult(true, new List<string>());
    }
}