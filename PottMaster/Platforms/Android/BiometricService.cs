using Android.Hardware.Fingerprints;
using Android.Content;
using Android.App;
using Microsoft.Maui.ApplicationModel;
using System.Threading.Tasks;
using Android.OS;
using PottMaster.Services;
using Android.Runtime;
using Java.Lang;

namespace PottMaster.Platforms.Android;

public class BiometricService : IBiometricService
{
    public bool IsBiometricAvailable()
    {
        var activity = Platform.CurrentActivity;
        if (activity == null) return false;
        var fingerprintManager = (FingerprintManager) activity.GetSystemService(Context.FingerprintService);
        return fingerprintManager != null && fingerprintManager.IsHardwareDetected && fingerprintManager.HasEnrolledFingerprints;
    }

    public Task<bool> AuthenticateBiometricAsync(string title, string description)
    {
        var activity = Platform.CurrentActivity;
        if (activity == null) return Task.FromResult(false);

        var fingerprintManager = (FingerprintManager) activity.GetSystemService(Context.FingerprintService);
        if (fingerprintManager == null || !fingerprintManager.IsHardwareDetected || !fingerprintManager.HasEnrolledFingerprints)
            return Task.FromResult(false);

        var tcs = new TaskCompletionSource<bool>();
        var cancellationSignal = new CancellationSignal();
        var authenticationCallback = new FingerprintAuthenticationCallback(tcs);
        fingerprintManager.Authenticate(null, cancellationSignal, 0, authenticationCallback, null);
        return tcs.Task;
    }

    private class FingerprintAuthenticationCallback : FingerprintManager.AuthenticationCallback
    {
        private readonly TaskCompletionSource<bool> _tcs;

        public FingerprintAuthenticationCallback(TaskCompletionSource<bool> tcs)
        {
            _tcs = tcs;
        }

        public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult result)
        {
            _tcs.SetResult(true);
        }
        public override void OnAuthenticationError([GeneratedEnum] FingerprintState errorCode, ICharSequence? errString)
        {
            _tcs.SetResult(false);
        }

        public override void OnAuthenticationFailed()
        {
            _tcs.SetResult(false);
        }
    }
}