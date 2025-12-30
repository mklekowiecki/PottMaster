using Supabase;
using System.Threading.Tasks;

namespace PottMaster.Services
{
    public interface IAuthService
    {
        Client Client { get; }

        Task SignUpAsync(string email, string password);

        Task SignInAsync(string email, string password);

        Task SignOutAsync();

        bool IsLoggedIn { get; }
    }
}