using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMasterLib.Services;
using PottMaster.Services;
using PottMaster.Resources;

namespace PottMaster.ViewModels;

public partial class SyncStatusViewModel : ObservableObject
{
    private readonly ISyncService _syncService;
    private readonly IAuthStateService _authStateService;
    private readonly IAlertService _alertService;
    private System.Timers.Timer? _updateTimer;

    [ObservableProperty]
    private bool _isAuthenticated;

    [ObservableProperty]
    private bool _isSyncing;

    [ObservableProperty]
    private bool _isOnline;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private int _pendingCount;

    [ObservableProperty]
    private string _statusIcon = "?";

    [ObservableProperty]
    private string _statusText = "";

    public bool HasPendingItems => PendingCount > 0;

    public SyncStatusViewModel(
        ISyncService syncService,
        IAuthStateService authStateService,
        IAlertService alertService)
    {
        _syncService = syncService;
        _authStateService = authStateService;
        _alertService = alertService;

        _authStateService.AuthStateChanged += OnAuthStateChanged;
        Connectivity.ConnectivityChanged += OnConnectivityChanged;

        IsAuthenticated = _authStateService.IsAuthenticated;

        StartUpdateTimer();
    }

    private void OnAuthStateChanged(object? sender, AuthStateChangedEventArgs e)
    {
        IsAuthenticated = e.IsAuthenticated;
        
        if (IsAuthenticated)
        {
            _ = UpdateStatusAsync();
        }
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        _ = UpdateStatusAsync();
    }

    private void StartUpdateTimer()
    {
        _updateTimer = new System.Timers.Timer(5000); // Update every 5 seconds
        _updateTimer.Elapsed += async (s, e) => await UpdateStatusAsync();
        _updateTimer.Start();
    }

    private async Task UpdateStatusAsync()
    {
        if (!IsAuthenticated)
        {
            return;
        }

        try
        {
            IsOnline = await _syncService.IsOnlineAsync();
            PendingCount = await _syncService.GetPendingSyncCountAsync();

            // Determine sync status
            if (IsSyncing)
            {
                StatusIcon = "⟳"; // Syncing
                StatusText = AppResources.Syncing;
            }
            else if (!IsOnline)
            {
                StatusIcon = "○"; // Offline
                StatusText = AppResources.Offline;
            }
            else if (HasError)
            {
                StatusIcon = "⚠"; // Error
                StatusText =  AppResources.SyncError;
            }
            else if (PendingCount > 0)
            {
                StatusIcon = "⟳"; // Pending
                StatusText = String.Format(AppResources.Pending,PendingCount);
            }
            else
            {
                StatusIcon = "✓"; // Synced
                StatusText = AppResources.Synced;
            }

            OnPropertyChanged(nameof(HasPendingItems));
        }
        catch
        {
            // Silently fail status updates
        }
    }

    [RelayCommand]
    private async Task TriggerSyncAsync()
    {
        if (!IsAuthenticated || !IsOnline || IsSyncing)
        {
            return;
        }

        if (PendingCount == 0)
        {
            return;
        }

        try
        {
            IsSyncing = true;
            await UpdateStatusAsync();

            await _syncService.SyncPendingChangesAsync();

            await UpdateStatusAsync();
        }
        catch (Exception ex)
        {
            HasError = true;
        }
        finally
        {
            IsSyncing = false;
            await UpdateStatusAsync();
        }
    }

    public void Dispose()
    {
        _updateTimer?.Stop();
        _updateTimer?.Dispose();
        _authStateService.AuthStateChanged -= OnAuthStateChanged;
        Connectivity.ConnectivityChanged -= OnConnectivityChanged;
    }
}
