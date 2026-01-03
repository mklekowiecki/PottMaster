using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Controls;
using System;

namespace PottMaster
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { "android.intent.action.VIEW" }, Categories = new[] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" }, DataScheme = "pottmaster", DataHost = "auth-callback")]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Log when MainActivity is created
            System.Diagnostics.Debug.WriteLine($"MainActivity OnCreate called");

            // Check if launched from an intent
            HandleIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);

            System.Diagnostics.Debug.WriteLine($"MainActivity OnNewIntent called");

            if (intent != null)
            {
                Intent = intent;
                HandleIntent(intent);
            }
        }

        private void HandleIntent(Intent? intent)
        {
            if (intent?.Data != null)
            {
                var data = intent.Data;
                System.Diagnostics.Debug.WriteLine($"Intent Data received: {data}");
                System.Diagnostics.Debug.WriteLine($"Scheme: {data.Scheme}, Host: {data.Host}");

                // Convert Android URI to .NET Uri and send to App
                var uri = new Uri(data.ToString()!);

                // Trigger the app link handling in the App class
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Microsoft.Maui.Controls.Application.Current is App app)
                    {
                        System.Diagnostics.Debug.WriteLine($"Calling SendOnAppLinkRequestReceived with URI: {uri}");
                        app.SendOnAppLinkRequestReceived(uri);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: Application.Current is not of type App");
                    }
                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No intent data received");
            }
        }
    }
}
