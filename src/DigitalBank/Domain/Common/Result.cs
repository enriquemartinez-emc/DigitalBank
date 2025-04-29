namespace DigitalBank.Domain.Common;

public record Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }
    protected Result(bool isSuccess, Error? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success() => new(true);
    public static Result<T> Success<T>(T value) => new(true, value);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Failure<T>(Error error) => new(false, default, error);
}

public record Result<T> : Result
{
    public T? Value { get; }
    internal Result(bool isSuccess, T? value = default, Error? error = null) : base(isSuccess, error)
    {
        Value = value;
    }
}
