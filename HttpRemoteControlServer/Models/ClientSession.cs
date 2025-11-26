using Microsoft.AspNetCore.Http.HttpResults;

namespace HttpRemoteControlServer.Models;

public sealed class ClientSession
{
    public required Guid SessionId { get; set; }
    public required Client Client { get; set; }
    public DateTime OpenedDate { get; set; }
    public DateTime ClosedDate { get; set; }

    public static ClientSession Create(Client client)
    {
        return new ClientSession()
        {
            SessionId = Guid.NewGuid(),
            Client = client,
            OpenedDate = DateTime.UtcNow
        };
    }
}