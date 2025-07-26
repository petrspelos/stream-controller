using Microsoft.AspNetCore.SignalR;

namespace StreamOverlay.Hubs;

public class ObsHub : Hub
{
    public Task Ping() => Clients.Caller.SendAsync("PongReceived");
}
