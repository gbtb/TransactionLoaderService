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
    
    public async Task<Result> LoadFileAsync(Stream readStream, TransactionFileFormat fileFormatGuess, CancellationToken token)
    {
        List<Transaction>? transactions = null;
        
        foreach (var transactionStreamReader in _transactionStreamReaders.OrderByDescending(r => r.SupportedFormat == fileFormatGuess))
        {
            transactionStreamReader.SetStream(readStream);
            if (!transactionStreamReader.CanRead)
                continue;

            transactions = transactionStreamReader.ReadTransactions(out var errors);
            if (errors.Count > 0)
                return new Result(errors);
        }

        if (transactions == null)
            return Result.InvalidFileFormat;

        var saveResult = await _transactionRepository.SaveAsync(transactions, token);

        return saveResult;
    }
}