namespace PottMaster;

public static class Constants
{
#if DEBUG

    //public const string SupabaseBaseUrl = "http://127.0.0.1:54321";
    // const string SupabaseAnonKey = "sb_publishable_ACJWlzQHlZjBrEguHvfOxg_3BJgxAaH";
    // TODO: Replace with actual values. For local development, run `supabase start` in the supabase directory to get the anon key.
    public const string SupabaseBaseUrl = "https://tvmitdhydieegokheqxx.supabase.co";
    public const string SupabaseAnonKey = "sb_publishable_n6UmZtzwWCJLJ87NLFvuuQ_kn_iWu8L";
#elif RELEASE
    public const string SupabaseBaseUrl = "https://your-staging-supabase-url.supabase.co";
    public const string SupabaseAnonKey = "your-staging-anon-key";
#endif


}