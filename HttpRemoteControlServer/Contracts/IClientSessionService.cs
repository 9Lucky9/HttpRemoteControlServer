using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Models;

namespace HttpRemoteControlServer.Contracts;

public interface IClientSessionService
{
    # region ClientSession
    public event EventHandler StateChanged;
    
    Task<ClientSession> CreateClientSession(ClientRegistrationRequest clientRegistrationRequest);
    Task<ClientSession> CreateTestStaticClientSession();
    Task RemoveClientSession(Guid clientSessionId);
    Task<ClientSession> GetClientSession(Guid clientSessionId);
    Task<IEnumerable<ClientSession>> GetClientSessions();
    Task<IEnumerable<Command>> GetCommandQueueFromSession(Guid clientSessionId);
    # endregion
    
    # region CommandQueue
    public Task EnqueueCommand(CommandEnqueueRequest commandEnqueueRequest);
    public Task<Command> DequeueCommand(DequeueCommandRequest dequeueCommandRequest);
    public Task WriteCommandResult(CommandResultRequest commandResultRequest);
    public Task ClearQueue(ClearCommandQueueRequest clearCommandQueueRequest);
    # endregion
}