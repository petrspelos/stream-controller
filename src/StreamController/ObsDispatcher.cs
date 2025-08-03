using Microsoft.AspNetCore.SignalR;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types.Events;
using StreamOverlay.Hubs;

namespace StreamOverlay;

public class ObsDispatcher(OBSWebsocket obs, IHubContext<ObsHub> hub) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        obs.CurrentProgramSceneChanged += OnCurrentProgramSceneChanged;
        obs.InputMuteStateChanged += OnInputMuteStateChanged;
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        obs.CurrentProgramSceneChanged -= OnCurrentProgramSceneChanged;
        obs.InputMuteStateChanged -= OnInputMuteStateChanged;
        
        return Task.CompletedTask;
    }

    private void OnInputMuteStateChanged(object? sender, InputMuteStateChangedEventArgs e)
    {
        // Fire and forget
        _ = hub.Clients.All.SendAsync("InputMuteStateChanged", e.InputName, e.InputMuted);
    }

    private void OnCurrentProgramSceneChanged(object? sender, ProgramSceneChangedEventArgs e)
    {
        // Fire and forget
        _ = hub.Clients.All.SendAsync("ProgramSceneChanged", e.SceneName);
    }
}
