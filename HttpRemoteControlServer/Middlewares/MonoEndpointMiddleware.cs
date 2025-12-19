using System.Text;
using System.Text.Json;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControl.Library.Models.Responses;
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
        
        //Route MonoEndpointDataRequest to appropriate controller
        //Set body to payload from request
        var rawRequestBody = 
            await new StreamReader(context.Request.Body).ReadToEndAsync();
        var monoEndpointDataRequest = 
            JsonSerializer.Deserialize<MonoEndpointDataRequest>(rawRequestBody);
        context.Request.Path = monoEndpointDataRequest.Path; 
        context.Request.Body = 
            new MemoryStream(Encoding.UTF8.GetBytes(monoEndpointDataRequest.Payload));
        
        await _next(context);
        
        //Create MonoEndpointDataResponse
        //Set original path
        //Set payload to body from response 
        var rawResponseBody = 
            await new StreamReader(context.Response.Body).ReadToEndAsync();
        var monoEndpointResponse = new MonoEndpointDataResponse()
        {
            Path = monoEndpointDataRequest.Path,
            Payload = rawResponseBody,
        };
        var responseJson = JsonSerializer.Serialize(monoEndpointResponse);
        await context.Response.Body.WriteAsync(
            Encoding.UTF8.GetBytes(responseJson));
    }
}