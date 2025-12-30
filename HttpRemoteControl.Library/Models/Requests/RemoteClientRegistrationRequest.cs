namespace HttpRemoteControl.Library.Models.Requests;

public sealed class RemoteClientRegistrationRequest
{
    public required Guid UniqueClientId { get; set; }
    public string? Description { get; set; }
    public required MachineInfo MachineInfo { get; set; }
}