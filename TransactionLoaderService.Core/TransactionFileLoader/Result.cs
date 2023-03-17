namespace TransactionLoaderService.Core.TransactionFileLoader;

/// <summary>
/// Quick implementation of Result/Maybe pattern
/// </summary>
public class Result
{
    public static Result InvalidFileFormat = new Result(new []{ "Submitted file has invalid format" });
    public static Result Success = new Result(true, Array.Empty<string>());

    public Result(IReadOnlyCollection<string> errors)
    {
        IsSuccess = false;
        Errors = errors;
    }
    
    public Result(string error): this(new []{ error })
    {
    }

    private Result(bool isSuccess, string[] empty)
    {
        IsSuccess = isSuccess;
        Errors = empty;
    }

    public bool IsSuccess { get; }
    public IReadOnlyCollection<string> Errors { get; }
}