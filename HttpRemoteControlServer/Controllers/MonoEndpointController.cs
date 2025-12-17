using System.Text.Json;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Exceptions;
using HttpRemoteControlServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("api/[action]")]
public sealed class MonoEndpointController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly MonoEndpointService _service;

    public MonoEndpointController(ILogger<AdminController> logger, MonoEndpointService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Stats(MonoEndpointDataRequest request)
    {
        try
        {
            _logger.LogInformation("Received MonoEndpointDataRequest. json: {json}",
                JsonSerializer.Serialize(request));
            var response = await _service.Process(request);
            return Ok(response);
        }
        catch (MonoEndpointException e)
        {
            _logger.LogError(
                "Error occurred during executing mono endpoint. Cause: {exMessage}", 
                e.Message);
            return BadRequest(e.Message);

        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unexpected error occurred during executing mono endpoint. Ex: {ex}", 
                e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}