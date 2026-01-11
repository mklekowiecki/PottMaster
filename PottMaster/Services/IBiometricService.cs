using System.Threading.Tasks;

namespace PottMaster.Services;

public interface IBiometricService
{
    bool IsBiometricAvailable();
    Task<bool> AuthenticateBiometricAsync(string title, string description);
}