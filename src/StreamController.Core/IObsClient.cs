using FluentResults;

namespace StreamController.Core;

public interface IObsClient
{
    bool IsConnected { get; }
    
    Task ConnectAsync(CancellationToken cancellationToken = default);
    
    Result<IReadOnlyCollection<string>> GetSceneNames();
    
    Result ActivateScene(string sceneName);
    
    Result<IReadOnlyCollection<Source>> GetSceneSources(string sceneName);
    
    Result<string> GetActiveSceneName();
}
