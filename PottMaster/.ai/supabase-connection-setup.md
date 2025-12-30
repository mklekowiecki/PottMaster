# Supabase Connection Setup for PottMaster

## Overview
This document explains how to configure the Supabase API connection in the PottMaster KMP app. The setup uses platform-specific configuration files to securely store the Supabase project URL and anon key, avoiding hardcoding sensitive information.

## Prerequisites
- A Supabase project created at [supabase.com](https://supabase.com).
- Obtain your project's URL (e.g., `https://your-project-id.supabase.co`) and anon key from the dashboard under Settings > API.

## Configuration Steps

### Android
1. Open or create `local.properties` in the **project root** (e.g., `c:/AIDev/PotMaster/local.properties`). This file is git-ignored and not committed.
2. Add the following lines (replace with your actual Supabase details):
   ```
   supabaseUrl=https://your-project-id.supabase.co
   supabaseAnonKey=your_anon_key_here
   ```
3. Sync the Gradle project (in Android Studio or VS Code) to generate `BuildConfig` with these values.
4. The `project.findProperty("supabaseUrl")` in `build.gradle.kts` reads from this file.
5. For release builds, update the hardcoded placeholders in `build.gradle.kts` or use a secure secrets management solution (e.g., Gradle properties or CI/CD variables).

### iOS
1. Open `iosApp/iosApp/Info.plist`.
2. Add the following keys inside the `<dict>` tag:
   ```xml
   <key>SUPABASE_URL</key>
   <string>https://your-project-id.supabase.co</string>
   <key>SUPABASE_ANON_KEY</key>
   <string>your_anon_key_here</string>
   ```
3. The values are read at runtime using `NSBundle.mainBundle.infoDictionary` in the iOS-specific `SupabaseConfig.ios.kt`.

### Shared Code
- The configuration is accessed via `SupabaseConfig.url` and `SupabaseConfig.anonKey` in commonMain.
- The `SupabaseClient` is initialized in `di/AppModule.kt` using these values.
- Fallback placeholders are used if config values are not set.

## Verification
- Build and run the Android app; check logs for successful client initialization.
- For iOS, build in Xcode and verify in the console.
- Test authentication flows in `AuthViewModel` to ensure connection works.

## Security Notes
- Never commit API keys to version control.
- Use environment variables in CI/CD for production builds.
- Consider using Supabase's service role key only on the server-side (e.g., Edge Functions).

## Troubleshooting
- **Invalid URL/Key**: Ensure the URL ends with `.supabase.co` and the key starts with `eyJ...`.
- **Build Errors**: Sync Gradle after updating `local.properties` or `build.gradle.kts`.
- **Runtime Errors**: Check that Info.plist keys match exactly (case-sensitive).

For local development with Supabase CLI, update the hardcoded values in `AppModule.kt` temporarily to use `http://127.0.0.1:54321` and the local anon key.

**Last Updated**: 2025-12-30