namespace StockMarketLive.Domain.Common;

/// <summary>
/// Projenin temel Result sarmalayıcısı. Başarılı veya başarısız işlemleri Exception fırlatmadan döndürmek için kullanılır.
/// (Clean Architecture kuralı).
/// </summary>
public sealed record Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public sealed record Result<T>
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
