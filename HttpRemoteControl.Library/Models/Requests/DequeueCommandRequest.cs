namespace HttpRemoteControl.Library.Models.Requests;

public sealed class DequeueCommandRequest
{
    public Guid RemoteClientUniqueId { get; set; }
    public Guid SessionId { get; set; }
}