namespace Organizer_przepisów_kulinarnych.BLL.Common
{
    public class Result
    {
        public bool Success { get; private set; }
        public string Error { get; private set; }
        public string? Message { get; private set; }

        public static Result Ok(string? message = null) => new Result { Success = true, Message = message };
        public static Result Fail(string error) => new Result { Success = false, Error = error };
    }
    public class Result<T>
    {
        public bool Success { get; private set; }
        public string Error { get; private set; }
        public string? Message { get; private set; }
        public T Data { get; private set; }

        public static Result<T> Ok(T data, string message = null) => new Result<T> { Success = true, Data = data, Message = message };
        public static Result<T> Fail(string error) => new Result<T> { Success = false, Error = error };
    }
}
