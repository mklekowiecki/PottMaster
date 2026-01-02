using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PottMaster.Services;

public class BackgroundSyncWorker : IAsyncDisposable
{
    private readonly ISyncService _syncService;
    private readonly ILogger<BackgroundSyncWorker> _logger;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();
    private Task? _workerTask;
    private const int SyncIntervalMinutes = 15;

    public BackgroundSyncWorker(ISyncService syncService, ILogger<BackgroundSyncWorker> logger)
    {
        _syncService = syncService;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromMinutes(SyncIntervalMinutes));
    }

    public void Start()
    {
        _workerTask = RunAsync(_cts.Token);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background sync worker started.");
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _syncService.SyncPendingChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background sync failed.");
            }
            await _timer.WaitForNextTickAsync(cancellationToken);
        }
        _logger.LogInformation("Background sync worker stopped.");
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        if (_workerTask != null)
            await _workerTask;
        _timer.Dispose();
        _cts.Dispose();
    }
}
