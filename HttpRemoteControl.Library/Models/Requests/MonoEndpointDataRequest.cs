namespace HttpRemoteControl.Library.Models.Requests;

public sealed class MonoEndpointDataRequest
{
    public string Path { get; set; }
    public string Payload { get; set; }
}