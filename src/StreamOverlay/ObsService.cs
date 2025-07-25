using FluentResults;
using Microsoft.Extensions.Options;
using OBSWebsocketDotNet;
using StreamOverlay.Options;

namespace StreamOverlay;

public sealed class ObsService(
    ILogger<ObsService> logger,
    OBSWebsocket obs,
    IOptions<ObsOptions> options
    ) : IObsService
{
    private readonly ObsOptions _obsOptions = options.Value;
    private bool _initialized;

    public Result<IReadOnlyCollection<string>> GetSceneNames()
        => PerformObsQuery<IReadOnlyCollection<string>>(o 
            => o.GetSceneList().Scenes.Select(info => info.Name).ToList());

    public Result ActivateScene(string sceneName) 
        => PerformObsAction(o 
            => o.SetCurrentProgramScene(sceneName));

    private Result<TResult> PerformObsQuery<TResult>(Func<OBSWebsocket, TResult> query)
    {
        if (!_initialized || !obs.IsConnected)
        {
            logger.LogError("Failed to perform action - OBS not connected");
            return Result.Fail("Failed to perform action - OBS not connected");
        }

        try
        {
            return query.Invoke(obs);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to perform action - Action failed");
            return Result.Fail("Failed to perform action - Action failed");
        }
    }
    
    private Result PerformObsAction(Action<OBSWebsocket> action)
    {
        if (!_initialized || !obs.IsConnected)
        {
            logger.LogError("Failed to perform action - OBS not connected");
            return Result.Fail("Failed to perform action - OBS not connected");
        }

        try
        {
            action.Invoke(obs);
            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to perform action - Action failed");
            return Result.Fail("Failed to perform action - Action failed");
        }
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (obs.IsConnected || _initialized)
        {
            logger.LogWarning("Already connected / initialized");
            return Task.CompletedTask;
        }

        try
        {
            obs.Connected += OnObsConnected;
            obs.ConnectAsync(_obsOptions.Url, _obsOptions.Password);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to connect to OBS");
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            obs.Connected -= OnObsConnected;
            obs.Disconnect();
            _initialized = false;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to disconnect from OBS");
        }

        return Task.CompletedTask;
    }
    
    private void OnObsConnected(object? sender, EventArgs e)
    {
        logger.LogDebug("OBS connected");
        _initialized = true;
    }
}
