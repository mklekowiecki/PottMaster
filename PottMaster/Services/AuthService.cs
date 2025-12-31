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

	public async Task SignUpAsync(string email, string password)
	{
	    var session = await Client.Auth.SignUp(email, password);
	    if (session == null || session.User == null)
	    {
	        throw new Exception("Signup failed. Please try again.");
	    }

	    // Create user profile after successful signup
	    var userId = session.User.Id;
	    var initials = string.Join("", email.Split('@')[0].Split('.').Select(s => s.Length >0 ? s[0].ToString().ToUpper() : "").ToArray());
	    var profile = new UserProfiles
	    {
	        Id = userId,
	        Initials = initials
	    };
	    var response = await Client.From<UserProfiles>().Insert(profile);
	    if (response.Models == null || !response.Models.Any())
	    {
	        throw new Exception("Failed to create user profile.");
	    }
	}
	
	public async Task SignInAsync(string email, string password)
	{
	    var session = await Client.Auth.SignInWithPassword(email, password);
	    if (String.IsNullOrEmpty(session?.AccessToken))
	    {
	        throw new Exception("Invalid email or password.");
	    }
	}
	
	public async Task SignOutAsync()
	{
	    await Client.Auth.SignOut();
	}
	
	public async Task<UserProfiles> GetUserProfileAsync()
	{
	    var response = await Client.From<UserProfiles>()
	        .Where(x => x.Id == Client.Auth.CurrentUser!.Id)
	        .Single();
	    if (response == null)
	    {
	        throw new Exception("User profile not found.");
	    }
	    return response;
	}
	
	public bool IsLoggedIn => Client.Auth.CurrentSession != null;
}