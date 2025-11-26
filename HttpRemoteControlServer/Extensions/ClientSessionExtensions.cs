using HttpRemoteControl.Library.Models.Responses;
using HttpRemoteControlServer.Models;

namespace HttpRemoteControlServer.Extensions;

public static class ClientSessionExtensions
{
    public static ClientRegistrationResponse ToClientRegistrationResponse(this ClientSession clientSession)
    {
        return new ClientRegistrationResponse()
        {
            SessionId = clientSession.SessionId,
            ClientId = clientSession.Client.Id
        };
    }
}