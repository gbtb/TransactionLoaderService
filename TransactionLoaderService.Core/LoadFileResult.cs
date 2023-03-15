namespace TransactionLoaderService.Core;

public class LoadFileResult
{
    public LoadFileResult(bool isSuccess, IReadOnlyCollection<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public IReadOnlyCollection<string> Errors { get; }
}