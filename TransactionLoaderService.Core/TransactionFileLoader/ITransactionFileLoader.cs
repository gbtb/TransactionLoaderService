namespace TransactionLoaderService.Core.TransactionFileLoader;

/// <summary>
/// Loads transaction data from provided stream
/// </summary>
public interface ITransactionFileLoader
{
    Task<Result> LoadFileAsync(Stream readStream, TransactionFileFormat fileFormatGuess,
        CancellationToken token);
}