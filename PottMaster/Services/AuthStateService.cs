using Microsoft.Maui.Storage;
using Microsoft.Extensions.Logging;

namespace PottMaster.Services;

public class AuthStateService : IAuthStateService
{
    private const string UserIdKey = "auth_user_id";
    private const string UserEmailKey = "auth_user_email";
    private const string UserInitialsKey = "auth_user_initials";
    private const string SessionTokenKey = "auth_session_token";
    
    private readonly IAuthService _authService;
    private readonly ILogger<AuthStateService> _logger;
    
    private bool _isAuthenticated;
    private string? _currentUserId;
    private string? _currentUserEmail;
    private string? _userInitials;
    
    public bool IsAuthenticated => _isAuthenticated;
    public string? CurrentUserId => _currentUserId;
    public string? CurrentUserEmail => _currentUserEmail;
    public string? UserInitials => _userInitials;
    
    public event EventHandler<AuthStateChangedEventArgs>? AuthStateChanged;
    
    public AuthStateService(IAuthService authService, ILogger<AuthStateService> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    public async Task InitializeAsync()
    {
        try
        {
            await RestoreSessionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize auth state");
        }
    }
    
    public async Task SetAuthenticatedAsync(string userId, string email, string initials)
    {
        _currentUserId = userId;
        _currentUserEmail = email;
        _userInitials = initials;
        _isAuthenticated = true;
        
        // Store in secure storage
        await SecureStorage.Default.SetAsync(UserIdKey, userId);
        await SecureStorage.Default.SetAsync(UserEmailKey, email);
        await SecureStorage.Default.SetAsync(UserInitialsKey, initials);
        
        // Also store in preferences for easy access
        Preferences.Default.Set(UserInitialsKey, initials);
        
        _logger.LogInformation("User authenticated: {UserId}", userId);
        
        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs
        {
            IsAuthenticated = true,
            UserId = userId,
            Email = email
        });
    }
    
    public async Task ClearAuthenticationAsync()
    {
        _currentUserId = null;
        _currentUserEmail = null;
        _userInitials = null;
        _isAuthenticated = false;
        
        // Clear secure storage
        SecureStorage.Default.Remove(UserIdKey);
        SecureStorage.Default.Remove(UserEmailKey);
        SecureStorage.Default.Remove(UserInitialsKey);
        SecureStorage.Default.Remove(SessionTokenKey);
        
        // Clear preferences
        Preferences.Default.Remove(UserInitialsKey);
        
        _logger.LogInformation("User logged out");
        
        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs
        {
            IsAuthenticated = false,
            UserId = null,
            Email = null
        });
        
        await Task.CompletedTask;
    }
    
    public async Task<bool> RestoreSessionAsync()
    {
        try
        {
            var userId = await SecureStorage.Default.GetAsync(UserIdKey);
            var email = await SecureStorage.Default.GetAsync(UserEmailKey);
            var initials = await SecureStorage.Default.GetAsync(UserInitialsKey);
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            {
                return false;
            }
            
            // Verify session is still valid with Supabase
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null || currentUser.Id != userId)
            {
                await ClearAuthenticationAsync();
                return false;
            }
            
            _currentUserId = userId;
            _currentUserEmail = email;
            _userInitials = initials;
            _isAuthenticated = true;
            
            _logger.LogInformation("Session restored for user: {UserId}", userId);
            
            AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs
            {
                IsAuthenticated = true,
                UserId = userId,
                Email = email
            });
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore session");
            await ClearAuthenticationAsync();
            return false;
        }
    }
}
