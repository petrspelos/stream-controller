using FluentResults;

namespace StreamController.Core;

public interface IObsClient
{
    Task ConnectAsync(CancellationToken cancellationToken = default);
    
    Result<IReadOnlyCollection<string>> GetSceneNames();
    
    Result ActivateScene(string sceneName);
    
    Result<IReadOnlyCollection<Source>> GetSceneSources(string sceneName);
}
