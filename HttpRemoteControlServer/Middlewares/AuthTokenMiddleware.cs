using HttpRemoteControlServer.Options;
using Microsoft.Extensions.Options;

namespace HttpRemoteControlServer.Middlewares;

public sealed class AuthTokenMiddleware
{
    private readonly RequestDelegate _next;
 
    public AuthTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }
 
    public async Task InvokeAsync(HttpContext context, IOptionsSnapshot<AuthOptions> authOptions)
    {
        //if (!context.Request.Path.StartsWithSegments("/Client") || !context.Request.Path.StartsWithSegments("/Command"))
        //{
        //    await _next.Invoke(context);
        //}
        var token = context.Request.Headers.Authorization;
        if (token!=authOptions.Value.Token)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(NotFoundPage());
        }
        else
        {
            await _next.Invoke(context);
        }
    }

    private static string NotFoundPage()
    {
        return """
               <html>
               <head><title>404 Not Found</title></head>
               <body>
               <center><h1>404 Not Found</h1></center>
               <hr><center>nginx</center>
               </body>
               </html> 
               """;
    }
}