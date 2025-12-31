using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Domain;

namespace HttpRemoteControlServer.Contracts;

public interface ISessionManager
{
    Task Create(RemoteClientSession session);
    
    Task<IEnumerable<RemoteClientSession>> GetAll();
    Task<RemoteClientSession> Get(Guid sessionId);

    Task EnqueueCommand(CommandEnqueueRequest  request);
    
    Task<RemoteClientSession> CreateTestStaticSession();
}