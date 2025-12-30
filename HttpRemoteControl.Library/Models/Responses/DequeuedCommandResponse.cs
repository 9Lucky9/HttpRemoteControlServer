namespace HttpRemoteControl.Library.Models.Responses;

public sealed class DequeuedCommandResponse
{
    public required Guid CommandId { get; set; }
    public required string FileName { get; set; }
    public required string Args { get; set; }
}