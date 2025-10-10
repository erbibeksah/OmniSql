<pre>

░█████╗░███╗░░░███╗███╗░░██╗██╗░██████╗███████╗██████╗░██╗░░░██╗██╗░█████╗░███████╗░██████╗
██╔══██╗████╗░████║████╗░██║██║██╔════╝██╔════╝██╔══██╗██║░░░██║██║██╔══██╗██╔════╝██╔════╝
██║░░██║██╔████╔██║██╔██╗██║██║╚█████╗░█████╗░░██████╔╝╚██╗░██╔╝██║██║░░╚═╝█████╗░░╚█████╗░
██║░░██║██║╚██╔╝██║██║╚████║██║░╚═══██╗██╔══╝░░██╔══██╗░╚████╔╝░██║██║░░██╗██╔══╝░░░╚═══██╗
╚█████╔╝██║░╚═╝░██║██║░╚███║██║██████╔╝███████╗██║░░██║░░╚██╔╝░░██║╚█████╔╝███████╗██████╔╝
░╚════╝░╚═╝░░░░░╚═╝╚═╝░░╚══╝╚═╝╚═════╝░╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚═╝░╚════╝░╚══════╝╚═════╝░        
</pre>

# OmniServices.Packages.DataBase
![NuGet Version](https://img.shields.io/nuget/v/Omniservices.Packages.DataBase.svg)
![NuGet Downloads](https://img.shields.io/nuget/dt/Omniservices.Packages.DataBase.svg)
![License](https://img.shields.io/github/license/erbibeksah/OmniSql)
[![C#](https://img.shields.io/badge/C%23-12.0%2F.NET%209-blueviolet)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![GitHub](https://img.shields.io/badge/github-erbibeksah/OmniSql-blue.svg)](https://github.com/erbibeksah/OmniSql)

<p>
	<img src="https://img.shields.io/github/stars/erbibeksah/OmniSql?style=social" alt="GitHub stars">
	<img src="https://img.shields.io/github/forks/erbibeksah/OmniSql?style=social" alt="GitHub forks">
	<img src="https://img.shields.io/github/issues/erbibeksah/OmniSql?color=yellow" alt="GitHub issues">
</p>

A comprehensive database utility library for .NET developers. This library streamlines SQL query execution, abstracts database operations, and provides robust migration support for seamless schema evolution across environments. It supports SQL-based databases including MSSQL and PostgreSQL. It provides the sets of static methods for common database operations (query, update, insert, delete, schema, and conversion) using ADO.NET. It also includes helpers for configuration, logging, and migrations.

---
## Give a Star! ⭐

If you like or are using this project please give it a star. Thanks!


## Setup Instructions

### 1. Namespace Usage
Add a `GlobalUsings.cs` file in your project base directory:

### ⚡ Setup
```bash 
global using DataBase;
```
Or, in your `.cs` files:
```bash
using DataBase;
```

### 2. Load Configuration
In `Program.cs`:
```bash
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// add the connection and appsetting key string in appsettings.json file

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AppSettings": {
    "DataProvider": "Microsoft.Data.SqlClient"
  },
  "ConnectionStrings": {
    "ConnectionString": "write-your-own-connection-string"
  }
  "AllowedHosts": "*"
}

```
In `Appsettings.json`:
```bash
"AppSettings": {
    "DataProvider": "Microsoft.Data.SqlClient"
  }
```
```bash
 "ConnectionStrings": {
    "ConnectionString": "write-your-own-connection-string"
  }
```


### 3. Register Database Providers
In `Program.cs`:
```bash
// For SQL Server 
DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
// For PostgreSQL 
DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
```

### 4. Initialize Configuration
In `Program.cs`:
```bash
AppSettingFile.Initialize(builder.Configuration);
```


### 5. Logger and Migration Setup
In `Program.cs`:
```bash
var env = app.Services.GetRequiredService<IHostEnvironment>(); 
var logger = app.Services.GetRequiredService<ILoggerService>(); 
logger.Initialize(env);
var migrationRunner = app.Services.GetRequiredService<DataBase.MigrationRunnerHelper>();
migrationRunner.RunMigrations();
```

### 6. Entire Program.cs Setup
In `Program.cs`:
```bash
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Add all services to the container.
builder.Services.AddCustomServices();


// database register factory
DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
DbProviderFactories.RegisterFactory("Npgsql", Microsoft.Data.SqlClient.SqlClientFactory.Instance);

// Load configuration globally
AppSettingFile.Initialize(builder.Configuration);

var app = builder.Build();

// Initialize logger
var env = app.Services.GetRequiredService<IHostEnvironment>();
var logger = app.Services.GetRequiredService<ILoggerService>();
logger.Initialize(env);

// Run migrations at startup
var migrationRunner = app.Services.GetRequiredService<DataBase.MigrationRunnerHelper>();
migrationRunner.RunMigrations();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// middleware mapping for global exception handler
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Serve Swagger JSON endpoint
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        options.RoutePrefix = string.Empty; // Makes Swagger UI available at root
    });
}

// run in async mode
await app.RunAsync();
```


---

## DB.cs Methods and Their Uses

### Query Methods
- `GetDataTable(DbCommand cmd)`: Executes a command and returns results as a `DataTable`.
- `GetDataSet(DbCommand cmd)`: Executes a command and returns results as a `DataSet`.
- `GetDataRow(DbCommand cmd)`: Executes a command and returns the first row.
- `GetDataTableByProc(string proc, params IDataParameter[])`: Executes a stored procedure and returns a `DataTable`.
- `GetDataSetByProc(string proc, params IDataParameter[])`: Executes a stored procedure and returns a `DataSet`.
- `GetDataTableByQuery(string query, params IDataParameter[])`: Executes a SQL query and returns a `DataTable`.
- `GetDataRow(string proc, params IDataParameter[])`: Executes a stored procedure and returns the first row.

### Command Methods
- `ExecProc(string proc)`: Executes a stored procedure (no result).
- `ExecNonQueryByProc(string proc, params IDataParameter[])`: Executes a stored procedure and returns the command.
- `ExecNonQueryByQuery(string query, params IDataParameter[])`: Executes a SQL query and returns the command.
- `ExecAllNonQuery(List<string> queries)`: Executes multiple queries in sequence.

### Schema Methods
- `GetColumns(string table, string filter)`: Gets column schema for a table with a filter.
- `GetColumns(string table)`: Gets column schema for a table (excludes columns with 'ID').

### DataSet Fillers
- `FillDatasetByQuery(string query, DataSet ds, string table)`: Fills a dataset table from a query.
- `FillReportDataSetByProc(string proc, DataSet ds, string table, params IDataParameter[])`: Fills a dataset table from a stored procedure (for reporting).
- `FillReportDataSetByProc(string proc, DataSet ds, string table, out DbCommand, params IDataParameter[])`: Same as above, also returns the command.

### Scalar Methods
- `ExeScalarByProc(string proc, params IDataParameter[])`: Executes a stored procedure and returns an integer result.
- `ExecScalarByQuery(string query, params IDataParameter[])`: Executes a query and returns an integer result.

### Transaction
- `GetNewTransactionScope(TransactionScopeOption)`: Creates a new transaction scope.

### Table Operations
- `InsertByProc(string proc, params IDataParameter[])`: Inserts using a stored procedure.
- `InsertByProc(string proc, out DataTable, params IDataParameter[])`: Inserts and returns a `DataTable`.
- `UpdateByProc(string proc, out DataTable, params IDataParameter[])`: Updates and returns a `DataTable`.
- `UpdateByProc(string proc, params IDataParameter[])`: Updates using a stored procedure.
- `UpdateByQuery(string query, params IDataParameter[])`: Updates using a query.
- `DeleteByProc(string proc, params IDataParameter[])`: Deletes using a stored procedure.

### Connection Helper
- `GetConnectionString()`: Gets the connection string from configuration.

---

## Usage Example as (CRUD)

### 1. Select Query
```bash
public UserMst GetUserById(Guid p_sUserId)
{
    DataRow? dr;
    TransactionScopeOption tso;
    DbConnectionScopeOption cso;
    if (Transaction.Current == null)
    {
        tso = TransactionScopeOption.RequiresNew;
    }
    else
    {
        tso = TransactionScopeOption.Required;
    }
    if (DbConnectionScope.Current == null)
    {
        cso = DbConnectionScopeOption.RequiresNew;
    }
    else
    {
        cso = DbConnectionScopeOption.Required;
    }
    using (TransactionScope ts = DB.GetNewTransactionScope(tso))
    {
        try
        {
            using (DbConnectionScope cs = new DbConnectionScope(cso))
            {
                dr = DB.GetDataRow("USR_GetByUserId",
                                   CSqlParameter.CreateParameter("@p_sUserId", SqlDbType.UniqueIdentifier, p_sUserId));
            }
            ts.Complete();
        }
        catch (Exception ex)
        {
            ExHandler.Handle(ex);
            throw;
        }
    }
    return dr != null ? setUserMst(dr) : null!;
}
```
### 2. Similarly for Update, Insert
```bash
// namespace
using DataBase;

// Insert data 
DB.InsertByProc("sp_InsertUser", new SqlParameter("@Name", "John"));

// Update data 
DB.UpdateByQuery("UPDATE Users SET Name = @Name WHERE Id = @Id", new SqlParameter("@Name", "Jane"), new SqlParameter("@Id", 1));

/// <summary>
/// Inserts or updates the login status for a user session.
/// </summary>
/// <param name="objUserSessionMst">The <see cref="UserSessionMst"/> object containing session details.</param>
public void InsertAndUpdateLoginStatus(UserSessionMst objUserSessionMst)
{
    if (objUserSessionMst is null) return;
    TransactionScopeOption tso;
    DbConnectionScopeOption cso;
    if (Transaction.Current == null)
    {
        tso = TransactionScopeOption.RequiresNew;
    }
    else
    {
        tso = TransactionScopeOption.Required;
    }
    if (DbConnectionScope.Current == null)
    {
        cso = DbConnectionScopeOption.RequiresNew;
    }
    else
    {
        cso = DbConnectionScopeOption.Required;
    }
    using (TransactionScope ts = DB.GetNewTransactionScope(tso))
    {
        try
        {
            using (DbConnectionScope cs = new DbConnectionScope(cso))
            {
                DB.UpdateByProc("USR_AddUpdateUserSession",
                          CSqlParameter.CreateParameter("@p_sUserId", SqlDbType.UniqueIdentifier, objUserSessionMst.USERID),
                          CSqlParameter.CreateParameter("@p_sSessionId", SqlDbType.VarChar, objUserSessionMst.SESSIONID),
                          CSqlParameter.CreateParameter("@p_sLocation", SqlDbType.VarChar, objUserSessionMst.LOCATION),
                          CSqlParameter.CreateParameter("@p_sUserAgent", SqlDbType.VarChar, objUserSessionMst.USERAGENT),
                          CSqlParameter.CreateParameter("@p_sDevice", SqlDbType.VarChar, objUserSessionMst.DEVICE),
                          CSqlParameter.CreateParameter("@p_sIpaddress", SqlDbType.VarChar, objUserSessionMst.IPADDRESS),
                          CSqlParameter.CreateParameter("@p_sModifiedBy", SqlDbType.VarChar, objUserSessionMst.MODIFIEDBY));
            }
            ts.Complete();
        }
        catch (Exception ex)
        {
            ExHandler.Handle(ex);
            throw;
        }
    }
}

```
### 3. For Delete
```bash
public void DeleteUserSession(Guid p_sUserId, string p_sSessionId)
{
    TransactionScopeOption tso;
    DbConnectionScopeOption cso;
    if (Transaction.Current == null)
    {
        tso = TransactionScopeOption.RequiresNew;
    }
    else
    {
        tso = TransactionScopeOption.Required;
    }
    if (DbConnectionScope.Current == null)
    {
        cso = DbConnectionScopeOption.RequiresNew;
    }
    else
    {
        cso = DbConnectionScopeOption.Required;
    }
    using (TransactionScope ts = DB.GetNewTransactionScope(tso))
    {
        try
        {
            using (DbConnectionScope cs = new DbConnectionScope(cso))
            {
                DB.DeleteByProc("USR_DeleteUserSession",
                                            CSqlParameter.CreateParameter("@p_sUserId", SqlDbType.UniqueIdentifier, p_sUserId),
                                            CSqlParameter.CreateParameter("@p_sSessionId", SqlDbType.VarChar, p_sSessionId));
            }
            ts.Complete();
        }
        catch (Exception ex)
        {
            ExHandler.Handle(ex);
            throw;
        }
    }
}
```

## Usage Example as (Migration)
### ⚡ Setup

### 1. NameSpace Usage
```bash
using FluentMigrator;
```
### 2. Migration Creation
1. Create a Migration Folder Any Where in your Project.
2. Inside that create a class file `.cs` and paste this code inside it.
```bash
/// <summary>
/// Create Migration for User Table
/// </summary>
/// <remarks>
/// ⚠️ This migration no must be in this format {yy-mm-dd-hour-minute} is intended to use {two-digit}**.
/// </remarks>
[Migration(202609112116)]
public class M001_Create_User_Table : Migration
{
    public override void Up()
    {
        this.Create.Table("t_ROLEMST")
                .WithColumn("ID").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("ROLENAME").AsString(50).NotNullable()
                .WithColumn("ISADMIN").AsBoolean().NotNullable()
                .WithColumn("ISSUPERADMIN").AsBoolean().NotNullable()
                .WithColumn("ISACTIVEROLE").AsBoolean().NotNullable()
                .WithColumn("CREATEDAT").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
               .WithColumn("UPDATEDAT").AsDateTime().Nullable()
                .WithColumn("MODIFIEDBY").AsString(30).Nullable();
            
        // for exec procedure and also make sure the sql file should be embedded in project
        this.Execute.EmbeddedScript("USR_GetByUserName.sql"); 
    }

    /// <summary>
    /// Rollback migration for the 't_userMst' table.
    /// </summary>
    /// <remarks>
    /// ⚠️ This rollback method is intended for **emergency use only**.
    /// Use with caution in production environments.
    ///
    /// Calling this will drop the 't_ROLEMST' table and delete all role records.
    ///
    /// Use programmatically via:
    /// <code>runner.Rollback(1);</code>
    /// </remarks>
    public override void Down()
    {
        // not needed as of now
        // Delete.Table("t_ROLEMST");
    }

}

```
## Usage Example as (Serilogger Logger)
1. One should get the all logs inside the base directory folder Logs/Application/{Application Environment}/omni-xxx-xxx.log
```bash
public class AuthController : BaseApiController
{
    private readonly ILoggerService _logger;
    public AuthController(IHttpContextAccessor httpContextAccessor, ILoggerService logger) : base(httpContextAccessor)
    {
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    [ActionName("GetUserById")]
    public IActionResult GetUserById()
    {
        _logger.Info("AuthController.GetUserById:: API called Successfully");
        return ApiResult<object>.SuccessResult("Get the user", "Get User Successfully").ToActionResult();
    }
}

```

## Features

- **Database Abstraction:** Easily switch between supported databases (MSSQL, PostgreSQL).
- **Migration Support:** Use [FluentMigrator](https://fluentmigrator.github.io/) for versioned schema migrations.
- **Logging:** Integrated with [Serilog](https://serilog.net/) for file and console logging.
- **Configuration:** Uses Microsoft.Extensions.Configuration for flexible settings.
- **Hosting Integration:** Compatible with Microsoft.Extensions.Hosting for environment-aware operations.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Supported database server (MSSQL or PostgreSQL)

### Installation

Add the package references in your `.csproj`:


### Usage

#### 1. Configure Logging

Use `LoggerService` to initialize logging:


#### 2. Run Migrations

Create and run migrations using `MigrationRunnerHelper`:



#### 3. Add Migrations

Create migration classes using FluentMigrator:



## Configuration

Set your database provider and connection string in your application settings:



## Logging

Logs are written to the specified file path. When using hosting, logs are placed in:

## Created By

**erbibeksah** - Passionate about making developer tools that don't suck.

- 🐙 GitHub: [@erbibeksah](https://github.com/erbibeksah)
- 🚀 Project: [OmniSql](https://github.com/erbibeksah/OmniSql)

---

## 🤝 Contributing

Found a bug? Have a cool feature idea? Contributions make the open-source world go round!

1. Fork the repo
2. Create your feature branch (`git checkout -b amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the **MIT License** ([LICENSE-MIT](LICENSE-MIT)) - simple, permissive, and developer-friendly!

---

<div align="center">

<b>Made with ❤️ and lots of ☕ by <a href="https://github.com/erbibeksah">erbibeksah</a></b>

<i>If OmniSql saved you time, consider giving it a ⭐ on <a href="https://github.com/erbibeksah/OmniSql">GitHub</a>!</i>

</div>
