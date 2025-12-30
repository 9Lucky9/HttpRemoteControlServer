using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Domain;

namespace HttpRemoteControlServer.Contracts;

public interface IClientSessionService
{
    # region ClientSession
    public event EventHandler StateChanged;
    
    Task<RemoteClientSession> CreateClientSession(RemoteClientRegistrationRequest remoteClientRegistrationRequest);
    Task<RemoteClientSession> CreateTestStaticClientSession();
    Task RemoveClientSession(Guid clientSessionId);
    Task<RemoteClientSession> GetClientSession(Guid clientSessionId);
    Task<IEnumerable<RemoteClientSession>> GetClientSessions();
    Task<IEnumerable<Command>> GetCommandQueueFromSession(Guid clientSessionId);
    # endregion
    
    # region CommandQueue
    public Task EnqueueCommand(CommandEnqueueRequest commandEnqueueRequest);
    public Task<Command> DequeueCommand(DequeueCommandRequest dequeueCommandRequest);
    public Task WriteCommandResult(PushCommandResultRequest pushCommandResultRequest);
    public Task ClearQueue(ClearCommandQueueRequest clearCommandQueueRequest);
    # endregion
}