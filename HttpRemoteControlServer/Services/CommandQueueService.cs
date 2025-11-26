using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Contracts;

namespace HttpRemoteControlServer.Services;

public class CommandQueueService : ICommandQueueService
{
    private readonly Queue<Command> _commands;
    private readonly Dictionary<Guid, Command> _sessionCommands;

    public CommandQueueService()
    {
        _commands = new Queue<Command>();
        _sessionCommands = new Dictionary<Guid, Command>();
    }

    public void EnqueueCommand(CommandEnqueueRequest commandEnqueueRequest)
    {
        if(string.IsNullOrEmpty(commandEnqueueRequest.FileName))
            throw new ArgumentException("Command FileName cannot be null or empty.", nameof(commandEnqueueRequest));
        if(string.IsNullOrEmpty(commandEnqueueRequest.Args))
            throw new ArgumentException("Command Args cannot be null or empty.", nameof(commandEnqueueRequest));
        var command = new Command()
        {
            Id = Guid.NewGuid(),
            FileName = commandEnqueueRequest.FileName,
            Args = commandEnqueueRequest.Args,
            EnqueuedAt = DateTime.Now,
        };
        _commands.Enqueue(command);
    }

    public Command DequeueCommandFromQueue()
    {
        var command = _commands.Dequeue();
        command.DequeuedAt = DateTime.Now;
        _sessionCommands.Add(command.Id, command);
        return _commands.Dequeue();
    }

    public void WriteCommandResult(CommandResultRequest commandResultRequest)
    {
        _sessionCommands.TryGetValue(commandResultRequest.CommandId, out var command);
        if(command == null)
            throw new ArgumentException("Command not found.", nameof(commandResultRequest));
        command.Result = commandResultRequest.Result;
    }

    public IEnumerable<Command> GetAllCommandInSession()
    {
        return _sessionCommands.Values;
    }

    public IEnumerable<Command> GetAllCommandsInQueue()
    {
        return _commands;
    }

    public void ClearCommandQueue()
    {
        _commands.Clear();
    }
}