using HttpRemoteControl.Library.Models;

namespace HttpRemoteControlServer.Contracts;

public interface ILogService
{
    Task Write(LogDto logDto);
}