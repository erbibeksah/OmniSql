namespace DataBase;

/// <summary>
/// Defines logging operations used by the migration tooling.
/// </summary>
/// <remarks>
/// Implementations are expected to configure an underlying logging framework and expose
/// simple methods for recording informational, warning and error messages.
/// </remarks>
public interface ILoggerService
{
    /// <summary>
    /// Initializes the logger to write to the specified file path.
    /// </summary>
    /// <param name="logFilePath">
    /// The full file path where log events will be written. This can be absolute or relative.
    /// The implementation may create any missing directories in the path.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <paramref name="logFilePath"/> is invalid for underlying I/O operations.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// Thrown when an I/O error occurs while creating directories or opening the log file.
    /// </exception>
    void Initialize(string logFilePath);

    /// <summary>
    /// Initializes the logger using host environment information and an optional prefix for the log file name.
    /// </summary>
    /// <param name="env">Hosting environment that provides <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath"/> and <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.EnvironmentName"/>.</param>
    /// <param name="logPrefix">Optional prefix to include in the generated log file name. Defaults to "omni".</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="env"/> is <c>null</c>.</exception>
    /// <remarks>
    /// This method composes a log file path in the format:
    /// {ContentRootPath}/Logs/Application/{EnvironmentName}/{logPrefix}-{yyyy-MM-dd-HHmm}.log
    /// and delegates to <see cref="Initialize(string)"/>.
    /// </remarks>
    void Initialize(IHostEnvironment env, string logPrefix = "omni");

    /// <summary>
    /// Writes an informational message to the configured log sink(s).
    /// </summary>
    /// <param name="message">The text message to record.</param>
    void Info(string message);

    /// <summary>
    /// Writes a warning message to the configured log sink(s).
    /// </summary>
    /// <param name="message">The text message to record.</param>
    void Warn(string message);

    /// <summary>
    /// Writes an error message to the configured log sink(s), optionally including exception details.
    /// </summary>
    /// <param name="message">The error message to record.</param>
    /// <param name="ex">Optional exception whose details will be included in the log entry.</param>
    void Error(string message, Exception? ex = null);
}

/// <summary>
/// Default implementation of <see cref="ILoggerService"/> that configures and uses Serilog.
/// </summary>
/// <remarks>
/// This service sets a Serilog static <c>Log.Logger</c> instance configured to write to a file.
/// Calling <see cref="Initialize(string)"/> will create any missing directories required by the path.
/// The implementation is intended for use by migration tooling and other application startup tasks.
/// </remarks>
public class LoggerService : ILoggerService
{
    /// <summary>
    /// Synchronization object reserved for any future thread-safe operations.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// The path to the currently configured log file, or <c>null</c> if not yet initialized.
    /// </summary>
    private string? _logFilePath;

    /// <summary>
    /// Initializes the logger to write to the specified file path.
    /// </summary>
    /// <param name="logFilePath">
    /// The full path of the log file to write. The method will attempt to create any missing directory components.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <paramref name="logFilePath"/> is invalid for underlying path operations.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// Thrown when an I/O error occurs while creating directories or opening the log file.
    /// </exception>
    public void Initialize(string logFilePath)
    {
        _logFilePath = logFilePath;
        var dir = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
            .MinimumLevel.Override("FluentMigrator", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .WriteTo.File(_logFilePath, rollingInterval: RollingInterval.Infinite,
                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}]  {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    /// <summary>
    /// Initializes the logger using information from <paramref name="env"/> and an optional file name prefix.
    /// </summary>
    /// <param name="env">The hosting environment providing content root and environment name.</param>
    /// <param name="logPrefix">Optional prefix to prepend to the generated log file name. Defaults to "omni".</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="env"/> is <c>null</c>.</exception>
    /// <remarks>
    /// The method generates a timestamped log file name using the pattern:
    /// {logPrefix}-{yyyy-MM-dd-HHmm}.log and places it into:
    /// {ContentRootPath}/Logs/Application/{EnvironmentName}/
    /// It then calls <see cref="Initialize(string)"/> to perform the actual Serilog configuration.
    /// </remarks>
    public void Initialize(IHostEnvironment env, string logPrefix = "omni")
    {
        if (env == null) throw new ArgumentNullException(nameof(env));

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmm");
        string logFileName = $"{logPrefix}-{timestamp}.log";

        string logDir = Path.Combine(env.ContentRootPath, "Logs", "Application", env.EnvironmentName);
        string fullLogPath = Path.Combine(logDir, logFileName);
        Initialize(fullLogPath);
    }

    /// <inheritdoc />
    public void Info(string message) => Log.Information(message);

    /// <inheritdoc />
    public void Warn(string message) => Log.Warning(message);

    /// <summary>
    /// Logs an error message. If an <see cref="Exception"/> is supplied it will be included in the log entry.
    /// </summary>
    /// <param name="message">The error message to record.</param>
    /// <param name="ex">Optional exception whose details will be logged alongside the message.</param>
    public void Error(string message, Exception? ex = null)
    {
        if (ex != null)
            Log.Error(ex, "{Message}", message);
        else
            Log.Error(message);
    }
}

