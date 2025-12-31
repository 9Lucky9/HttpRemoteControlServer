using HttpRemoteControl.Library.Encryptor;
using HttpRemoteControlServer;
using HttpRemoteControlServer.Components;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Middlewares;
using HttpRemoteControlServer.Options;
using HttpRemoteControlServer.Services;
using HttpRemoteControlServer.Services.ClientSide;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

#region Options

builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection(nameof(AuthOptions)));

builder.Services.Configure<MonoEndpointOptions>(
    builder.Configuration.GetSection(nameof(MonoEndpointOptions)));

#endregion

builder.Services.AddTransient<IEncryptor, AesEncryptor>();

builder.Services.AddSingleton<ISessionNotifier, SessionNotifier>();
builder.Services.AddScoped<IRemoteClientManager, RemoteClientManager>();
builder.Services.AddScoped<ISessionManager, SessionManager>();

builder.Services.AddScoped<EncryptedMonoEndpointService>();


builder.Services.AddRouting(options =>
{
    options.ConstraintMap["hyphen"] = typeof(HyphenRouteParameterTransformer);
});
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = 307;
    options.HttpsPort = 8081;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddBlazorBootstrap();

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(o => 
    o.SwaggerEndpoint("/openapi/v1.json", "HttpRemoteControlServer API"));

var sessionManager = app.Services.GetService<ISessionManager>();
await sessionManager!.CreateTestStaticSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
    //app.UseMiddleware<AuthTokenMiddleware>();
    app.MapGet("/", async x =>
    {
        x.Response.Redirect("https://grafana.com", true);
        await Task.CompletedTask;
    });
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//Nah not working, currently disabled
//app.UseMiddleware<EnableBufferingMiddleware>();
//app.UseMiddleware<EncryptionMiddleware>();
//app.UseMiddleware<MonoEndpointMiddleware>();
app.MapControllers();

app.Run();
