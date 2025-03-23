namespace Apiand.Extensions.Models;

public class Result
{
    public bool IsSuccess => Error == null;
    public Error? Error { get; protected init; }
    
    public static Result Succeed()
    {
        return new Result
        {
            Error = null
        };
    }
    
    public static Result Fail(Error error)
    {
        return new Result
        {
            Error = error
        };
    }
    
    public static implicit operator Result(Error error) => Fail(error);
}


public class Result<T> : Result
{
    public T? Data { get; private init; }
    
    public static Result<T> Succeed(T data)
    {
        return new Result<T>
        {
            Data = data,
            Error = null
        };
    }
    
    public static new Result<T> Fail(Error error)
    {
        return new Result<T>
        {
            Error = error
        };
    }
    
    public static implicit operator Result<T>(Error error) => Fail(error);
    public static implicit operator Result<T>(T data) => Succeed(data);
}