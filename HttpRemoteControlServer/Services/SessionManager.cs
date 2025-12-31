using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.DbContexts;
using HttpRemoteControlServer.Domain;
using HttpRemoteControlServer.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HttpRemoteControlServer.Services;

public sealed class SessionManager : ISessionManager
{
    private readonly ILogger<SessionManager> _logger;
    private readonly IDbContextFactory<ServerContext> _contextFactory;

    public SessionManager(ILogger<SessionManager> logger, IDbContextFactory<ServerContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }
    
    public async Task Create(RemoteClientSession session)
    {
        
    }
    
    public async Task<IEnumerable<RemoteClientSession>> GetAll()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return context.RemoteSessions.AsNoTracking();
    }

    public async Task<RemoteClientSession> Get(Guid sessionId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        if(sessionId == Guid.Empty)
            throw new ArgumentException("Session ID cannot be empty");
        var session = await context.RemoteSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        if(session == null)
            throw new ArgumentException($"Session with ID {sessionId} does not exist");
        return session;
    }

    public async Task EnqueueCommand(CommandEnqueueRequest request)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var session =
            await context.RemoteSessions
                .FirstOrDefaultAsync(x => x.SessionId == request.SessionId);
        if(session == null)
            throw new EntityNotFoundException<RemoteClientSession>(
                $"RemoteSession with ID {request.SessionId} does not exist");
        
        session.EnqueueCommand(request.FileName, request.Args);
        await context.SaveChangesAsync();
    }

    public async Task<RemoteClientSession> CreateTestStaticSession()
    {
        throw new NotImplementedException();
    }
}