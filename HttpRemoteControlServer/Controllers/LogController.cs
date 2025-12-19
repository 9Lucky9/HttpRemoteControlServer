using HttpRemoteControl.Library.Models;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("[controller]/{sessionId}/[action]")]
public sealed class LogController : ControllerBase
{
    private readonly ILogger<ClientSessionController> _logger;
    private readonly IClientSessionService _clientSessionService;

    public LogController(ILogger<ClientSessionController> logger, IClientSessionService clientSessionService)
    {
        _logger = logger;
        _clientSessionService = clientSessionService;
    }

    [HttpPost]
    public async Task<IActionResult> Log(LogDto logDto)
    {
        try
        {
            _logger.LogInformation("Received request to post log.");
            throw new NotImplementedException();
            //await _clientSessionService.WriteLogToClient(logDto);
            return NoContent();
        }
        catch (ClientSessionNotFoundException)
        {
            _logger.LogError(
                "ClientSession with id {id} is not found.", logDto.SessionId);
            return BadRequest($"ClientSession with id {logDto.SessionId} is not found.");
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unexpected error occurred during request to post log. Ex: {ex}", 
                e);
            return StatusCode(StatusCodes.Status500InternalServerError); 
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logs([FromQuery] string sessionId)
    {
        try
        {
            _logger.LogInformation("Received request to post log.");
            var guid = Guid.Parse(sessionId);
            //var logs =
            //    await _clientSessionService.GetLogsFromClientSession(guid);
            return Ok();
            // Ok(logs);
        }
        catch (ClientSessionNotFoundException)
        {
            _logger.LogError(
                "Get all logs from clientSession with sessionId: {sessionId} is not found.", sessionId);
            return BadRequest($"ClientSession with sessionId: {sessionId} is not found.");   
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unexpected error occurred during request to get all logs from. Ex: {ex}", 
                e);
            return StatusCode(StatusCodes.Status500InternalServerError); 
        }
    }

}