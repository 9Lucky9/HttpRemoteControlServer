using HttpRemoteControlServer.Middlewares;

namespace HttpRemoteControlServer.Extensions;

public static class EnableBufferingMiddlewareExtensions
{
    public static IApplicationBuilder EnableBufferingMiddleware(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<EnableBufferingMiddleware>();
    }

}