using System.Threading.Tasks;

namespace PottMaster.Services;

/// <summary>
/// Default <see cref="IBiometricService" /> implementation for platforms where
/// biometric authentication is not yet implemented (for example, non-Android targets).
/// </summary>
/// <remarks>
/// <para>
/// This class acts as a stub: it always reports that biometrics are unavailable and
/// always fails authentication by returning <c>false</c>. This allows the rest of the
/// application to compile and run on platforms without biometric support.
/// </para>
/// <para>
/// Platform-specific implementations (e.g., Android, iOS, Windows) should provide
/// concrete logic that:
/// <list type="bullet">
/// <item>
/// <description>
/// Accurately reports whether biometric hardware/credentials are available via
/// <see cref="IsBiometricAvailable" />.
/// </description>
/// </item>
/// <item>
/// <description>
/// Performs a real biometric authentication prompt and returns <c>true</c> only when
/// the user successfully authenticates via <see cref="AuthenticateBiometricAsync" />.
/// </description>
/// </item>
/// </list>
/// </para>
/// </remarks>
public class BiometricService : IBiometricService
{
    /// <summary>
    /// Determines whether biometric authentication is available on the current device.
    /// </summary>
    /// <returns>
    /// In this stub implementation, always returns <c>false</c> to indicate that
    /// biometrics are not available on this platform.
    /// </returns>
    /// <remarks>
    /// A platform-specific implementation should return <c>true</c> only when the
    /// device has supported biometric hardware and the user has enrolled at least one
    /// biometric credential (e.g., fingerprint, face).
    /// </remarks>
    public bool IsBiometricAvailable()
    {
        return false;
    }

    /// <summary>
    /// Attempts to authenticate the user using a biometric prompt.
    /// </summary>
    /// <param name="title">
    /// The title to display in the biometric prompt, where supported.
    /// </param>
    /// <param name="description">
    /// The description or reason for the biometric request, where supported.
    /// </param>
    /// <returns>
    /// A task that, in this stub implementation, always completes with <c>false</c>,
    /// indicating that biometric authentication is not supported or not performed on
    /// this platform.
    /// </returns>
    /// <remarks>
    /// A platform-specific implementation should:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Display a native biometric prompt using the provided <paramref name="title" />
    /// and <paramref name="description" /> where applicable.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Return <c>true</c> if the user successfully authenticates via biometrics, or
    /// <c>false</c> if authentication fails, is canceled, or cannot be performed.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public Task<bool> AuthenticateBiometricAsync(string title, string description)
    {
        return Task.FromResult(false);
    }
}