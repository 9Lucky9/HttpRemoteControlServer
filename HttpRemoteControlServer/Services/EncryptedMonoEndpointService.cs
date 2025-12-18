using System.Text.Json;
using HttpRemoteControl.Library.Encryptor;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControl.Library.Models.Responses;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Exceptions;
using HttpRemoteControlServer.Options;
using Microsoft.Extensions.Options;

namespace HttpRemoteControlServer.Services;

public sealed class EncryptedMonoEndpointService
{
    private readonly ILogger<EncryptedMonoEndpointService> _logger;
    private readonly IClientService _clientService;
    private readonly IOptionsSnapshot<MonoEndpointOptions> _options;
    private readonly IEncryptor _encryptor;

    public EncryptedMonoEndpointService(ILogger<EncryptedMonoEndpointService> logger, IClientService clientService, IOptionsSnapshot<MonoEndpointOptions> options, IEncryptor encryptor)
    {
        _logger = logger;
        _clientService = clientService;
        _options = options;
        _encryptor = encryptor;
    }

    /// <summary>
    /// Processes request and returns encrypted response.
    /// </summary>
    public async Task<string> ProcessRequest(MonoEndpointDataRequest request)
    {
        if (string.IsNullOrEmpty(request.Method))
            throw new MonoEndpointException("Method canno't be null or empty.");
        if (string.IsNullOrEmpty(request.Payload))
            throw new MonoEndpointException("Payload canno't be null or empty.");
        switch (request.Method)
        {
            case "RegisterMe":
            {
                var response = 
                    await ProcessAsync<ClientRegistrationRequest, ClientRegistrationResponse>(
                        request, 
                        _clientService.RegisterClient);
                var json = JsonSerializer.Serialize(response);
                var encryptedJson = _encryptor.Encrypt(
                    _options.Value.Key,
                    json);
                return encryptedJson;
            }


            case "DequeueCommand":
            {
                var response = 
                    await ProcessAsync<DequeueCommandRequest, DequeuedCommandResponse>(
                        request, 
                        _clientService.DequeueCommand);
                
                var json = JsonSerializer.Serialize(response);
                var encryptedJson = _encryptor.Encrypt(
                    _options.Value.Key,
                    json);
                return encryptedJson;
            }


            case "PushCommandResult":
            {
                var commandResultRequest = 
                    JsonSerializer.Deserialize<CommandResultRequest>(request.Payload);
                await _clientService.WriteCommandResult(commandResultRequest);
                var monoResponse = new MonoEndpointDataResponse()
                {
                    Method = request.Method,
                    Payload = ""
                };
                var json = JsonSerializer.Serialize(monoResponse);
                var encryptedJson = _encryptor.Encrypt(
                    _options.Value.Key,
                    json);
                return encryptedJson;
            }
            default:
                throw new MonoEndpointException($"Method {request.Method} for processing not found.");
        }
    }

    private static async Task<MonoEndpointDataResponse> ProcessAsync<TRequest, TResponse>(
        MonoEndpointDataRequest monoEndpointDataRequest,
        Func<TRequest, Task<TResponse>> processor)
    {
        
        var request = 
            JsonSerializer.Deserialize<TRequest>(monoEndpointDataRequest.Payload);
        var processorResponse = await processor(request);
        
        var json = 
            JsonSerializer.Serialize(processorResponse);
        
        return new MonoEndpointDataResponse()
        {
            Method = monoEndpointDataRequest.Method,
            Payload = json,
        };
    }
}