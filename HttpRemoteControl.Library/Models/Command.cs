namespace HttpRemoteControl.Library.Models;

public sealed class Command
{
    public required Guid Id { get; set; }
    public required string FileName { get; set; }
    public string Args { get; set; }
    public string Result { get; set; }
    public DateTime EnqueuedAt { get; set; }
    public DateTime DequeuedAt { get; set; }
    public DateTime ExecutedAt { get; set; }
    public DateTime FinishedAt { get; set; }
}