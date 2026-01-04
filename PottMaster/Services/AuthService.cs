using PottMaster;
using Supabase;
using Supabase.Gotrue;
using System;
using Microsoft.Extensions.Logging;
using PottMasterLib.Services;
using PottMasterLib.Models;

namespace PottMaster.Services;

public class AuthService : IAuthService
{
    private readonly Supabase.Client _client;
    private readonly IDbService _dbService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(Supabase.Client client, IDbService dbService, ILogger<AuthService> logger)
    {
        _client = client;
        _dbService = dbService;
        _logger = logger;
    }

    public Supabase.Client Client => _client;

    public bool IsLoggedIn => _client.Auth.CurrentUser != null;

    public async Task<AuthResponse<bool>> SignInAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignIn(email, password);

            if (session?.User != null)
            {
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
            var session = await _client.Auth.SignUp(email, password);

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
            var userId = _client.Auth.CurrentUser?.Id;
            await _client.Auth.SignOut();
            _logger.LogInformation("User signed out: {UserId}", userId);
            return new AuthResponse<bool> { Result = AuthResult.Success, Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign out failed");
            return new AuthResponse<bool> { Result = AuthResult.UnknownError, ErrorMessage = ex.Message };
        }
    }

    public async Task<AuthResponse<LocalUserProfile>> GetUserProfileAsync()
    {
        try
        {
            var userId = _client.Auth.CurrentUser!.Id;
            var email = _client.Auth.CurrentUser!.Email;

            var response = await _client.From<UserProfiles>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId).Get();
            var remoteProfile = new UserProfiles();

            if (response.Models.Count() == 0)
            {
                remoteProfile = new UserProfiles
                {
                    Id = userId!,
                    Email = email!,
                    Initials = email!.Substring(0, 2).ToUpper(),
                    CreatedAt = DateTime.UtcNow
                };
                //Save the new profile to the remote database
                var insertResponseRemote = await _client.From<UserProfiles>().Insert(remoteProfile);
                if (insertResponseRemote == null)
                {
                    return new AuthResponse<LocalUserProfile> { Result = AuthResult.ProfileCreationFailed, ErrorMessage = "Failed to create user profile." };
                }

                _logger.LogInformation("User profile created for: {UserId}", userId);
            }
            else
                remoteProfile = response.Models[0];

            var newProfile = new LocalUserProfile
            {
                Id = remoteProfile.Id,
                Email = remoteProfile.Email!,
                Initials = remoteProfile.Initials!,
                CreatedAt = remoteProfile.CreatedAt
            };

            var insertResponse = await _dbService.UpsertUserProfileAsync(newProfile);
            if (insertResponse <= 0)
            {
                return new AuthResponse<LocalUserProfile> { Result = AuthResult.ProfileCreationFailed, ErrorMessage = "Failed to create user profile." };
            }

            return new AuthResponse<LocalUserProfile> { Result = AuthResult.Success, Data = newProfile };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user profile");
            return new AuthResponse<LocalUserProfile> { Result = AuthResult.UnknownError, ErrorMessage = ex.Message };
        }
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        try
        {
            return _client.Auth.CurrentUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get current user");
            return null;
        }
    }

    public async Task ClearSession()
    {
        await _client.Auth.SignOut();
    }
}