using SharedKernel;

public class ApplicationErrorException : Exception
{
    public Error AppError { get; }
    public string Code { get; }
    public ErrorType Type { get; }

    public ApplicationErrorException(Error error) : base(error.Description)
    {
        AppError = error;
        Code = error.Code;
        Type = error.Type;
    }
}
