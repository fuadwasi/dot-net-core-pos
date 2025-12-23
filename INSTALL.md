# Installation and Running Guide

This guide explains how to install, configure, and run the POS System on Windows.

## Prerequisites

Before you begin, ensure you have:

1. **.NET 9 SDK** installed
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verify: `dotnet --version` (should show 9.0.x)

2. **.NET MAUI Workload** installed
   ```bash
   dotnet workload install maui
   ```

3. **Visual Studio 2022** (17.8 or later) - Recommended
   - With ".NET Multi-platform App UI development" workload
   
   OR
   
   **Visual Studio Code** with C# Dev Kit extension

## Building the Application

### Option 1: Using Visual Studio 2022

1. Open `POSSystem.sln` in Visual Studio 2022
2. Select "Windows Machine" as the target in the debug dropdown
3. Press F5 or click "Start Debugging"
4. The application will build and launch

### Option 2: Using Command Line

```bash
# Navigate to the solution directory
cd /path/to/dot-net-core-pos

# Restore NuGet packages
dotnet restore

# Build the MAUI project for Windows
cd src/POSSystem.Maui
dotnet build -f net9.0-windows10.0.19041.0 -c Release

# Run the application
dotnet run -f net9.0-windows10.0.19041.0
```

## First Run - Installation Wizard

When you run the application for the first time, you'll see the **Installation Wizard** (similar to nopCommerce):

### Installation Steps

1. **Welcome Screen**
   - The installation wizard will automatically appear

2. **Choose Database Provider**
   
   **Option A: SQLite (Recommended)**
   - Select "SQLite (Recommended)" from the dropdown
   - Leave the path empty to use the default location
   - OR enter a custom path (e.g., `C:\POSData\pos.db`)
   - Click "Test Connection" to verify
   - Click "Install and Continue"

   **Option B: SQL Server**
   - Select "SQL Server" from the dropdown
   - Enter server address (e.g., `localhost` or `192.168.1.100`)
   - Enter database name (default: `POSSystem`)
   - Choose authentication type:
     - **Windows Authentication**: Uses your Windows login
     - **SQL Server Authentication**: Enter username and password
   - Optionally set command timeout (seconds)
   - Click "Test Connection" to verify
   - Click "Install and Continue"

3. **Database Creation**
   - The installer will:
     - Create the database
     - Initialize tables and schema
     - Save configuration to `%LOCALAPPDATA%\POSSystem\appsettings.json`
     - Restart the application

4. **Application Starts**
   - After installation, the main dashboard will appear
   - You're ready to start using the POS system!

## Default Database Locations

### SQLite (Default)
- **Windows**: `C:\Users\{YourUsername}\AppData\Local\POSSystem\pos.db`
- **macOS**: `~/Library/Application Support/POSSystem/pos.db`
- **Android**: `/data/data/com.possystem.maui/files/pos.db`
- **iOS**: App sandbox container

### SQL Server
- Configured database on your SQL Server instance

## Configuration File

After installation, configuration is stored in:
```
%LOCALAPPDATA%\POSSystem\appsettings.json
```

Example configuration:
```json
{
  "Data": {
    "ConnectionString": "Data Source=C:\\Users\\YourName\\AppData\\Local\\POSSystem\\pos.db",
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

## Changing Configuration

To reconfigure the database:

1. Delete the configuration file:
   ```
   %LOCALAPPDATA%\POSSystem\appsettings.json
   ```

2. Restart the application

3. The installation wizard will appear again

## Troubleshooting

### Issue: Application won't start

**Solution 1: Check .NET 9 SDK**
```bash
dotnet --version
```
Should show 9.0.x or higher

**Solution 2: Check MAUI Workload**
```bash
dotnet workload list
```
Should include `maui-windows`

**Solution 3: Reinstall MAUI Workload**
```bash
dotnet workload restore
```

### Issue: "MAUI workload not installed" error

**Solution**:
```bash
dotnet workload install maui
```

Then rebuild the project.

### Issue: Database connection fails during installation

**For SQLite**:
- Ensure the directory path exists and is writable
- Try using the default path (leave empty)
- Check Windows permissions for the AppData folder

**For SQL Server**:
- Verify SQL Server is running
- Check firewall settings
- Test connection with SQL Server Management Studio first
- Ensure the user has permission to create databases

### Issue: Application crashes on startup

**Solution 1: Delete configuration and reinstall**
```bash
del %LOCALAPPDATA%\POSSystem\appsettings.json
```

**Solution 2: Delete database and reinstall**
```bash
del %LOCALAPPDATA%\POSSystem\pos.db
del %LOCALAPPDATA%\POSSystem\appsettings.json
```

**Solution 3: Check Debug Output**
- Run from Visual Studio with debugger attached
- Check Output window for detailed error messages

### Issue: Build errors in Visual Studio

**Solution 1: Clean and Rebuild**
1. Right-click solution → Clean Solution
2. Close Visual Studio
3. Delete `bin` and `obj` folders in all projects
4. Reopen Visual Studio
5. Rebuild solution

**Solution 2: Restore NuGet Packages**
1. Right-click solution → Restore NuGet Packages
2. Rebuild solution

## Running on Different Platforms

### Windows
```bash
dotnet run -f net9.0-windows10.0.19041.0
```

### macOS (MacCatalyst)
```bash
dotnet run -f net9.0-maccatalyst
```

### Android (requires Android SDK)
```bash
dotnet run -f net9.0-android
```

### iOS (requires macOS with Xcode)
```bash
dotnet run -f net9.0-ios
```

## Development Mode

For development, you can run in debug mode:

```bash
dotnet run -f net9.0-windows10.0.19041.0 -c Debug
```

This enables:
- Hot Reload for XAML changes
- Detailed debug logging
- Better error messages

## Publishing for Distribution

### Windows (Self-Contained)
```bash
dotnet publish -f net9.0-windows10.0.19041.0 -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Output: `bin/Release/net9.0-windows10.0.19041.0/win-x64/publish/POSSystem.Maui.exe`

### Windows (Framework-Dependent)
```bash
dotnet publish -f net9.0-windows10.0.19041.0 -c Release
```

Requires .NET 9 runtime on target machine.

## Getting Help

If you encounter issues:

1. Check the [README.md](README.md) for general information
2. Review [CONFIGURATION.md](CONFIGURATION.md) for configuration details
3. Check [TROUBLESHOOTING.md](#troubleshooting) section above
4. Open an issue on GitHub with:
   - Error message
   - Steps to reproduce
   - Your OS and .NET version
   - Configuration (without sensitive data)

## Next Steps

Once the application is running:

1. Explore the **Dashboard** - View sales metrics
2. Navigate to **Products** - Add products for sale
3. Navigate to **Customers** - Add customer information
4. Use **New Sale** - Process sales transactions
5. Check **Sales History** - View past sales

## Sample Data

The application automatically creates sample products on first run:
- Laptop - $999.99
- Mouse - $29.99

You can add more products through the Products page.

---

**Enjoy using the POS System!** 🚀
