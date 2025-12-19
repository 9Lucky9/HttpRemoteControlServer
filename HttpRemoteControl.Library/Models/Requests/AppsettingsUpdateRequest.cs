namespace HttpRemoteControl.Library.Models.Requests;

public sealed class AppsettingsUpdateRequest
{
    public string Path { get; set; }
    public object NewValue { get; set; }
}