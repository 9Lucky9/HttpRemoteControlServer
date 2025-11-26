namespace HttpRemoteControl.Library.Models.Requests;

public sealed class DequeueCommandRequest
{
    public Guid SessionId { get; set; }
    public Guid ClientId { get; set; }
}