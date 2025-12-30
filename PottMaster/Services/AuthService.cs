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
	    if (session == null)
	    {
	        throw new Exception("Signup failed. Please try again.");
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
	
	public async Task<UserProfile> GetUserProfileAsync()
	{
	    var response = await Client.From<UserProfile>()
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