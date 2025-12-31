using HttpRemoteControlServer.Contracts;

namespace HttpRemoteControlServer.Services;

public sealed class SessionNotifier : ISessionNotifier
{
    public event Action? NewSessionOpened;
    public event Action? NewCommandResult;

    public void NotifyNewSessionOpened()
    {
        NewSessionOpened?.Invoke();
    }

    public void NotifyNewCommandResult()
    {
        NewCommandResult?.Invoke();
    }
}