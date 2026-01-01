namespace PottMaster.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public Exception? Exception { get; }
    
    private Result(bool isSuccess, T? value, string? error, Exception? exception)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Exception = exception;
    }
    
    public static Result<T> Success(T value) => new Result<T>(true, value, null, null);
    public static Result<T> Failure(string error) => new Result<T>(false, default, error, null);
    public static Result<T> Failure(string error, Exception exception) => new Result<T>(false, default, error, exception);
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public Exception? Exception { get; }
    
    private Result(bool isSuccess, string? error, Exception? exception)
    {
        IsSuccess = isSuccess;
        Error = error;
        Exception = exception;
    }
    
    public static Result Success() => new Result(true, null, null);
    public static Result Failure(string error) => new Result(false, error, null);
    public static Result Failure(string error, Exception exception) => new Result(false, error, exception);
}
