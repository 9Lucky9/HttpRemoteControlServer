using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControl.Library.Models.Responses;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Extensions;

namespace HttpRemoteControlServer.Services;

public sealed class ClientService : IClientService
{
    private readonly IClientSessionService _clientSessionService;

    public ClientService(IClientSessionService clientSessionService)
    {
        _clientSessionService = clientSessionService;
    }

    public async Task<ClientRegistrationResponse> RegisterClient(ClientRegistrationRequest clientRegistrationRequest)
    {
        var clientSession = await _clientSessionService.CreateClientSession(clientRegistrationRequest);
        return clientSession.ToClientRegistrationResponse();
    }

    public async Task<DequeuedCommandResponse> DequeueCommand(DequeueCommandRequest dequeueCommandRequest)
    {
        var command = await _clientSessionService.DequeueCommand(dequeueCommandRequest);
        return command.ToDequeuedCommandResponse();
    }

    public async Task WriteCommandResult(PushCommandResultRequest pushCommandResultRequest)
    {
        await _clientSessionService.WriteCommandResult(pushCommandResultRequest);
    }
}