using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControl.Library.Models.Responses;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.DbContexts;
using HttpRemoteControlServer.Domain;
using HttpRemoteControlServer.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HttpRemoteControlServer.Services;

public sealed class RemoteClientService : IRemoteClientService
{
    private readonly ServerContext _serverContext;

    public RemoteClientService(ServerContext serverContext)
    {
        _serverContext = serverContext;
    }

    public async Task<ClientRegistrationResponse> RegisterClient(
        RemoteClientRegistrationRequest request)
    {
        RemoteClient? remoteClient;
        remoteClient = await _serverContext.RemoteClients
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
        
        await _serverContext.RemoteClients.AddAsync(remoteClient);
        await _serverContext.RemoteSessions.AddAsync(remoteSession);
        await _serverContext.SaveChangesAsync();
        return new ClientRegistrationResponse()
        {
            SessionId = remoteSession.SessionId,
        };
    }

    public async Task<DequeuedCommandResponse> DequeueCommand(DequeueCommandRequest dequeueCommandRequest)
    {
        var remoteClient = await _serverContext.RemoteClients
            .AsNoTracking()
            .FirstOrDefaultAsync();
        if (remoteClient == null)
            throw new ArgumentException(
                $"Remote client not found. Id: {dequeueCommandRequest.RemoteClientUniqueId}");

        var remoteSession = await _serverContext.RemoteSessions
            .Include(remoteClientSessionEntity => remoteClientSessionEntity.Commands)
            .FirstOrDefaultAsync();
        if (remoteSession == null)
            throw new ArgumentException(
                $"Remote session not found. Id: {dequeueCommandRequest.SessionId}");

        var dequeudCommand = remoteSession.DequeueCommand();
        
        await _serverContext.SaveChangesAsync();

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
        var remoteClient = await _serverContext.RemoteClients
            .Include(x => x.Sessions)
            .ThenInclude(remoteClientSession => remoteClientSession.Commands)
            .FirstOrDefaultAsync();
        if (remoteClient == null)
            throw new ArgumentException(
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
            throw new CommandNotFoundException($"Command not found. Id: {request.CommandId}");
        
        command.Result = request.Result;
        await _serverContext.SaveChangesAsync();
    }
}