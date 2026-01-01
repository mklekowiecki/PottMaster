namespace PottMaster.Services;

public interface IAuthStateService
{
    bool IsAuthenticated { get; }
    string? CurrentUserId { get; }
    string? CurrentUserEmail { get; }
    string? UserInitials { get; }
    
    event EventHandler<AuthStateChangedEventArgs>? AuthStateChanged;
    
    Task InitializeAsync();
    Task SetAuthenticatedAsync(string userId, string email, string initials);
    Task ClearAuthenticationAsync();
    Task<bool> RestoreSessionAsync();
}

public class AuthStateChangedEventArgs : EventArgs
{
    public bool IsAuthenticated { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
}
