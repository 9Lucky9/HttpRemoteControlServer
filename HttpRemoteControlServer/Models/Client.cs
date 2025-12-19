using System.Diagnostics.CodeAnalysis;
using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Exceptions;

namespace HttpRemoteControlServer.Models;

public sealed class Client
{
    public Guid Id { get; set; }
    public required MachineInfo MachineInfo { get; set; }
    
    public IReadOnlyCollection<Command> Commands { get; set; }
    private Queue<Command> _commandQueue = new Queue<Command>();
    private Dictionary<Guid, Command> _dequeuedCommands = new Dictionary<Guid, Command>();
    
    public IReadOnlyCollection<Command> CommandQueue => _commandQueue;
    public IReadOnlyCollection<Command> DequeuedCommands => _dequeuedCommands.Values;

    public void EnqueueCommand(Command command)
    {
        _commandQueue.Enqueue(command);
    }

    public Command DequeueCommand()
    {
        var command = _commandQueue.Dequeue();
        _dequeuedCommands.Add(command.Id, command);
        return command;
    }
    
    public bool TryDequeueCommand([MaybeNullWhen(false)] out Command command)
    {
        if (!_commandQueue.TryDequeue(out command))
            return false;
        _dequeuedCommands.Add(command.Id, command);
        return true;
    }

    public void WriteCommandResult(PushCommandResultRequest pushCommandResultRequest)
    {
        if (!_dequeuedCommands.TryGetValue(pushCommandResultRequest.CommandId, out Command? command))
            throw new CommandNotFoundException(
                $"Writing result to command is failed, command with id: {pushCommandResultRequest.CommandId} is not found");
        command.Result = pushCommandResultRequest.Result;
    }

    public bool TryGetDequeuedCommand(Guid commandId, [MaybeNullWhen(false)] out Command? command)
    {
        return _dequeuedCommands.TryGetValue(commandId, out command);
    }
}