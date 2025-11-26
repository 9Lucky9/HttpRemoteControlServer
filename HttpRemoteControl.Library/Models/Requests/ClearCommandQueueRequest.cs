namespace HttpRemoteControl.Library.Models.Requests;

public sealed class ClearCommandQueueRequest
{
    public Guid SessionId { get; set; }
    public Guid ClientId { get; set; }
}