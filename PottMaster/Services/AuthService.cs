using PottMaster;
using PottMaster.Models;
using Supabase;
using System;

namespace PottMaster.Services;

public class AuthService : IAuthService
{
	private Client _client;
	public Client Client => _client;

	public AuthService()
	{
		_client = new Supabase.Client(Constants.SupabaseBaseUrl, Constants.SupabaseAnonKey);
		_client.InitializeAsync();
	}

	public async Task<AuthResponse<bool>> SignUpAsync(string email, string password)
	{
		try
		{
			var session = await Client.Auth.SignUp(email, password);
			if (session == null || session.User == null)
			{
				return new AuthResponse<bool> { Result = AuthResult.SignupFailed, ErrorMessage = "Signup failed. Please try again." };
			}

			return new AuthResponse<bool> { Result = AuthResult.Success, Data = true };
		}
		catch (Exception ex)
		{
			return new AuthResponse<bool> { Result = AuthResult.UnknownError, ErrorMessage = ex.Message };
		}
	}

	public async Task<AuthResponse<bool>> SignInAsync(string email, string password)
	{
		try
		{
			var session = await Client.Auth.SignInWithPassword(email, password);
			if (string.IsNullOrEmpty(session?.AccessToken))
			{
				return new AuthResponse<bool> { Result = AuthResult.InvalidCredentials, ErrorMessage = "Invalid email or password." };
			}
			return new AuthResponse<bool> { Result = AuthResult.Success, Data = true };
		}
		catch (Exception ex)
		{
			return new AuthResponse<bool> { Result = AuthResult.UnknownError, ErrorMessage = ex.Message };
		}
	}

	public async Task<AuthResponse<bool>> SignOutAsync()
	{
		try
		{
			await Client.Auth.SignOut();
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
			var userId = Client.Auth.CurrentUser!.Id;
			var email = Client.Auth.CurrentUser!.Email;
			var response = await Client.From<UserProfiles>()
				.Where(x => x.Id == userId)
				.Single();
			if (response == null)
			{
				// Create profile if not found
				var initials = string.Join("", email!.Split('@')[0].Split('.').Select(s => s.Length >0 ? s[0].ToString().ToUpper() : "").ToArray());
				var newProfile = new UserProfiles
				{
					Id = userId!,
					Initials = initials,
					Email = email,
					CreatedAt = DateTime.Now
				};
				var insertResponse = await Client.From<UserProfiles>().Insert(newProfile);
				if (insertResponse.Models == null || !insertResponse.Models.Any())
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

	public async Task<Supabase.Gotrue.User?> GetCurrentUserAsync()
	{
		return await Task.FromResult(Client.Auth.CurrentUser);
	}

	public bool IsLoggedIn => Client.Auth.CurrentSession != null;
}