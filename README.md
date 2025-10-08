# DataBase Migration Utility

A comprehensive database utility library for .NET 9 developers. This library streamlines SQL query execution, abstracts database operations, and provides robust migration support for seamless schema evolution across environments. It supports SQL-based databases including MSSQL and PostgreSQL.

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



## Contributing

Contributions are welcome! Please submit issues or pull requests via GitHub.

## License

This project is licensed under the MIT License.
