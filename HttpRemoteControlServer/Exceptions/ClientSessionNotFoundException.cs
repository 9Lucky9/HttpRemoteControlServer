namespace HttpRemoteControlServer.Exceptions;

public sealed class ClientSessionNotFoundException : Exception
{
    public ClientSessionNotFoundException(string? message) : base(message)
    {
    }
}