using System.Text.Json;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class ClientController : ControllerBase
{
    private readonly ILogger<ClientController> _logger;
    private readonly IRemoteClientService _remoteClientService;

    public ClientController(ILogger<ClientController> logger, IRemoteClientService remoteClientService)
    {
        _logger = logger;
        _remoteClientService = remoteClientService;
    }
    
    [HttpPost]
    public async Task<IActionResult> RegisterMe(RemoteClientRegistrationRequest remoteClientRegistrationRequest)
    {
        try
        {
            var receivedJson = JsonSerializer.Serialize(remoteClientRegistrationRequest);
            _logger.LogInformation(
                "Received request to register new client. Json: {json}", receivedJson);
            var clientGuid = await _remoteClientService.RegisterClient(remoteClientRegistrationRequest);
            var clientJson = JsonSerializer.Serialize(remoteClientRegistrationRequest);
            _logger.LogInformation(
                "Successfully registered client: {clientJson}. With id: {id}", clientJson, clientGuid);
            return Ok(clientGuid);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unexpected error occured during registering new client. Ex: {ex}", e);
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                "Unexpected error occured during registering new client.");
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> DequeueCommand(DequeueCommandRequest commandRequest)
    {
        try
        {
            _logger.LogInformation("Received request to dequeue command.");
            var command = await _remoteClientService.DequeueCommand(commandRequest);
            return Ok(command);
        }
        catch (InvalidOperationException)
        {
            _logger.LogInformation("Received request to dequeue command but queue is empty.");
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unexpected error occured during dequeue of command. Ex: {ex}", 
                e);
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                "Unexpected error occured during dequeue of command.");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> WriteCommandResult(PushCommandResultRequest pushCommandResultRequest)
    {
        try
        {
            _logger.LogInformation("Received request to write command result.");
            await _remoteClientService.WriteCommandResult(pushCommandResultRequest);
            return NoContent();
        }
        catch (CommandNotFoundException e)
        {
            _logger.LogError(
                "Error occured during writing command result. Command with id: {commandId} is not found", 
                pushCommandResultRequest.CommandId);
            return BadRequest($"Command with id: {pushCommandResultRequest.CommandId} is not found");
        }
        catch (ClientSessionNotFoundException e)
        {
            _logger.LogError(
                "Error occured during writing command result. Command with id: {commandId} is not found", 
                pushCommandResultRequest.CommandId);
            return BadRequest("");
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unexpected error occured during getting all commands in queue. Ex: {ex}", 
                e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}