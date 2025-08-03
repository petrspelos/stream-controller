using StreamController.Core;

namespace StreamOverlay;

public class ObsConnectionHostedService(
    IObsClient obsClient,
    ILogger<ObsConnectionHostedService> logger
    ) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Connecting to OBS...");
        await obsClient.ConnectAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
