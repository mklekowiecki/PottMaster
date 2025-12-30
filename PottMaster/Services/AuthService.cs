using PottMaster;
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
        if (session?.TokenType != null)
        {
            throw new Exception("Error");
        }
    }

    public async Task SignInAsync(string email, string password)
    {
        var session = await Client.Auth.SignInWithPassword(email, password);
        if (String.IsNullOrEmpty(session?.AccessToken))
        {
			throw new Exception("Error");
		}
    }

    public async Task SignOutAsync()
    {
        await Client.Auth.SignOut();
    }

    public bool IsLoggedIn => Client.Auth.CurrentSession != null;
}