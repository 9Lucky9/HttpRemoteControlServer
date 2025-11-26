namespace HttpRemoteControlServer.Exceptions;

public sealed class CommandNotFoundException : Exception
{
    public CommandNotFoundException(string? message) : base(message)
    {
    }
}