using Microsoft.AspNetCore.Mvc;

namespace HttpRemoteControlServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AppSettingsController : ControllerBase
{
    private readonly ILogger<ClientController> _logger;
    
}