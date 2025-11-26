namespace HttpRemoteControl.Library.Models;

public sealed class CommandResult
{
    public Guid CommandId { get; set; }
    public Guid ClientId { get; set; }
    public Guid SessionId { get; set; }
    public string Result { get; set; }
    public DateTime ExecutedAt { get; set; }
    public DateTime FinishedAt { get; set; }
}