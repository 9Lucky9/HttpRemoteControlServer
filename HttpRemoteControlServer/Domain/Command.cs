namespace HttpRemoteControlServer.Domain;

public sealed class Command
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string? Args { get; set; }
    public string Result { get; set; }
    public DateTime EnqueuedAt { get; set; }
    public DateTime DequeuedAt { get; set; }
    public DateTime ExecutedAt { get; set; }
    public DateTime FinishedAt { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public static Command Create()
    {
        return new Command()
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}