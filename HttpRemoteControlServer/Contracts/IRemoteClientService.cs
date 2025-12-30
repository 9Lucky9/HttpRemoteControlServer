using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControl.Library.Models.Responses;

namespace HttpRemoteControlServer.Contracts;

public interface IRemoteClientService
{
    Task<ClientRegistrationResponse> RegisterClient(RemoteClientRegistrationRequest request);
    Task<DequeuedCommandResponse> DequeueCommand(DequeueCommandRequest dequeueCommandRequest);
    Task WriteCommandResult(PushCommandResultRequest request);
}