namespace TransactionLoaderService.Core.TransactionFileLoader;

public interface ITransactionFileLoader
{
    Task<Result> LoadFileAsync(Stream readStream, TransactionFileFormat fileFormatGuess,
        CancellationToken token);
}