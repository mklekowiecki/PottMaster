using PottMaster;
using PottMaster.Models;
using Supabase;
using Supabase.Gotrue;
using System;

namespace PottMaster.Services;

public class AuthService : IAuthService
{
    private readonly Lazy<Task<Supabase.Client>> _clientTask;
    private readonly IDbService _dbService;

    public AuthService(IDbService dbService)
    {
        _dbService = dbService;
        _clientTask = new Lazy<Task<Supabase.Client>>(async () =>
        {
            var client = new Supabase.Client(Constants.SupabaseBaseUrl, Constants.SupabaseAnonKey);
            await client.InitializeAsync();
            return client;
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
            }

            return new AuthResponse<bool>
            {
                Result = AuthResult.Success,
                Data = true
            };
        }
        catch (Exception ex)
        {
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
            await client.Auth.SignOut();
            return new AuthResponse<bool> { Result = AuthResult.Success, Data = true };
        }
        catch (Exception ex)
        {
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
                return new AuthResponse<UserProfiles> { Result = AuthResult.Success, Data = insertResponse.Models.First() };
            }
            return new AuthResponse<UserProfiles> { Result = AuthResult.Success, Data = response };
        }
        catch (Exception ex)
        {
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
        catch
        {
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
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to cache user profile: {ex.Message}");
        }
    }
}