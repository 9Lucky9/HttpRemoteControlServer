namespace HttpRemoteControlServer.Options;

public sealed class MonoEndpointOptions
{
    public string Path { get; set; }
    public bool EncryptionEnabled { get; set; }
    public bool EncryptPayload { get; set; }
    public string Key { get; set; }
}