namespace Apiand.Extensions.Models;

public class Result<T>
{
    public T? Data { get; private init; }
    public bool IsSuccess => Error == null;
    public Error? Error { get; private init; }
    
    public static Result<T> Succeed(T data)
    {
        return new Result<T>
        {
            Data = data,
            Error = null
        };
    }
    
    public static Result<T> Fail(Error error)
    {
        return new Result<T>
        {
            Error = error
        };
    }
    
    public static implicit operator Result<T>(Error error) => Fail(error);
    public static implicit operator Result<T>(T data) => Succeed(data);
}