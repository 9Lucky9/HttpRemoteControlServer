namespace HttpRemoteControl.Library.Models.Requests;

public sealed class PushCommandResultRequest
{
    public Guid ClientId { get; set; }
    public Guid SessionId { get; set; }
    public Guid CommandId { get; set; }
    public string Result { get; set; }
    public DateTime ExecutedAt { get; set; }
    public DateTime FinishedAt { get; set; }
}