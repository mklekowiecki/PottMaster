using PottMaster.Resources;
using System.Net;
using Microsoft.Extensions.Logging;

namespace PottMaster.Services;

public class ErrorHandlingService : IErrorHandlingService
{
    private readonly ILogger<ErrorHandlingService> _logger;
    
    public ErrorHandlingService(ILogger<ErrorHandlingService> logger)
    {
        _logger = logger;
    }
    
    public void LogError(Exception exception, string context)
    {
        _logger.LogError(exception, "Error in {Context}: {Message}", context, exception.Message);
    }
    
    public void LogWarning(string message, string context)
    {
        _logger.LogWarning("{Context}: {Message}", context, message);
    }
    
    public void LogInfo(string message, string context)
    {
        _logger.LogInformation("{Context}: {Message}", context, message);
    }
    
    public string GetUserFriendlyError(Exception exception)
    {
        return exception switch
        {
            HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.Unauthorized => 
                "Unauthorized access. Please log in again.",
            HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.NotFound => 
                "Resource not found.",
            HttpRequestException => 
                "Network error. Please check your connection.",
            TimeoutException => 
                "Request timed out. Please try again.",
            UnauthorizedAccessException => 
                "Unauthorized access. Please log in again.",
            InvalidOperationException => 
                "Invalid operation.",
            _ => AppResources.Error ?? "An error occurred"
        };
    }
    
    public async Task<bool> HandleErrorAsync(Exception exception, string context, bool showToUser = true)
    {
        LogError(exception, context);
        
        if (showToUser && Application.Current?.MainPage != null)
        {
            var message = GetUserFriendlyError(exception);
            await Application.Current.MainPage.DisplayAlert(
                AppResources.Error ?? "Error", 
                message, 
                AppResources.Ok ?? "OK");
        }
        
        return true;
    }
}
