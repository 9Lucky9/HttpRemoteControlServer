using HttpRemoteControlServer.Domain;
using Microsoft.EntityFrameworkCore;

namespace HttpRemoteControlServer.DbContexts;

public sealed class ServerContext : DbContext
{
    public DbSet<RemoteClient> RemoteClients { get; set; }
    public DbSet<RemoteClientSession> RemoteSessions { get; set; }
    public DbSet<Command> Commands { get; set; }

    public ServerContext(DbContextOptions options) : base(options)
    {
        
    }
}