namespace CommunityBoard.Common
{
    public record ApiError(string Code, string Message, IDictionary<string, string[]>? Details = null);

    public class Result<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public ApiError? Error { get; init; }

        public static Result<T> Ok(T data) => new() { Success = true, Data = data };
        public static Result<T> Fail(string code, string message, IDictionary<string, string[]>? details = null)
            => new() { Success = false, Error = new ApiError(code, message, details) };
    }
    public class Result
    {
        public bool Success { get; init; }
        public ApiError? Error { get; init; }

        public static Result Ok() => new() { Success = true };
        public static Result Fail(string code, string message, IDictionary<string, string[]>? details = null)
            => new() { Success = false, Error = new ApiError(code, message, details) };
    }
}

