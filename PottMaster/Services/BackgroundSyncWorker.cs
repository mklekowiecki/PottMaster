using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PottMasterLib.Services;

namespace PottMaster.Services;

public class BackgroundSyncWorker : IAsyncDisposable
{
    private readonly ISyncService _syncService;
    private readonly IDryingMonitorService _dryingMonitor;
    private readonly ILogger<BackgroundSyncWorker> _logger;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();
    private Task? _workerTask;
    private const int SyncIntervalSeconds = 300; // 5 minutes; adjust if configuration support is added

    public BackgroundSyncWorker(ISyncService syncService, IDryingMonitorService dryingMonitor, ILogger<BackgroundSyncWorker> logger)
    {
        _syncService = syncService;
        _dryingMonitor = dryingMonitor;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(SyncIntervalSeconds));
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
                _logger.LogError(ex, "Background sync of pending changes failed.");
            }

            try
            {
                await _dryingMonitor.CheckAndAdvanceCompletedWorksAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background drying monitor check failed.");
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
