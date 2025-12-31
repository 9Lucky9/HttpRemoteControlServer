using HttpRemoteControl.Library.Models;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Domain;
using HttpRemoteControlServer.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("[controller]/{sessionId}/[action]")]
public sealed class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Log(LogDto logDto)
    {
        try
        {
            _logger.LogInformation("Received request to post log.");
            throw new NotImplementedException();
            //await _RemoteSessionService.WriteLogToClient(logDto);
            return NoContent();
        }
        catch (EntityNotFoundException<RemoteClientSession>)
        {
            _logger.LogError(
                "RemoteSession with id {id} is not found.", logDto.SessionId);
            return BadRequest($"RemoteSession with id {logDto.SessionId} is not found.");
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
            _logger.LogInformation(
                "Received request to get all logs from session. Id: {sessionId}", sessionId);
            var guid = Guid.Parse(sessionId);
            //var logs =
            //    await _RemoteSessionService.GetLogsFromRemoteSession(guid);
            return Ok();
            // Ok(logs);
        }
        catch (EntityNotFoundException<RemoteClientSession>)
        {
            _logger.LogError(
                "Get all logs from remoteSession with Id: {sessionId} is not found.", sessionId);
            return BadRequest($"RemoteSession with Id: {sessionId} is not found.");   
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