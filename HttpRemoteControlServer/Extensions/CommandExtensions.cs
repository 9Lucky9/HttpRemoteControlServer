using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Responses;

namespace HttpRemoteControlServer.Extensions;

static public class CommandExtensions
{
    static public DequeuedCommandResponse ToDequeuedCommandResponse(this Command command)
    {
        return new DequeuedCommandResponse()
        {
            Id = command.Id,
            FileName = command.FileName,
            Args = command.Args
        };
    }
}