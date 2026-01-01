using PottMaster;
using PottMaster.Models;
using Supabase;
using Supabase.Gotrue;
using System;
using Microsoft.Extensions.Logging;

namespace PottMaster.Services;

public class AuthService : IAuthService
{
    private readonly Lazy<Task<Supabase.Client>> _clientTask;
    private readonly IDbService _dbService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IDbService dbService, ILogger<AuthService> logger)
    {
        _dbService = dbService;
        _logger = logger;
        _clientTask = new Lazy<Task<Supabase.Client>>(async () =>
        {
            try
            {
                var client = new Supabase.Client(Constants.SupabaseBaseUrl, Constants.SupabaseAnonKey);
                await client.InitializeAsync();
                _logger.LogInformation("Supabase client initialized successfully");
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Supabase client");
                throw;
            }
        });
    }

    private async Task<Supabase.Client> GetClientAsync() => await _clientTask.Value;

    public Supabase.Client Client => _clientTask.Value.Result;

    public bool IsLoggedIn => Client.Auth.CurrentUser != null;

    public async Task<AuthResponse<bool>> SignInAsync(string email, string password)
    {
        try
        {
            var client = await GetClientAsync();
            var session = await client.Auth.SignIn(email, password);

            if (session?.User != null)
            {
                await CacheUserProfileAsync(session.User.Id);
                _logger.LogInformation("User signed in: {Email}", email);
            }

            return new AuthResponse<bool>
            {
                Result = AuthResult.Success,
                Data = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign in failed for email: {Email}", email);
            return new AuthResponse<bool>
            {
                Result = AuthResult.InvalidCredentials,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<AuthResponse<bool>> SignUpAsync(string email, string password)
    {
        try
        {
            var client = await GetClientAsync();
            var session = await client.Auth.SignUp(email, password);

            if (session?.User != null)
            {
                _logger.LogInformation("User signed up: {Email}", email);
                return new AuthResponse<bool>
                {
                    Result = AuthResult.Success,
                    Data = true
                };
            }

            return new AuthResponse<bool>
            {
                Result = AuthResult.SignupFailed,
                ErrorMessage = "Failed to create account"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign up failed for email: {Email}", email);
            return new AuthResponse<bool>
            {
                Result = AuthResult.SignupFailed,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<AuthResponse<bool>> SignOutAsync()
    {
        try
        {
            var client = await GetClientAsync();
            var userId = client.Auth.CurrentUser?.Id;
            await client.Auth.SignOut();
            _logger.LogInformation("User signed out: {UserId}", userId);
            return new AuthResponse<bool> { Result = AuthResult.Success, Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign out failed");
            return new AuthResponse<bool> { Result = AuthResult.UnknownError, ErrorMessage = ex.Message };
        }
    }

    public async Task<AuthResponse<UserProfiles>> GetUserProfileAsync()
    {
        try
        {
            var client = await GetClientAsync();
            var userId = client.Auth.CurrentUser!.Id;
            var email = client.Auth.CurrentUser!.Email;

            var response = await client
                .From<UserProfiles>()
                .Where(x => x.Id == userId)
                .Single();

            if (response == null)
            {
                var newProfile = new UserProfiles
                {
                    Id = userId,
                    Email = email!,
                    Initials = email!.Substring(0, 2).ToUpper(),
                    CreatedAt = DateTime.UtcNow
                };

                var insertResponse = await client.From<UserProfiles>().Insert(newProfile);
                if (insertResponse == null || insertResponse.Models.Count == 0)
                {
                    return new AuthResponse<UserProfiles> { Result = AuthResult.ProfileCreationFailed, ErrorMessage = "Failed to create user profile." };
                }
                
                _logger.LogInformation("User profile created for: {UserId}", userId);
                return new AuthResponse<UserProfiles> { Result = AuthResult.Success, Data = insertResponse.Models.First() };
            }
            
            return new AuthResponse<UserProfiles> { Result = AuthResult.Success, Data = response };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user profile");
            return new AuthResponse<UserProfiles> { Result = AuthResult.UnknownError, ErrorMessage = ex.Message };
        }
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        try
        {
            var client = await GetClientAsync();
            return client.Auth.CurrentUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get current user");
            return null;
        }
    }

    private async Task CacheUserProfileAsync(string userId)
    {
        try
        {
            var client = await GetClientAsync();
            var response = await client.From<UserProfiles>()
                .Where(x => x.Id == userId)
                .Single();

            if (response != null)
            {
                var localProfile = new LocalUserProfile
                {
                    Id = response.Id,
                    Email = response.Email,
                    Initials = response.Initials,
                    CreatedAt = response.CreatedAt,
                    Preferences = "{}",
                    UpdatedAt = DateTime.UtcNow
                };

                await _dbService.UpsertUserProfileAsync(localProfile);
                _logger.LogInformation("User profile cached for: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache user profile: {UserId}", userId);
        }
    }
}