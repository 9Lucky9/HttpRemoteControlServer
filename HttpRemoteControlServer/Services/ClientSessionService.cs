using System.Collections.Concurrent;
using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Exceptions;
using HttpRemoteControlServer.Models;

namespace HttpRemoteControlServer.Services;

public sealed class ClientSessionService : IClientSessionService
{
    private ConcurrentDictionary<Guid, ClientSession> _sessions = 
        new ConcurrentDictionary<Guid, ClientSession>();
    
    public event EventHandler? StateChanged;

    public async Task<ClientSession> CreateClientSession(ClientRegistrationRequest clientRegistrationRequest)
    {
        var client = new Client()
        {
            MachineInfo = clientRegistrationRequest.MachineInfo,
        };
        var clientSession = new ClientSession()
        {
            SessionId = Guid.NewGuid(),
            Client = client,
            OpenedDate = DateTime.UtcNow
        };
        _sessions.TryAdd(clientSession.SessionId, clientSession);
        OnStateChanged();
        return await Task.FromResult(clientSession);
    }

    public async Task<ClientSession> CreateTestStaticClientSession()
    {
        var client = new Client()
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
        var clientSession = new ClientSession()
        {
            SessionId = new Guid("89801fbb-08e6-45be-9cae-855080c9393c"),
            Client = client,
            OpenedDate = DateTime.UtcNow
        };
        _sessions.TryAdd(clientSession.SessionId, clientSession);
        Console.WriteLine(
            $"Created TestStaticClientSession with sessionId: {clientSession.SessionId}");
        OnStateChanged();
        return await Task.FromResult(clientSession);
    }

    public Task RemoveClientSession(Guid clientSessionId)
    {
        throw new NotImplementedException();
        OnStateChanged();
    }

    public async Task<ClientSession> GetClientSession(Guid clientSessionId)
    {
        var found = _sessions.TryGetValue(clientSessionId, out var clientSession);
        if (!found)
            throw new ClientSessionNotFoundException(
                $"ClientSession with id: {clientSessionId} is not found.");
        return (await Task.FromResult(clientSession))!;
    }

    public async Task<IEnumerable<ClientSession>> GetClientSessions()
    {
        return await Task.FromResult(_sessions.Values.ToList());
    }

    public async Task<IEnumerable<Command>> GetCommandQueueFromSession(Guid clientSessionId)
    {
        _sessions.TryGetValue(clientSessionId, out ClientSession? session);
        if (session == null)
            throw new ClientSessionNotFoundException(
                $"ClientSession with id: {clientSessionId} is not found.");
        return await Task.FromResult(session.Client.CommandQueue);
    }

    public async Task EnqueueCommand(CommandEnqueueRequest commandEnqueueRequest)
    {
        _sessions.TryGetValue(commandEnqueueRequest.SessionId, out ClientSession session);
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
        session.Client.EnqueueCommand(command);
        await Task.FromResult(session.Client.CommandQueue);
        OnStateChanged();
    }

    public async Task<Command> DequeueCommand(DequeueCommandRequest dequeueCommandRequest)
    {
        _sessions.TryGetValue(dequeueCommandRequest.SessionId, out ClientSession session);
        if (session == null)
            throw new ArgumentException(
                $"ClientSession with session id: {dequeueCommandRequest.SessionId} is not found.", 
                nameof(dequeueCommandRequest.SessionId));
        session.Client.TryDequeueCommand(out var command);
        return await Task.FromResult(command);
        OnStateChanged();
    }

    public async Task WriteCommandResult(CommandResultRequest commandResultRequest)
    {
        _sessions.TryGetValue(commandResultRequest.SessionId, out ClientSession session);
        if (session == null)
            throw new ClientSessionNotFoundException(
                $"ClientSession with id: {commandResultRequest.SessionId} is not found.");
        session.Client.TryGetDequeuedCommand(commandResultRequest.CommandId, out var command);
        if (command == null)
            throw new CommandNotFoundException($"Command with id: {commandResultRequest.CommandId} is not found.");
        command.Result = commandResultRequest.Result;
        session.Client.WriteCommandResult(commandResultRequest);
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
