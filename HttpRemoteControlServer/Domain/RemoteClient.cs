using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using HttpRemoteControl.Library.Models;
using HttpRemoteControl.Library.Models.Requests;
using HttpRemoteControlServer.Exceptions;

namespace HttpRemoteControlServer.Domain;

public class RemoteClient
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public MachineInfo MachineInfo { get; set; }
    public virtual List<RemoteClientSession> Sessions { get; set; }

    public static RemoteClient Create(Guid id, string? description, MachineInfo machineInfo)
    {
        if(id == Guid.Empty)
            throw new ArgumentException("Remote client Id cannot be empty");
        return new RemoteClient()
        {
            Id = id,
            Description = description,
            MachineInfo = machineInfo
        };
    }
}