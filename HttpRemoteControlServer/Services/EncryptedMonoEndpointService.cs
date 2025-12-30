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
    private readonly IOptionsSnapshot<MonoEndpointOptions> _options;
    private readonly IEncryptor _encryptor;
    private readonly IRemoteClientService _remoteClientService;

    public EncryptedMonoEndpointService(ILogger<EncryptedMonoEndpointService> logger, IOptionsSnapshot<MonoEndpointOptions> options, IEncryptor encryptor, IRemoteClientService remoteClientService)
    {
        _logger = logger;
        _options = options;
        _encryptor = encryptor;
        _remoteClientService = remoteClientService;
    }

    public async Task<string> ProcessEncryptedJson(string encryptedJson)
    {
        if(string.IsNullOrEmpty(encryptedJson))
            throw new MonoEndpointException("Received encryptedJson json is null or empty");

        //Decrypt json
        var decryptedJson = 
            _encryptor.Decrypt(_options.Value.Key, encryptedJson);
        
        //Deserialize decrypted json
        var monoRequest = 
            JsonSerializer.Deserialize<MonoEndpointDataRequest>(decryptedJson);
        
        if(string.IsNullOrEmpty(monoRequest.Path))
            throw new MonoEndpointException("MonoRequest.Path cannot be null or empty");
        
        if(string.IsNullOrEmpty(monoRequest.Payload))
            throw new MonoEndpointException("MonoRequest.Payload cannot be null or empty");
        
        //Map to clientService method & execute
        var monoResponse = monoRequest.Path switch
        {
            "/client/register-me" =>
                await Process<RemoteClientRegistrationRequest, ClientRegistrationResponse>(
                    monoRequest,
                    _remoteClientService.RegisterClient),
            
            "/client/dequeue-command" =>
                await Process<DequeueCommandRequest, DequeuedCommandResponse>(
                    monoRequest,
                    _remoteClientService.DequeueCommand),
            
            "/client/write-command-result" =>
                await Process<PushCommandResultRequest>(
                    monoRequest,
                    _remoteClientService.WriteCommandResult),
            _ => throw new MonoEndpointException(
                $"monoRequest.Path was not recognized: {monoRequest.Path}")
        };
        
        var responseJson = JsonSerializer.Serialize(monoResponse);
        var encryptedResponse = 
            _encryptor.Encrypt(_options.Value.Key, responseJson);
        return encryptedResponse;
    }
    
    private static async Task<MonoEndpointDataResponse> Process<TReq, TRes>(
        MonoEndpointDataRequest monoRequest,
        Func<TReq, Task<TRes>> handler)
    {
        var request = 
            JsonSerializer.Deserialize<TReq>(monoRequest.Payload);
        var response = await handler(request);
        var responseJson = JsonSerializer.Serialize(response);
        var monoEndpointDataResponse = new MonoEndpointDataResponse()
        {
            Path = monoRequest.Path,
            Payload = responseJson
        };
        return monoEndpointDataResponse;
    }

    private static async Task<MonoEndpointDataResponse> Process<TReq>(
        MonoEndpointDataRequest monoRequest,
        Func<TReq, Task> handler)
    {
        var request = 
            JsonSerializer.Deserialize<TReq>(monoRequest.Payload);
        await handler(request);
        var monoEndpointDataResponse = new MonoEndpointDataResponse()
        {
            Path = monoRequest.Path,
            Payload = ""
        };
        return monoEndpointDataResponse;
    }
}