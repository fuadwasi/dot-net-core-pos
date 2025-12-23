using Microsoft.EntityFrameworkCore;
using POSSystem.Infrastructure.Configuration;
using POSSystem.Infrastructure.Data;
using System.Text.Json;

namespace POSSystem.Maui.Pages;

public partial class InstallPage : ContentPage
{
    public InstallPage()
    {
        InitializeComponent();
        
        // Wire up event handlers
        DatabaseProviderPicker.SelectedIndexChanged += OnDatabaseProviderChanged;
        AuthTypePicker.SelectedIndexChanged += OnAuthTypeChanged;
    }

    private void OnDatabaseProviderChanged(object? sender, EventArgs e)
    {
        var isSQLite = DatabaseProviderPicker.SelectedIndex == 0;
        SQLiteSection.IsVisible = isSQLite;
        SqlServerSection.IsVisible = !isSQLite;
    }

    private void OnAuthTypeChanged(object? sender, EventArgs e)
    {
        var isWindowsAuth = AuthTypePicker.SelectedIndex == 0;
        SqlAuthSection.IsVisible = !isWindowsAuth;
    }

    private async void OnTestConnectionClicked(object sender, EventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        TestResultLabel.IsVisible = false;
        TestConnectionButton.IsEnabled = false;

        try
        {
            var connectionString = BuildConnectionString();
            var provider = DatabaseProviderPicker.SelectedIndex == 0 ? "SQLite" : "SqlServer";

            // Test the connection
            var optionsBuilder = new DbContextOptionsBuilder<POSDbContext>();
            
            if (provider == "SQLite")
            {
                optionsBuilder.UseSqlite(connectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(connectionString);
            }

            using var context = new POSDbContext(optionsBuilder.Options);
            await context.Database.CanConnectAsync();

            TestResultLabel.Text = "✓ Connection successful!";
            TestResultLabel.TextColor = Colors.Green;
            TestResultLabel.IsVisible = true;
        }
        catch (Exception ex)
        {
            TestResultLabel.Text = $"✗ Connection failed: {ex.Message}";
            TestResultLabel.TextColor = Colors.Red;
            TestResultLabel.IsVisible = true;
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            TestConnectionButton.IsEnabled = true;
        }
    }

    private async void OnInstallClicked(object sender, EventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        InstallButton.IsEnabled = false;

        try
        {
            var connectionString = BuildConnectionString();
            var provider = DatabaseProviderPicker.SelectedIndex == 0 ? "SQLite" : "SqlServer";
            
            int? timeout = null;
            if (!string.IsNullOrWhiteSpace(TimeoutEntry.Text) && int.TryParse(TimeoutEntry.Text, out var parsedTimeout))
            {
                timeout = parsedTimeout;
            }

            // Save configuration
            var config = new DataConfig
            {
                ConnectionString = connectionString,
                DataProvider = provider,
                SQLCommandTimeout = timeout
            };

            await SaveConfigurationAsync(config);

            // Initialize database
            var optionsBuilder = new DbContextOptionsBuilder<POSDbContext>();
            
            if (provider == "SQLite")
            {
                optionsBuilder.UseSqlite(connectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(connectionString);
            }

            using var context = new POSDbContext(optionsBuilder.Options);
            
            // Create database and apply migrations
            await context.Database.EnsureCreatedAsync();

            await DisplayAlert("Success", "Installation completed successfully! The application will now restart.", "OK");

            // Restart the application by navigating to main shell
            Application.Current!.MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Installation failed: {ex.Message}", "OK");
            InstallButton.IsEnabled = true;
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private string BuildConnectionString()
    {
        if (DatabaseProviderPicker.SelectedIndex == 0) // SQLite
        {
            if (string.IsNullOrWhiteSpace(SQLitePathEntry.Text))
            {
                // Use default path
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "pos.db");
                return $"Data Source={dbPath}";
            }
            else
            {
                return $"Data Source={SQLitePathEntry.Text}";
            }
        }
        else // SQL Server
        {
            var server = ServerEntry.Text?.Trim() ?? "localhost";
            var database = DatabaseEntry.Text?.Trim() ?? "POSSystem";
            
            if (AuthTypePicker.SelectedIndex == 0) // Windows Auth
            {
                return $"Server={server};Database={database};Integrated Security=true;TrustServerCertificate=true;";
            }
            else // SQL Auth
            {
                var username = UsernameEntry.Text?.Trim() ?? "sa";
                var password = PasswordEntry.Text ?? "";
                return $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate=true;";
            }
        }
    }

    private async Task SaveConfigurationAsync(DataConfig config)
    {
        var appSettingsPath = Path.Combine(FileSystem.AppDataDirectory, "appsettings.json");
        
        var settings = new
        {
            Data = new
            {
                config.ConnectionString,
                config.DataProvider,
                config.SQLCommandTimeout
            },
            Logging = new
            {
                LogLevel = new
                {
                    Default = "Information",
                    Microsoft = "Warning",
                    Microsoft_Hosting_Lifetime = "Information"
                }
            }
        };

        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(appSettingsPath, json);
        
        // Also load into DataSettingsManager
        DataSettingsManager.LoadSettings(config);
    }
}

