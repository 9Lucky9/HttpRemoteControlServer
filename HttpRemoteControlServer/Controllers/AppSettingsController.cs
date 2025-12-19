using HttpRemoteControl.Library.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AppSettingsController : ControllerBase
{
    private readonly ILogger<AppSettingsController> _logger;

    [HttpPost]
    public async Task<IActionResult> Update(AppsettingsUpdateRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> RawWrite(AppsetingsRawWriteRequest writeRequest)
    {
        try
        {
            throw new NotImplementedException();
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
}