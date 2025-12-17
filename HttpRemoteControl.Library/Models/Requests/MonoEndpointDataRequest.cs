namespace HttpRemoteControl.Library.Models.Requests;

public sealed class MonoEndpointDataRequest
{
    public string Method { get; set; }
    public string Payload { get; set; }
}