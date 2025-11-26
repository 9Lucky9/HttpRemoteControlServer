using System.Text;
using HttpRemoteControl.Library.Encryptor;
using HttpRemoteControlServer.Options;
using Microsoft.Extensions.Options;

namespace HttpRemoteControlServer.Middlewares;

public sealed class DecryptorMiddleware
{
    private readonly RequestDelegate _next;

    public DecryptorMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(
        HttpContext context, 
        IOptionsSnapshot<EncryptOptions> encryptOptions, 
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
        if (!encrypted)
        {
            await _next.Invoke(context);
            return;
        }

        context.Request.Body.Position = 0;
        var rawRequestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var decryptedPayload = encryptor.Decrypt(
            encryptOptions.Value.Key,
            rawRequestBody);
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(decryptedPayload));
        await _next.Invoke(context);
    }
}