using HttpRemoteControlServer.Exceptions;

namespace HttpRemoteControlServer.Domain;

public class RemoteClientSession
{
    public Guid SessionId { get; set; }
    
    public Guid RemoteClientId { get; set; }
    public virtual RemoteClient RemoteClient { get; set; }
    public virtual List<Command> Commands { get; set; }
    public DateTime OpenedDateUtc { get; set; }
    public DateTime ClosedDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    
    public static RemoteClientSession Open(RemoteClient remoteClient)
    {
        return new RemoteClientSession()
        {
            SessionId = Guid.NewGuid(),
            RemoteClient = remoteClient,
            OpenedDateUtc = DateTime.UtcNow
        };
    }
    
    public void EnqueueCommand(string fileName, string args)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("CommandFileName can't be empty string");
        var command = new Command()
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            Args = args,
            Result = "",
            EnqueuedAt = DateTime.UtcNow,
            DequeuedAt = DateTime.MinValue,
            ExecutedAt = DateTime.MinValue,
            FinishedAt = DateTime.MinValue,
            CreatedAtUtc = DateTime.UtcNow,
        };
        Commands.Add(command);
    }
    
    public Command DequeueCommand()
    {
        var command = Commands.FirstOrDefault(
            x => x.DequeuedAt == DateTime.MinValue);
        if (command == null)
            throw new QueueEmptyException();
        command.DequeuedAt = DateTime.UtcNow;
        return command;
    }

    public void WriteCommandResult(Guid commandId, string result)
    {
        var command = Commands.FirstOrDefault(x => x.Id == commandId);
        if (command == null)
            throw new EntityNotFoundException<Command>(
                $"Writing result to command is failed. command not found. Id: {commandId}");
        command.Result = result;
    }
}