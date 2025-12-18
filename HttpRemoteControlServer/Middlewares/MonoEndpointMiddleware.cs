using System.Text.Json;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Options;
using Microsoft.Extensions.Options;

namespace HttpRemoteControlServer.Middlewares;

public sealed class MonoEndpointMiddleware
{
    private readonly RequestDelegate _next;
    
    public MonoEndpointMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context, 
        IOptionsSnapshot<MonoEndpointOptions> monoOptions)
    {
        if (context.Request.Path != monoOptions.Value.Path)
        {
            await _next(context);
            return;
        }

        var pathFromRequest = 
            JsonSerializer.Deserialize<MonoEndpointDataRequest>(context.Request.Body);
        context.Request.Path = pathFromRequest.Path;
        await _next(context);
    }
}