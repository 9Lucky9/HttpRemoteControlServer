using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Responses;

namespace HttpRemoteControlServer.Extensions;

public static class CommandExtensions
{
    public static DequeuedCommandResponse ToDequeuedCommandResponse(this Command command)
    {
        return new DequeuedCommandResponse()
        {
            Id = command.Id,
            FileName = command.FileName,
            Args = command.Args
        };
    }
}