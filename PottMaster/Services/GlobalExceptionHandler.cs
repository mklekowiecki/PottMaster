using Microsoft.Extensions.Logging;
using System.Text;

namespace PottMaster.Services;

public class GlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IErrorHandlingService _errorHandlingService;
    private static readonly string LogDirectory = Path.Combine(FileSystem.AppDataDirectory, "Logs");
    private static readonly string LogFileName = $"crash_log_{DateTime.Now:yyyyMMdd}.txt";

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IErrorHandlingService errorHandlingService)
    {
        _logger = logger;
        _errorHandlingService = errorHandlingService;
        EnsureLogDirectoryExists();
    }

    public void Initialize()
    {
        // Hook into AppDomain unhandled exceptions (background threads)
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;

        // Hook into TaskScheduler unobserved task exceptions
        TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;

#if ANDROID
        // Android-specific exception handler
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#endif
    }

    private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            LogUnhandledException(exception, "AppDomain.UnhandledException", e.IsTerminating);
        }
    }

    private void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException", false);
        e.SetObserved(); // Prevent process termination
    }

#if ANDROID
    private void OnAndroidUnhandledException(object sender, Android.Runtime.RaiseThrowableEventArgs e)
    {
        LogUnhandledException(e.Exception, "Android.UnhandledException", true);
    }
#endif

    private void LogUnhandledException(Exception exception, string source, bool isTerminating)
    {
        try
        {
            var logMessage = BuildLogMessage(exception, source, isTerminating);

            // Log to console/debug output
            _logger.LogCritical(exception, "UNHANDLED EXCEPTION from {Source}. IsTerminating: {IsTerminating}", source, isTerminating);

            // Log to file for persistence
            WriteToLogFile(logMessage);

            // Try to show alert to user (may not work if app is terminating)
            if (!isTerminating)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await _errorHandlingService.HandleErrorAsync(exception, source, showToUser: true);
                    }
                    catch
                    {
                        // Ignore errors in error handler to prevent recursive failures
                    }
                });
            }
        }
        catch (Exception loggingException)
        {
            // Last resort - write to system diagnostics
            System.Diagnostics.Debug.WriteLine($"Failed to log unhandled exception: {loggingException.Message}");
            System.Diagnostics.Debug.WriteLine($"Original exception: {exception}");
        }
    }

    private string BuildLogMessage(Exception exception, string source, bool isTerminating)
    {
        var sb = new StringBuilder();
        sb.AppendLine("???????????????????????????????????????????????????????????");
        sb.AppendLine($"UNHANDLED EXCEPTION - {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        sb.AppendLine("???????????????????????????????????????????????????????????");
        sb.AppendLine($"Source: {source}");
        sb.AppendLine($"Is Terminating: {isTerminating}");
        sb.AppendLine($"Device: {DeviceInfo.Current.Model} ({DeviceInfo.Current.Platform} {DeviceInfo.Current.VersionString})");
        sb.AppendLine($"App Version: {AppInfo.Current.VersionString}");
        sb.AppendLine();
        sb.AppendLine("EXCEPTION DETAILS:");
        sb.AppendLine("???????????????????????????????????????????????????????????");
        
        var currentException = exception;
        var depth = 0;
        
        while (currentException != null)
        {
            if (depth > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"INNER EXCEPTION #{depth}:");
                sb.AppendLine("???????????????????????????????????????????????????????????");
            }
            
            sb.AppendLine($"Type: {currentException.GetType().FullName}");
            sb.AppendLine($"Message: {currentException.Message}");
            sb.AppendLine($"Stack Trace:");
            sb.AppendLine(currentException.StackTrace ?? "No stack trace available");
            
            if (currentException.Data.Count > 0)
            {
                sb.AppendLine("Data:");
                foreach (var key in currentException.Data.Keys)
                {
                    sb.AppendLine($"  {key}: {currentException.Data[key]}");
                }
            }
            
            currentException = currentException.InnerException;
            depth++;
        }
        
        sb.AppendLine("???????????????????????????????????????????????????????????");
        sb.AppendLine();
        
        return sb.ToString();
    }

    private void EnsureLogDirectoryExists()
    {
        try
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create log directory");
        }
    }

    private void WriteToLogFile(string message)
    {
        try
        {
            var logFilePath = Path.Combine(LogDirectory, LogFileName);
            File.AppendAllText(logFilePath, message);
            
            // Clean up old log files (keep last 7 days)
            CleanupOldLogFiles();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write to log file");
        }
    }

    private void CleanupOldLogFiles()
    {
        try
        {
            var cutoffDate = DateTime.Now.AddDays(-7);
            var logFiles = Directory.GetFiles(LogDirectory, "crash_log_*.txt");
            
            foreach (var file in logFiles)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < cutoffDate)
                {
                    File.Delete(file);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old log files");
        }
    }

    public static string GetLogDirectoryPath() => LogDirectory;

    public static IEnumerable<string> GetLogFiles()
    {
        try
        {
            if (Directory.Exists(LogDirectory))
            {
                return Directory.GetFiles(LogDirectory, "crash_log_*.txt")
                    .OrderByDescending(f => new FileInfo(f).CreationTime);
            }
        }
        catch
        {
            // Ignore errors
        }
        
        return Array.Empty<string>();
    }

    public static string? ReadLogFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
        }
        catch
        {
            // Ignore errors
        }
        
        return null;
    }
}
