using System.Text.Json;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControl.Library.Models.Responses;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Exceptions;

namespace HttpRemoteControlServer.Services;

public sealed class MonoEndpointService
{
    private readonly IClientService _clientService;

    public MonoEndpointService(IClientService clientService)
    {
        _clientService = clientService;
    }

    public async Task<MonoEndpointDataResponse> Process(MonoEndpointDataRequest request)
    {
        if (string.IsNullOrEmpty(request.Method))
            throw new MonoEndpointException("Method canno't be null or empty.");
        if (string.IsNullOrEmpty(request.Payload))
            throw new MonoEndpointException("Payload canno't be null or empty.");
        switch (request.Method)
        {
            case "RegisterClient":
                return await ExecuteAsync<ClientRegistrationRequest, ClientRegistrationResponse>(request,
                    _clientService.RegisterClient);
            
            case "DequeueCommand":
                return await ExecuteAsync<DequeueCommandRequest, DequeuedCommandResponse>(request,
                    _clientService.DequeueCommand);
            
            case "WriteCommandResult":
                var commandResultRequest = 
                    JsonSerializer.Deserialize<CommandResultRequest>(request.Payload);
                await _clientService.WriteCommandResult(commandResultRequest);
                return new MonoEndpointDataResponse()
                {
                    Method = request.Method,
                    Payload = ""
                };
            
            default:
                throw new MonoEndpointException($"Method {request.Method} for processing not found.");
        }
    }

    private static async Task<MonoEndpointDataResponse> ExecuteAsync<TRequest, TResponse>(
        MonoEndpointDataRequest monoEndpointDataRequest,
        Func<TRequest, Task<TResponse>> processor)
    {
        var request = 
            JsonSerializer.Deserialize<TRequest>(monoEndpointDataRequest.Payload);
        var response = await processor(request);
        
        var json = 
            JsonSerializer.Serialize(response);
        
        return new MonoEndpointDataResponse()
        {
            Method = monoEndpointDataRequest.Method,
            Payload = json,
        };
    }
}