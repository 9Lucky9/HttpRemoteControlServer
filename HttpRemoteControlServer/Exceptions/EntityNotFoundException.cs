namespace HttpRemoteControlServer.Exceptions;

public sealed class EntityNotFoundException<T> : Exception
{
    public EntityNotFoundException(string? message) : base(message)
    {
    }
}