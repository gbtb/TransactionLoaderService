namespace TransactionLoaderService.Core;

public class TransactionFileLoader: ITransactionFileLoader
{
    public TransactionFileLoader(ICsvTransactionStreamReader csvTransactionStreamReader,
        IXmlTransactionStreamReader xmlTransactionStreamReader)
    {
        
    }
    
    public async Task<LoadFileResult> LoadFileAsync(Stream readStream, TransactionFileFormat transactionFileFormatGuess, CancellationToken token)
    {
        
        
        return new LoadFileResult(true, new List<string>());
    }
}

public interface IXmlTransactionStreamReader: ITransactionStreamReader
{
    
}

public interface ICsvTransactionStreamReader: ITransactionStreamReader
{
}

public interface ITransactionStreamReader
{
    bool CanRead { get; }
}