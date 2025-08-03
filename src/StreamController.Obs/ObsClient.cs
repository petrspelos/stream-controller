using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using StreamController.Core;
using StreamController.Core.Options;

namespace StreamController.Obs;

public class ObsClient : IObsClient, IDisposable
{
    private readonly OBSWebsocket _obs;
    private readonly ObsOptions _options;
    private readonly ILogger<ObsClient> _logger;
    
    public ObsClient(
        OBSWebsocket obs,
        IOptions<ObsOptions> options,
        ILogger<ObsClient> logger)
    {
        _obs = obs ?? throw new ArgumentNullException(nameof(obs));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        obs.Connected += OnConnected;
        obs.Disconnected += OnDisconnected;
    }

    private void OnConnected(object? sender, EventArgs e)
    {
        _logger.LogInformation("OBS connected");
    }

    private void OnDisconnected(object? sender, ObsDisconnectionInfo e)
    {
        _logger.LogInformation("OBS disconnected");
    }

    public bool IsConnected => _obs.IsConnected;

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _obs.ConnectAsync(_options.Url, _options.Password);
        
        return Task.CompletedTask;
    }

    public Result<IReadOnlyCollection<string>> GetSceneNames()
        => PerformObsQuery<IReadOnlyCollection<string>>(o 
            => o.GetSceneList().Scenes.Select(info => info.Name).ToList());

    public Result ActivateScene(string sceneName) 
        => PerformObsAction(o 
            => o.SetCurrentProgramScene(sceneName));

    public Result<IReadOnlyCollection<Source>> GetSceneSources(string sceneName)
        => PerformObsQuery<IReadOnlyCollection<Source>>(o
            => o.GetSceneItemList(sceneName)
                .Select(item => new Source(item.ItemId, item.SourceName, item.SourceKind))
                .ToList());

    public Result<string> GetActiveSceneName()
        => PerformObsQuery<string>(o => _obs.GetSceneList().CurrentProgramSceneName);

    private Result<TResult> PerformObsQuery<TResult>(Func<OBSWebsocket, TResult> query)
    {
        if (!_obs.IsConnected)
        {
            _logger.LogError("Failed to perform action - OBS not connected");
            return Result.Fail("Failed to perform action - OBS not connected");
        }

        try
        {
            return query.Invoke(_obs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to perform action - Action failed");
            return Result.Fail("Failed to perform action - Action failed");
        }
    }
    
    private Result PerformObsAction(Action<OBSWebsocket> action)
    {
        if (!_obs.IsConnected)
        {
            _logger.LogError("Failed to perform action - OBS not connected");
            return Result.Fail("Failed to perform action - OBS not connected");
        }

        try
        {
            action.Invoke(_obs);
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to perform action - Action failed");
            return Result.Fail("Failed to perform action - Action failed");
        }
    }

    public void Dispose()
    {
        if (_obs.IsConnected)
            _obs.Disconnect();

        _obs.Connected -= OnConnected;
        _obs.Disconnected -= OnDisconnected;
    }
}
