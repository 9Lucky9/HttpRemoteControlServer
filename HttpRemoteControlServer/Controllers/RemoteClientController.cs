using System.Text.Json;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Domain;
using HttpRemoteControlServer.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class RemoteClientController : ControllerBase
{
    private readonly ILogger<RemoteClientController> _logger;
    private readonly IRemoteClientManager _remoteClientManager;

    public RemoteClientController(ILogger<RemoteClientController> logger, IRemoteClientManager remoteClientManager)
    {
        _logger = logger;
        _remoteClientManager = remoteClientManager;
    }
    
    [HttpPost]
    public async Task<IActionResult> RegisterMe(RemoteClientRegistrationRequest remoteClientRegistrationRequest)
    {
        try
        {
            var receivedJson = JsonSerializer.Serialize(remoteClientRegistrationRequest);
            _logger.LogInformation(
                "Received request to register new client. Json: {json}", receivedJson);
            var clientGuid = await _remoteClientManager.RegisterClient(remoteClientRegistrationRequest);
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
            var command = await _remoteClientManager.DequeueCommand(commandRequest);
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
            await _remoteClientManager.WriteCommandResult(pushCommandResultRequest);
            return NoContent();
        }
        catch (EntityNotFoundException<RemoteClient> e)
        {
            _logger.LogError(
                "Error occured during writing command result. RemoteClient with id: {Id} is not found", 
                pushCommandResultRequest.ClientId);
            return BadRequest("");
        }
        catch (EntityNotFoundException<Command> e)
        {
            _logger.LogError(
                "Error occured during writing command result. Command with id: {commandId} is not found", 
                pushCommandResultRequest.CommandId);
            return BadRequest($"Command with id: {pushCommandResultRequest.CommandId} is not found");
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