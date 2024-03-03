namespace YoutubeLinks.Shared.Exceptions
{
    public abstract class CustomException : Exception
    {
        public ExceptionType Type { get; set; }
        public CustomException(string message) : base(message) { }
    }

    public class MyValidationException : CustomException
    {
        private readonly static string _errorMessage = "Validation Error";
        public Dictionary<string, List<string>> Errors { get; set; } = [];

        public MyValidationException(Dictionary<string, List<string>> errors) : base(_errorMessage)
        {
            Type = ExceptionType.Validation;
            Errors = errors;
        }

        public MyValidationException(string propertyName, string error) : base(_errorMessage)
        {
            Type = ExceptionType.Validation;
            Errors.Add(propertyName, [error]);
        }
    }

    public class MyServerException : CustomException
    {
        private readonly static string _errorMessage = "Server Error";
        public MyServerException() : base(_errorMessage)
        {
            Type = ExceptionType.Server;
        }
    }

    public class MyUnauthorizedException : CustomException
    {
        private readonly static string _errorMessage = "Unauthorized Error";
        public MyUnauthorizedException() : base(_errorMessage)
        {
            Type = ExceptionType.Unauthorized;
        }
    }

    public class MyForbiddenException : CustomException
    {
        private readonly static string _errorMessage = "Forbidden Error";
        public MyForbiddenException() : base(_errorMessage)
        {
            Type = ExceptionType.Forbidden;
        }
    }

    public class MyNotFoundException : CustomException
    {
        private readonly static string _errorMessage = "Not Found Error";

        public MyNotFoundException() : base(_errorMessage)
        {
            Type = ExceptionType.NotFound;
        }
    }
}
