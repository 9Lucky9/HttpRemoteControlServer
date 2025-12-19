using System.Text;
using HttpRemoteControl.Library.Encryptor;
using HttpRemoteControlServer.Options;
using Microsoft.Extensions.Options;

namespace HttpRemoteControlServer.Middlewares;

public sealed class EncryptionMiddleware
{
    private readonly RequestDelegate _next;

    public EncryptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(
        HttpContext context, 
        IOptionsSnapshot<MonoEndpointOptions> monoOptions, 
        IEncryptor encryptor)
    {
        var containsEncryption = 
            context.Request.Headers.TryGetValue("Encrypted", out var encryptedValues);
        if (!containsEncryption)
        {
            await _next.Invoke(context);
            return;
        }

        var encrypted = bool.Parse(encryptedValues.ToString());
        if (!encrypted || context.Request.Path == "/client/register-me")
        {
            await _next.Invoke(context);
            return;
        }

        //Decrypt
        context.Request.Body.Position = 0;
        var rawRequestBody = 
            await new StreamReader(context.Request.Body).ReadToEndAsync();
        var decryptedPayload = encryptor.Decrypt(
            monoOptions.Value.Key,
            rawRequestBody);
        context.Request.Body = 
            new MemoryStream(Encoding.UTF8.GetBytes(decryptedPayload));
        
        await _next.Invoke(context);
        
        //Encrypt
        context.Request.Body.Position = 0;
        var rawResponseBody = 
            await new StreamReader(context.Request.Body).ReadToEndAsync();
        var encryptedResponse = encryptor.Encrypt(
            monoOptions.Value.Key,
            rawResponseBody);
        context.Response.Body = 
            new MemoryStream(Encoding.UTF8.GetBytes(encryptedResponse));
    }
}