namespace TransactionLoaderService.Core.TransactionFileLoader;

public interface ITransactionFileLoader
{
    Task<LoadFileResult> LoadFileAsync(Stream readStream, TransactionFileFormat transactionFileFormatGuess,
        CancellationToken token);
}