using FluentResults;

namespace StreamOverlay;

public interface IObsService : IHostedService
{
    Result<IReadOnlyCollection<string>> GetSceneNames();
    
    Result ActivateScene(string sceneName);
}
