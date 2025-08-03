using Microsoft.AspNetCore.SignalR;
using StreamController.Core;

namespace StreamOverlay.Hubs;

public class ObsHub(IObsClient obs) : Hub
{
    public Task GetSceneSources(string sceneName)
    {
        var sourcesResult = obs.GetSceneSources(sceneName);

        return sourcesResult.IsSuccess
            ? Clients.Caller.SendAsync("SceneSourcesReceived", sourcesResult.Value)
            : Task.CompletedTask;
    }
}
