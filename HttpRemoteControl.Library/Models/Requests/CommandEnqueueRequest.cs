namespace HttpRemoteControl.Library.Models.Requests;

public sealed class CommandEnqueueRequest
{
    public required Guid ClientId { get; set; }
    public required Guid SessionId { get; set; }
    public required string FileName { get; set; }
    public required string Args { get; set; }
}