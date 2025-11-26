using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("[controller]/{sessionId}/[action]")]
public sealed class ClientSessionController : ControllerBase
{
    private readonly ILogger<ClientSessionController> _logger;
    private readonly IClientSessionService _clientSessionService;

    public ClientSessionController(ILogger<ClientSessionController> logger, IClientSessionService clientSessionService)
    {
        _logger = logger;
        _clientSessionService = clientSessionService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCommands(Guid sessionId)
    {
        try
        {
            _logger.LogInformation("Received request to retrieve all commands from session.");
            var commands = 
                await _clientSessionService.GetCommandQueueFromSession(sessionId);
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