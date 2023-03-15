namespace TransactionLoaderService.Core;

public interface ITransactionFileLoader
{
    Task<LoadFileResult> LoadFileAsync(Stream readStream, TransactionFileFormat transactionFileFormatGuess,
        CancellationToken token);
}