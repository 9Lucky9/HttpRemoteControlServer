using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Requests;

namespace HttpRemoteControlServer.Contracts;

public interface ICommandQueueService
{
    public void EnqueueCommand(CommandEnqueueRequest commandEnqueueRequest);
    public Command DequeueCommandFromQueue();
    public void WriteCommandResult(CommandResultRequest commandResultRequest);
    public IEnumerable<Command> GetAllCommandsInQueue();
    public IEnumerable<Command> GetAllCommandInSession();
    public void ClearCommandQueue();
}