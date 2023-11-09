namespace WebApi.Shared;

public class Error
{
    public string Message { get; private set; }
    public string Code { get; private set; }

    public Error(string message, string code)
    {
        Message = message;
        Code = code;
    }
    public Error(string message)
    {
        Message = message;
        Code = string.Empty;
    }

}
