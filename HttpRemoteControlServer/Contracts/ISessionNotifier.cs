namespace HttpRemoteControlServer.Contracts;

public interface ISessionNotifier
{
    event Action? NewSessionOpened;
    event Action? NewCommandResult;
    void NotifyNewSessionOpened();
    void NotifyNewCommandResult();
}