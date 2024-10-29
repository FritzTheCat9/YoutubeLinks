namespace YoutubeLinks.Shared.Exceptions;

public abstract class CustomException(string message) : Exception(message)
{
    public ExceptionType Type { get; protected init; }
}

public class MyValidationException : CustomException
{
    private const string ErrorMessage = "Validation Error";

    public MyValidationException(Dictionary<string, List<string>> errors) : base(ErrorMessage)
    {
        Type = ExceptionType.Validation;
        Errors = errors;
    }

    public MyValidationException(string propertyName, string error) : base(ErrorMessage)
    {
        Type = ExceptionType.Validation;
        Errors.Add(propertyName, [error]);
    }

    public Dictionary<string, List<string>> Errors { get; set; } = [];
}

public class MyServerException : CustomException
{
    private const string ErrorMessage = "Server Error";

    public MyServerException() : base(ErrorMessage)
    {
        Type = ExceptionType.Server;
    }
}

public class MyUnauthorizedException : CustomException
{
    private const string ErrorMessage = "Unauthorized Error";

    public MyUnauthorizedException() : base(ErrorMessage)
    {
        Type = ExceptionType.Unauthorized;
    }
}

public class MyForbiddenException : CustomException
{
    private const string ErrorMessage = "Forbidden Error";

    public MyForbiddenException() : base(ErrorMessage)
    {
        Type = ExceptionType.Forbidden;
    }
}

public class MyNotFoundException : CustomException
{
    private const string ErrorMessage = "Not Found Error";

    public MyNotFoundException() : base(ErrorMessage)
    {
        Type = ExceptionType.NotFound;
    }
}