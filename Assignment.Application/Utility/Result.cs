namespace Assignment.Application.Utility;

public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public FailureType? FailureType { get; }

    protected Result(bool isSuccess, FailureType? type, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        FailureType = type;
    }

    public static Result Success() => new(true, default, string.Empty);
    public static Result Failure(FailureType type, string? errorMessage) => new(false, type, errorMessage);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, FailureType? type, string? errorMessage) : base(isSuccess, type, errorMessage)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, default, string.Empty);
    new public static Result<T> Failure(FailureType type, string? errorMessage) => new(false, default, type, errorMessage);
}

public enum FailureType
{
    NotFound,
    Unauthorized,
    DomainError
}