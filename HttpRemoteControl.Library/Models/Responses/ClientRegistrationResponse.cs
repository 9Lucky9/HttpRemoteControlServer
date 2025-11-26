namespace HttpRemoteControl.Library.Models.Responses;

public sealed class ClientRegistrationResponse
{
    public Guid ClientId { get; set; }
    public Guid SessionId { get; set; }
}