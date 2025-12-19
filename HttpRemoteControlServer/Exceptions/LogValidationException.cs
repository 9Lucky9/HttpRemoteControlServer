namespace HttpRemoteControlServer.Exceptions;

public sealed class LogValidationException : Exception
{
    public LogValidationException(string? message) : base(message)
    {
    }
}