namespace DataBase;

/// <summary>
/// Helper for accessing application configuration values and connection strings.
/// </summary>
/// <remarks>
/// This static helper holds a reference to an <see cref="IConfiguration"/> instance which must be
/// provided by calling <see cref="Initialize(IConfiguration)"/> before invoking any of the
/// lookup methods. The class is intentionally simple and returns empty strings when keys or
/// connections are not found or in case of exceptions to simplify callers that expect string results.
/// </remarks>
public static class AppSettingFile
{
    /// <summary>
    /// The backing <see cref="IConfiguration"/> instance used to read settings.
    /// </summary>
    /// <remarks>
    /// This field is nullable until <see cref="Initialize(IConfiguration)"/> is called.
    /// Accessors should handle the null case gracefully.
    /// </remarks>
    private static IConfiguration? _configuration;

    /// <summary>
    /// Initializes the helper with the provided configuration instance.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to use for subsequent lookups.</param>
    /// <remarks>
    /// This method must be called once during application startup (for example from DI composition root)
    /// before other APIs on this class are used.
    /// </remarks>
    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // _configuration.GetRequiredSection("AppSettings").GetRequiredSection("DataProvider").Value;
    // _configuration.GetConnectionString("DefaultConnection");

    /// <summary>
    /// Gets a connection string by name from the configured <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="p_keyName">The name of the connection string to retrieve.</param>
    /// <returns>
    /// The connection string associated with <paramref name="p_keyName"/> if present; otherwise an empty string.
    /// </returns>
    /// <remarks>
    /// If <paramref name="p_keyName"/> is null or empty, or if the underlying configuration is not set,
    /// this method returns an empty string. No exceptions are thrown for missing values.
    /// </remarks>
    public static string GetConnection(string p_keyName) =>
        !string.IsNullOrEmpty(p_keyName) && _configuration?.GetConnectionString(p_keyName) != null
            ? _configuration.GetConnectionString(p_keyName) ?? string.Empty
            : string.Empty;

    /// <summary>
    /// Retrieves a configuration value from either a specified section or from the "AppSettings" section.
    /// </summary>
    /// <param name="p_keyName">The key name to look up (required).</param>
    /// <param name="p_keyFrom">
    /// Optional. If provided, the method will attempt to retrieve the key from this configuration section.
    /// If not provided, the method will attempt to read the key from the "AppSettings" section.
    /// </param>
    /// <returns>
    /// The configuration value if found; otherwise an empty string. The method also returns an empty string
    /// if an exception occurs during lookup.
    /// </returns>
    /// <remarks>
    /// This method uses <c>GetRequiredSection</c> in the original code to retrieve subsections. The implementation
    /// guards against null values and exceptions and normalizes all failure cases to an empty string to simplify callers.
    /// </remarks>
    public static string Get(string p_keyName, string p_keyFrom = "")
    {
        try
        {
            if (string.IsNullOrEmpty(p_keyName)) return string.Empty;
            if (!string.IsNullOrEmpty(p_keyName) && !string.IsNullOrEmpty(p_keyFrom))
            {
                if (_configuration?.GetRequiredSection(p_keyFrom).GetRequiredSection(p_keyName) != null && _configuration?.GetRequiredSection(p_keyFrom).GetRequiredSection(p_keyName).Value != null)
                {
                    return _configuration.GetRequiredSection(p_keyFrom).GetRequiredSection(p_keyName).Value ?? string.Empty;
                }
            }
            else if (_configuration?.GetRequiredSection("AppSettings").GetRequiredSection(p_keyName) != null && _configuration?.GetRequiredSection("AppSettings").GetRequiredSection(p_keyName).Value != null)
            {
                return _configuration.GetRequiredSection("AppSettings").GetRequiredSection(p_keyName).Value ?? string.Empty;
            }
        }
        catch (Exception)
        {
            return string.Empty;
        }
        return string.Empty;
    }
}

