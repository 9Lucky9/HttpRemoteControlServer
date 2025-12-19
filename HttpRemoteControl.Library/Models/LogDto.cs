namespace HttpRemoteControl.Library.Models;

public sealed class LogDto
{
    public Guid SessionId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
}