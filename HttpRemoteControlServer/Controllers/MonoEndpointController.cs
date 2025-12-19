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
    private readonly EncryptedMonoEndpointService _service;

    public MonoEndpointController(ILogger<AdminController> logger, EncryptedMonoEndpointService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Stats([FromBody]string encryptedJson)
    {
        try
        {
            _logger.LogInformation("Received MonoEndpointDataRequest. encrypted json: {json}",
                JsonSerializer.Serialize(encryptedJson));
            var response = await _service.ProcessEncryptedJson(encryptedJson);
            return Ok(response);
        }
        catch (MonoEndpointException e)
        {
            _logger.LogError(
                "Error occurred during executing mono endpoint. Ex: {ex}", 
                e);
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