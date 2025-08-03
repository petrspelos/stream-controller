using Microsoft.AspNetCore.SignalR;
using StreamController.Core;

namespace StreamOverlay.Hubs;

public class ObsHub(
    IObsClient obs,
    ILogger<ObsHub> logger
    ) : Hub
{
    public Task RequestScenes()
    {
        var scenesResult = obs.GetSceneNames();

        return scenesResult.IsSuccess
            ? Clients.Caller.SendAsync("Scenes", scenesResult.Value)
            : Task.CompletedTask;
    }

    public Task RequestActiveScene()
    {
        var scenesResult = obs.GetActiveSceneName();

        return scenesResult.IsSuccess
            ? Clients.Caller.SendAsync("ActiveScene", scenesResult.Value)
            : Task.CompletedTask;
    }
    
    public Task GetSceneSources(string sceneName)
    {
        var sourcesResult = obs.GetSceneSources(sceneName);

        return sourcesResult.IsSuccess
            ? Clients.Caller.SendAsync("SceneSources", sourcesResult.Value)
            : Task.CompletedTask;
    }
}
