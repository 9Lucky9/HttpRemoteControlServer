namespace HttpRemoteControlServer.Exceptions;

public sealed class MonoEndpointException : Exception
{
    public MonoEndpointException(string? message) : base(message)
    {
    }
}