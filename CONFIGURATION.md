# Configuration Guide

This document explains how to configure the POS System using the appsettings.json file.

## Overview

Following nopCommerce's pattern, the POS System uses a centralized configuration approach with `appsettings.json` stored in the MAUI project. The configuration is loaded at application startup and used throughout the application.

## Configuration File Location

The configuration file is located at:
```
src/POSSystem.Maui/appsettings.json
```

This file is embedded as a resource in the MAUI application and loaded at runtime.

## Configuration Structure

### Complete appsettings.json Example

```json
{
  "Data": {
    "ConnectionString": "",
    "DataProvider": "SQLite",
    "SQLCommandTimeout": null
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Data Configuration

The `Data` section contains database connection settings.

### Properties

#### ConnectionString
- **Type**: `string`
- **Default**: `""` (empty, will use default SQLite path)
- **Description**: The database connection string

**SQLite Example (default)**:
```json
"ConnectionString": "Data Source=/path/to/pos.db"
```

**SQL Server Example**:
```json
"ConnectionString": "Server=localhost;Database=POSSystem;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
```

**Default Behavior**: If the connection string is empty, the system automatically uses:
```
{AppDataDirectory}/pos.db
```
Where `{AppDataDirectory}` is the platform-specific application data directory.

#### DataProvider
- **Type**: `string`
- **Default**: `"SQLite"`
- **Options**: `"SQLite"`, `"SqlServer"`, `"PostgreSQL"`
- **Description**: The database provider to use

**Example**:
```json
"DataProvider": "SQLite"
```

#### SQLCommandTimeout
- **Type**: `int?` (nullable integer)
- **Default**: `null` (uses provider default)
- **Description**: Command execution timeout in seconds. Set to `0` for infinite timeout.

**Example**:
```json
"SQLCommandTimeout": 30
```

## Platform-Specific Database Paths

When using the default SQLite configuration (empty connection string), the database is stored in:

### Windows
```
C:\Users\{Username}\AppData\Local\POSSystem\pos.db
```

### macOS
```
/Users/{Username}/Library/Application Support/POSSystem/pos.db
```

### Android
```
/data/data/com.possystem.maui/files/pos.db
```

### iOS
```
{App Container}/Library/Application Support/pos.db
```

## Database Provider Configuration

### SQLite (Default)

SQLite is the default provider and works cross-platform without any additional setup.

**Configuration**:
```json
{
  "Data": {
    "ConnectionString": "",
    "DataProvider": "SQLite",
    "SQLCommandTimeout": null
  }
}
```

**Custom Path**:
```json
{
  "Data": {
    "ConnectionString": "Data Source=C:\\MyData\\pos.db",
    "DataProvider": "SQLite",
    "SQLCommandTimeout": null
  }
}
```

### SQL Server

To use SQL Server instead of SQLite:

**Configuration**:
```json
{
  "Data": {
    "ConnectionString": "Server=localhost;Database=POSSystem;User Id=sa;Password=YourPassword;TrustServerCertificate=true;",
    "DataProvider": "SqlServer",
    "SQLCommandTimeout": 30
  }
}
```

**Note**: SQL Server is primarily supported on Windows. The `Microsoft.EntityFrameworkCore.SqlServer` package is included in the Infrastructure project.

### PostgreSQL (Future Support)

PostgreSQL support can be added by:
1. Adding the `Npgsql.EntityFrameworkCore.PostgreSQL` package
2. Updating `MauiProgram.cs` to handle PostgreSQL provider
3. Setting the configuration:

```json
{
  "Data": {
    "ConnectionString": "Host=localhost;Database=possystem;Username=postgres;Password=YourPassword",
    "DataProvider": "PostgreSQL",
    "SQLCommandTimeout": 30
  }
}
```

## How Configuration is Loaded

The configuration is loaded in `MauiProgram.cs`:

```csharp
// Load configuration from embedded appsettings.json
var assembly = Assembly.GetExecutingAssembly();
using var stream = assembly.GetManifestResourceStream("POSSystem.Maui.appsettings.json");

var config = new ConfigurationBuilder()
    .AddJsonStream(stream!)
    .Build();

builder.Configuration.AddConfiguration(config);

// Load data configuration
var dataConfig = new DataConfig();
builder.Configuration.GetSection("Data").Bind(dataConfig);
```

## DataSettingsManager

The `DataSettingsManager` class manages the data configuration at runtime:

```csharp
// Load settings (called during app startup)
DataSettingsManager.LoadSettings(dataConfig);

// Check if database is configured
if (DataSettingsManager.IsDatabaseConfigured())
{
    // Database is ready
}

// Get current settings
var settings = DataSettingsManager.GetSettings();
var connectionString = settings.ConnectionString;

// Get command timeout
int timeout = DataSettingsManager.GetSqlCommandTimeout();
```

## Changing Configuration

### During Development

1. Edit `src/POSSystem.Maui/appsettings.json`
2. Rebuild the application
3. The new settings will be embedded in the app

### After Deployment

For deployed applications, you may want to:

1. **Add environment-specific configuration files**:
   - `appsettings.Development.json`
   - `appsettings.Production.json`

2. **Use environment variables** (future enhancement):
   ```csharp
   builder.Configuration.AddEnvironmentVariables("POS_");
   ```

3. **Store sensitive data securely**:
   - Use `SecureStorage` for passwords
   - Use encrypted configuration

## Migration from Hardcoded Values

Previously, the connection string was hardcoded in `MauiProgram.cs`:

```csharp
// OLD: Hardcoded
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "pos.db");
builder.Services.AddDbContext<POSDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
```

Now it uses configuration:

```csharp
// NEW: Configuration-based
builder.Services.AddDbContext<POSDbContext>(options =>
{
    var connectionString = dataConfig.ConnectionString;
    
    if (dataConfig.DataProvider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(connectionString);
    }
    else if (dataConfig.DataProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
    }
    
    if (dataConfig.SQLCommandTimeout.HasValue)
    {
        options.CommandTimeout(dataConfig.SQLCommandTimeout.Value);
    }
});
```

## Best Practices

1. **Never commit sensitive data**: Keep passwords and sensitive connection strings out of source control
2. **Use relative paths for SQLite**: Let the app determine the platform-specific data directory
3. **Set appropriate timeouts**: Adjust `SQLCommandTimeout` based on your needs
4. **Test on all platforms**: Different platforms may have different path requirements
5. **Backup your database**: Especially when using local SQLite files

## Troubleshooting

### Database Not Found

**Symptom**: Application crashes on startup with "database not found" error.

**Solution**: 
- Check the connection string path
- Ensure the directory exists and is writable
- On Windows, use double backslashes in paths: `C:\\Data\\pos.db`

### Connection Timeout

**Symptom**: Operations take too long and timeout.

**Solution**:
- Increase `SQLCommandTimeout` value
- Check network connectivity (for SQL Server)
- Optimize database queries

### Provider Not Found

**Symptom**: Error about missing database provider.

**Solution**:
- Ensure the correct NuGet package is installed
- Check the `DataProvider` value matches installed providers
- Rebuild the solution

## Examples

### Example 1: Local SQLite (Default)

```json
{
  "Data": {
    "ConnectionString": "",
    "DataProvider": "SQLite",
    "SQLCommandTimeout": null
  }
}
```

Result: Database at `{Platform-specific AppData}/pos.db`

### Example 2: Custom SQLite Path

```json
{
  "Data": {
    "ConnectionString": "Data Source=C:\\POSData\\database.db",
    "DataProvider": "SQLite",
    "SQLCommandTimeout": null
  }
}
```

Result: Database at `C:\POSData\database.db`

### Example 3: SQL Server with Timeout

```json
{
  "Data": {
    "ConnectionString": "Server=192.168.1.100;Database=POSSystem;User Id=posuser;Password=SecurePass123;TrustServerCertificate=true;",
    "DataProvider": "SqlServer",
    "SQLCommandTimeout": 60
  }
}
```

Result: Uses SQL Server with 60-second command timeout

## Future Enhancements

- Support for connection string encryption
- Support for Azure SQL Database
- Support for PostgreSQL
- Environment-specific configuration
- Configuration UI in the app
- Cloud-based configuration management

---

This configuration approach follows industry best practices and is inspired by nopCommerce's robust configuration system.
