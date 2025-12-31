using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControl.Library.Models.Responses;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.DbContexts;
using HttpRemoteControlServer.Domain;
using HttpRemoteControlServer.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HttpRemoteControlServer.Services.ClientSide;

public sealed class RemoteClientManager : IRemoteClientManager
{
    private readonly ILogger<RemoteClientManager> _logger;
    private readonly IDbContextFactory<ServerContext> _contextFactory;
    private readonly ISessionNotifier _sessionNotifier;

    public RemoteClientManager(ILogger<RemoteClientManager> logger, IDbContextFactory<ServerContext> contextFactory, ISessionNotifier sessionNotifier)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _sessionNotifier = sessionNotifier;
    }

    public async Task<ClientRegistrationResponse> RegisterClient(
        RemoteClientRegistrationRequest request)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        RemoteClient? remoteClient;
        remoteClient = await context.RemoteClients
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.UniqueClientId);
        
        //If client not exists - register
        if (remoteClient == null)
        {
            remoteClient = RemoteClient.Create(
                request.UniqueClientId, 
                request.Description, 
                request.MachineInfo);
        }

        var remoteSession = 
            RemoteClientSession.Open(remoteClient);
        
        await context.RemoteClients.AddAsync(remoteClient);
        await context.RemoteSessions.AddAsync(remoteSession);
        await context.SaveChangesAsync();
        
        _sessionNotifier.NotifyNewSessionOpened();
        return new ClientRegistrationResponse()
        {
            SessionId = remoteSession.SessionId,
        };
    }

    public async Task<DequeuedCommandResponse> DequeueCommand(DequeueCommandRequest dequeueCommandRequest)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var remoteClient = await context.RemoteClients
            .AsNoTracking()
            .FirstOrDefaultAsync();
        if (remoteClient == null)
            throw new ArgumentException(
                $"Remote client not found. Id: {dequeueCommandRequest.RemoteClientUniqueId}");

        var remoteSession = await context.RemoteSessions
            .Include(remoteClientSessionEntity => remoteClientSessionEntity.Commands)
            .FirstOrDefaultAsync();
        if (remoteSession == null)
            throw new ArgumentException(
                $"Remote session not found. Id: {dequeueCommandRequest.SessionId}");

        var dequeudCommand = remoteSession.DequeueCommand();
        
        await context.SaveChangesAsync();

        var response = new DequeuedCommandResponse()
        {
            CommandId = dequeudCommand.Id,
            FileName = dequeudCommand.FileName,
            Args = dequeudCommand.Args,
        };
        return response;
    }

    public async Task WriteCommandResult(PushCommandResultRequest request)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var remoteClient = await context.RemoteClients
            .Include(x => x.Sessions)
            .ThenInclude(remoteClientSession => remoteClientSession.Commands)
            .FirstOrDefaultAsync();
        if (remoteClient == null)
            throw new EntityNotFoundException<RemoteClient>(
                $"Remote client not found. Id: {request.ClientId}");
        
        var remoteSession = remoteClient
            .Sessions
            .FirstOrDefault(
                x => x.SessionId == request.SessionId);
        
        if (remoteSession == null)
            throw new ArgumentException(
                $"Remote session not found. " +
                $"ClientId: {request.ClientId} " +
                $"SessionId: {request.SessionId}");

        var command = 
            remoteSession.Commands.FirstOrDefault(x => x.Id == request.CommandId);

        if (command == null)
            throw new EntityNotFoundException<Command>($"Command not found. Id: {request.CommandId}");
        
        command.Result = request.Result;
        await context.SaveChangesAsync();
        _sessionNotifier.NotifyNewCommandResult();
    }
}