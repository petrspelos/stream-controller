using Polly;
using StreamController.Core;

namespace StreamOverlay;

public class ObsConnectionHostedService(
    IObsClient obsClient,
    ILogger<ObsConnectionHostedService> logger
) : IHostedService
{
    private Task? _connectionTask;
    private CancellationTokenSource? _cts;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // TODO: Reconnect on Disconnect event from the client
        var policy = Policy
            .HandleResult<bool>(connected => !connected)
            .WaitAndRetryForeverAsync(
                _ => TimeSpan.FromSeconds(2),
                (outcome, timespan) => logger.LogInformation("OBS Not Connected - Retrying...")
            );

        _connectionTask = Task.Run(async () =>
        {
            await policy.ExecuteAsync(async token =>
            {
                logger.LogInformation("Connecting to OBS...");
                await obsClient.ConnectAsync(token);
                await Task.Delay(TimeSpan.FromSeconds(1), token);
                return obsClient.IsConnected;
            }, _cts.Token);
        }, CancellationToken.None);
        
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts is not null)
            await _cts.CancelAsync();

        if (_connectionTask is not null)
        {
            try
            {
                await _connectionTask;
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation
            }
        }
    }
}
