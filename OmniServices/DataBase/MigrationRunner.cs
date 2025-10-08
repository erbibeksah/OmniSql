namespace DataBase;

/// <summary>
/// Helper that configures and runs database migrations using FluentMigrator.
/// </summary>
/// <remarks>
/// This class reads the configured data provider and connection string, builds a
/// <see cref="Microsoft.Extensions.DependencyInjection.ServiceProvider"/> configured for
/// FluentMigrator, and executes any pending migrations. It uses an injected
/// <see cref="ILoggerService"/> to surface informational, warning and error messages.
/// </remarks>
public class MigrationRunnerHelper
{
    /// <summary>
    /// Logger used to record migration status and errors.
    /// </summary>
    private readonly ILoggerService _logger;

    /// <summary>
    /// The configured data provider name (case-insensitive). Expected values include "npgsql" for PostgreSQL;
    /// any other value will default to SQL Server.
    /// </summary>
    private readonly string _provider;

    /// <summary>
    /// Connection string used by the migration runner.
    /// </summary>
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationRunnerHelper"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger implementation used to record migration progress and errors. Must not be <c>null</c>.
    /// </param>
    /// <remarks>
    /// The constructor reads the "DataProvider" application setting and obtains the
    /// runtime connection string from <see cref="DB.GetConnectionString()"/>.
    /// </remarks>
    public MigrationRunnerHelper(ILoggerService logger)
    {
        _logger = logger;
        _provider = AppSettingFile.Get("DataProvider") ?? "";
        _connectionString = DB.GetConnectionString();
    }

    /// <summary>
    /// Builds the FluentMigrator service provider and executes pending migrations.
    /// </summary>
    /// <remarks>
    /// The method does the following:
    /// - Configures a <see cref="ServiceCollection"/> with FluentMigrator 
    /// </remarks>
    public void RunMigrations()
    {
        var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb =>
            {
                if (_provider.Equals("npgsql", StringComparison.OrdinalIgnoreCase))
                    rb.AddPostgres();
                else
                    rb.AddSqlServer();

                rb.WithGlobalConnectionString(_connectionString)
                  .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations()
                  .ScanIn(Assembly.GetExecutingAssembly()).For.EmbeddedResources();
            })
            .AddLogging(lb =>
            {
                lb.ClearProviders();
                lb.AddSerilog(dispose: false);
                lb.SetMinimumLevel(LogLevel.Debug);
            })
            .BuildServiceProvider(false);
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        try
        {
            if (runner.HasMigrationsToApplyUp())
            {
                _logger.Info("=== Migration Status::Starting migrations! ===");
                runner.MigrateUp();
                _logger.Info("=== Migration Status::applied successfully! ===");
            }
            else
            {
                _logger.Warn("=== Migration Status::No migrations to apply! ===");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("=== Migration Status::Migration failed! ===", ex);
        }
    }
}
