using System.Collections.Concurrent;
using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Domain;
using HttpRemoteControlServer.Exceptions;

namespace HttpRemoteControlServer.Services;

public sealed class ClientSessionService : IClientSessionService
{
    private ConcurrentDictionary<Guid, RemoteClientSession> _sessions = 
        new ConcurrentDictionary<Guid, RemoteClientSession>();
    
    public event EventHandler? StateChanged;

    public async Task<RemoteClientSession> CreateClientSession(RemoteClientRegistrationRequest remoteClientRegistrationRequest)
    {
        var client = new RemoteClient()
        {
            MachineInfo = remoteClientRegistrationRequest.MachineInfo,
        };
        var clientSession = new RemoteClientSession()
        {
            SessionId = Guid.NewGuid(),
            RemoteClient = client,
            OpenedDate = DateTime.UtcNow
        };
        _sessions.TryAdd(clientSession.SessionId, clientSession);
        OnStateChanged();
        return await Task.FromResult(clientSession);
    }

    public async Task<RemoteClientSession> CreateTestStaticClientSession()
    {
        var client = new RemoteClient()
        {
            MachineInfo = new MachineInfo()
            {
                PrettyName = "Debian GNU/Linux 12 (bookworm)",
                IpAddress = "172.20.17.130 10.202.0.110",
                HostName = "k8s-controlplane01",
                User = "S_EvstigneevPA",
                Home = "/home/S_EvstigneevPA",
                Shell = "/bin/bash"
            }
        };
        var clientSession = new RemoteClientSession()
        {
            SessionId = new Guid("89801fbb-08e6-45be-9cae-855080c9393c"),
            RemoteClient = client,
            OpenedDate = DateTime.UtcNow
        };
        _sessions.TryAdd(clientSession.SessionId, clientSession);
        Console.WriteLine(
            $"Created TestStaticClientSession with sessionId: {clientSession.SessionId}");
        OnStateChanged();
        return await Task.FromResult(clientSession);
    }

    public async Task RemoveClientSession(Guid clientSessionId)
    {
        _sessions.TryRemove(clientSessionId, out var clientSession);
        if(clientSession == null)
            throw new ClientSessionNotFoundException(
                $"ClientSession with id: {clientSessionId} is not found.");
        OnStateChanged();
    }

    public async Task<RemoteClientSession> GetClientSession(Guid clientSessionId)
    {
        var found = _sessions.TryGetValue(clientSessionId, out var clientSession);
        if (!found)
            throw new ClientSessionNotFoundException(
                $"ClientSession with id: {clientSessionId} is not found.");
        return (await Task.FromResult(clientSession))!;
    }

    public async Task<IEnumerable<RemoteClientSession>> GetClientSessions()
    {
        return await Task.FromResult(_sessions.Values.ToList());
    }

    public async Task<IEnumerable<Command>> GetCommandQueueFromSession(Guid clientSessionId)
    {
        _sessions.TryGetValue(clientSessionId, out RemoteClientSession? session);
        if (session == null)
            throw new ClientSessionNotFoundException(
                $"ClientSession with id: {clientSessionId} is not found.");
        return await Task.FromResult(session.RemoteClient.CommandQueue);
    }

    public async Task EnqueueCommand(CommandEnqueueRequest commandEnqueueRequest)
    {
        _sessions.TryGetValue(commandEnqueueRequest.SessionId, out RemoteClientSession session);
        if (session == null)
            throw new ArgumentException(
                $"ClientSession with session id: {commandEnqueueRequest.SessionId} is not found.", 
                nameof(commandEnqueueRequest.SessionId));
        var command = new Command()
        {
            Id = Guid.NewGuid(),
            FileName = commandEnqueueRequest.FileName,
            Args = commandEnqueueRequest.Args,
            EnqueuedAt = DateTime.UtcNow,
        };
        session.RemoteClient.EnqueueCommand(command);
        await Task.FromResult(session.RemoteClient.CommandQueue);
        OnStateChanged();
    }

    public async Task<Command> DequeueCommand(DequeueCommandRequest dequeueCommandRequest)
    {
        _sessions.TryGetValue(dequeueCommandRequest.SessionId, out RemoteClientSession session);
        if (session == null)
            throw new ArgumentException(
                $"ClientSession with session id: {dequeueCommandRequest.SessionId} is not found.", 
                nameof(dequeueCommandRequest.SessionId));
        session.RemoteClient.TryDequeueCommand(out var command);
        return await Task.FromResult(command);
        OnStateChanged();
    }

    public async Task WriteCommandResult(PushCommandResultRequest pushCommandResultRequest)
    {
        _sessions.TryGetValue(pushCommandResultRequest.SessionId, out RemoteClientSession session);
        if (session == null)
            throw new ClientSessionNotFoundException(
                $"ClientSession with id: {pushCommandResultRequest.SessionId} is not found.");
        session.RemoteClient.TryGetDequeuedCommand(pushCommandResultRequest.CommandId, out var command);
        if (command == null)
            throw new CommandNotFoundException($"Command with id: {pushCommandResultRequest.CommandId} is not found.");
        command.Result = pushCommandResultRequest.Result;
        session.RemoteClient.WriteCommandResult(pushCommandResultRequest);
        OnStateChanged();
    }

    public Task ClearQueue(ClearCommandQueueRequest clearCommandQueueRequest)
    {
        throw new NotImplementedException();
        OnStateChanged();
    }

    private void OnStateChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
