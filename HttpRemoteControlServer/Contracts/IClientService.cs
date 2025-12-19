using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControl.Library.Models.Responses;

namespace HttpRemoteControlServer.Contracts;

public interface IClientService
{
    Task<ClientRegistrationResponse> RegisterClient(ClientRegistrationRequest clientRegistrationRequest);
    Task<DequeuedCommandResponse> DequeueCommand(DequeueCommandRequest dequeueCommandRequest);
    Task WriteCommandResult(PushCommandResultRequest pushCommandResultRequest);
}