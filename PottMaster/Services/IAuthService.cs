using Supabase;
using System.Threading.Tasks;
using PottMaster.Models;

namespace PottMaster.Services
{
    public enum AuthResult
    {
        Success,
        InvalidCredentials,
        SignupFailed,
        ProfileCreationFailed,
        UserProfileNotFound,
        UnknownError
    }

    public class AuthResponse<T>
    {
        public AuthResult Result { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public interface IAuthService
    {
        Client Client { get; }

        Task<AuthResponse<bool>> SignUpAsync(string email, string password);

        Task<AuthResponse<bool>> SignInAsync(string email, string password);

        Task<AuthResponse<bool>> SignOutAsync();

        bool IsLoggedIn { get; }

        Task<AuthResponse<UserProfiles>> GetUserProfileAsync();
    }
}