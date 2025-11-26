namespace HttpRemoteControlServer.Middlewares;

public sealed class EnableBufferingMiddleware
{
    private readonly RequestDelegate _next;

    public EnableBufferingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try { context.Request.EnableBuffering(); } catch { }
        await _next(context);
        // context.Request.Body.Dipose() might be added to release memory, not tested
    }
}