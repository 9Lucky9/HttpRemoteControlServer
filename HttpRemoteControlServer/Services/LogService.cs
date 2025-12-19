using HttpRemoteControl.Library.Models;
using HttpRemoteControlServer.Contracts;

namespace HttpRemoteControlServer.Services;

public sealed class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;
    
    public Task Write(LogDto logDto)
    {
        throw new NotImplementedException();
    }
}