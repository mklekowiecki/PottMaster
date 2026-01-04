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
    private readonly IAuthStateService _authStateService;
    private readonly ILogger<BackgroundSyncWorker> _logger;
    private readonly PeriodicTimer _timer;
    private CancellationTokenSource _cts = new();
    private Task? _workerTask;
    private const int SyncIntervalSeconds = 300; // 5 minutes; adjust if configuration support is added

    public BackgroundSyncWorker(ISyncService syncService, IDryingMonitorService dryingMonitor, IAuthStateService authStateService, ILogger<BackgroundSyncWorker> logger)
    {
        _syncService = syncService;
        _dryingMonitor = dryingMonitor;
        _authStateService = authStateService;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(SyncIntervalSeconds));
        _authStateService.AuthStateChanged += OnAuthStateChanged;
    }

    private void OnAuthStateChanged(object? sender, AuthStateChangedEventArgs e)
    {
        if (e.IsAuthenticated)
        {
            Start();
        }
        else
        {
            Stop();
        }
    }

    public void Start()
    {
        if (_workerTask != null && !_workerTask.IsCompleted) return;
        _cts = new CancellationTokenSource();
        _workerTask = RunAsync(_cts.Token);
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background sync worker started.");
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _syncService.SyncPendingChangesAsync();
                await _syncService.SyncWorksFromServerAsync(_authStateService.CurrentUserId!);
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
