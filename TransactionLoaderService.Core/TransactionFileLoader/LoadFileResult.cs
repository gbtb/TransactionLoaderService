namespace TransactionLoaderService.Core.TransactionFileLoader;

public class LoadFileResult
{
    public static LoadFileResult InvalidFileFormat = new LoadFileResult(false, new []{ "Submitted file has invalid format" });

    public LoadFileResult(bool isSuccess, IReadOnlyCollection<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public IReadOnlyCollection<string> Errors { get; }
}