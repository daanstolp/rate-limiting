using System.Diagnostics;

namespace Caller;

public class Worker(
    ILogger<Worker> logger,
    CounterGateway counterGateway,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds <= 5_000)
            {
                await counterGateway.Count();
            }
            var stats = await counterGateway.GetStats();
            logger.LogInformation("Total requests: {TotalRequests}, Requests per second: {RequestsPerSecond}", 
                stats.TotalRequests, stats.RequestsPerSecond);

            hostApplicationLifetime.StopApplication();
        }
    }
}
