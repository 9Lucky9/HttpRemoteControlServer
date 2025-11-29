using HttpRemoteControl.Library.Encryptor;
using HttpRemoteControlServer;
using HttpRemoteControlServer.Components;
using HttpRemoteControlServer.Contracts;
using HttpRemoteControlServer.Middlewares;
using HttpRemoteControlServer.Options;
using HttpRemoteControlServer.Services;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection(nameof(AuthOptions)));

builder.Services.Configure<EncryptOptions>(
    builder.Configuration.GetSection(nameof(EncryptOptions)));

//builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IClientSessionService, ClientSessionService>();
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IEncryptor, AesEncryptor>();

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = 307;
    options.HttpsPort = 8081;
});

builder.Services.AddOpenApi();

builder.Services.AddControllers(opts =>
{
    opts.Conventions.Add(new RouteTokenTransformerConvention(
        new HyphenRouteParameterTransformer()));
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBootstrap();

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(o => 
    o.SwaggerEndpoint("/openapi/v1.json", "HttpRemoteControlServer API"));

var clientSessionService = app.Services.GetRequiredService<IClientSessionService>();
await clientSessionService.CreateTestStaticClientSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
    app.UseMiddleware<AuthTokenMiddleware>();
    app.MapGet("/", async x =>
    {
        x.Response.Redirect("https://grafana.com", true);
        await Task.CompletedTask;
    });
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseMiddleware<EnableBufferingMiddleware>();
app.UseMiddleware<DecryptorMiddleware>();

app.MapControllers();

app.UseAntiforgery();

app.Run();