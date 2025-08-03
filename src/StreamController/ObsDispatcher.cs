using Microsoft.AspNetCore.SignalR;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using OBSWebsocketDotNet.Types.Events;
using StreamOverlay.Hubs;

namespace StreamOverlay;

public class ObsDispatcher(OBSWebsocket obs, IHubContext<ObsHub> hub) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        obs.CurrentProgramSceneChanged += OnCurrentProgramSceneChanged;
        obs.InputMuteStateChanged += OnInputMuteStateChanged;
        obs.Connected += OnConnected;
        obs.Disconnected += OnDisconnected;
        obs.SceneItemEnableStateChanged += OnSceneItemEnableStateChanged;
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        obs.CurrentProgramSceneChanged -= OnCurrentProgramSceneChanged;
        obs.InputMuteStateChanged -= OnInputMuteStateChanged;
        obs.Connected -= OnConnected;
        obs.Disconnected -= OnDisconnected;
        obs.SceneItemEnableStateChanged -= OnSceneItemEnableStateChanged;
        
        return Task.CompletedTask;
    }

    private void OnSceneItemEnableStateChanged(object? sender, SceneItemEnableStateChangedEventArgs e)
    {
        // Fire and forget
        _ = hub.Clients.All.SendAsync("SourceEnableStateChanged", e.SceneItemId, e.SceneItemEnabled);
        throw new NotImplementedException();
    }
    
    private void OnDisconnected(object? sender, ObsDisconnectionInfo e)
    {
        // Fire and forget
        _ = hub.Clients.All.SendAsync("ObsDisconnected");
    }

    private void OnConnected(object? sender, EventArgs e)
    {
        // Fire and forget
        _ = hub.Clients.All.SendAsync("ObsConnected");
    }

    private void OnInputMuteStateChanged(object? sender, InputMuteStateChangedEventArgs e)
    {
        // Fire and forget
        _ = hub.Clients.All.SendAsync("InputMuteStateChanged", e.InputName, e.InputMuted);
    }

    private void OnCurrentProgramSceneChanged(object? sender, ProgramSceneChangedEventArgs e)
    {
        // Fire and forget
        _ = hub.Clients.All.SendAsync("ActiveScene", e.SceneName);
    }
}
