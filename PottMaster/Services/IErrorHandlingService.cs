namespace PottMaster.Services;

public interface IErrorHandlingService
{
    void LogError(Exception exception, string context);
    void LogWarning(string message, string context);
    void LogInfo(string message, string context);
    string GetUserFriendlyError(Exception exception);
    Task<bool> HandleErrorAsync(Exception exception, string context, bool showToUser = true);
}
