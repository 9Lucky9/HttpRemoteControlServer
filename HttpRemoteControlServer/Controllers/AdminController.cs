using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly IClientSessionService _clientSessionService;

    public AdminController(ILogger<AdminController> logger, IClientSessionService clientSessionService)
    {
        _logger = logger;
        _clientSessionService = clientSessionService;
    }

    [HttpPost]
    public async Task<IActionResult> EnqueueCommand(CommandEnqueueRequest commandEnqueueRequest)
    {
        try
        {
            _logger.LogInformation("Received request to enqueue command.");
            await _clientSessionService.EnqueueCommand(commandEnqueueRequest);
            return NoContent();
        }
        catch (ArgumentException)
        {
            _logger.LogInformation("Received request to enqueue command was invalid.");
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unexpected error occured during enqueue of command. Ex: {ex}", 
                e);
            return StatusCode(StatusCodes.Status500InternalServerError); 
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCommands(Guid sessionId)
    {
        try
        {
            _logger.LogInformation("Received request to retrieve all commands from session.");
            var commands = await _clientSessionService.GetCommandQueueFromSession(sessionId);
            return Ok(commands);
        }
        catch (ClientSessionNotFoundException)
        {
            _logger.LogError("Session with id: {sessionId} not found.", sessionId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unexpected error occurred during retrieval of commands. Ex: {ex}", 
                e);
            return StatusCode(StatusCodes.Status500InternalServerError); 
        }
    }
}