namespace LMS.Application.Common.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }
    public AppException(string message, int statusCode = 400)
        : base(message) => StatusCode = statusCode;
}

public class NotFoundException : AppException
{
    public NotFoundException(string name, object key)
        : base($"{name} with id '{key}' was not found.", 404) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized")
        : base(message, 401) { }
}