using POSSystem.Infrastructure.Data;
using POSSystem.Maui.Pages;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace POSSystem.Maui;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App(POSDbContext dbContext)
    {
        try
        {
            InitializeComponent();

            // Subscribe to unhandled exception events
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            // Check if installation is needed
            Debug.WriteLine("Checking installation status...");
            if (NeedsInstallation())
            {
                Debug.WriteLine("Installation required. Showing install page...");
                MainPage = new NavigationPage(new InstallPage());
            }
            else
            {
                // Initialize database
                Debug.WriteLine("Initializing database...");
                try
                {
                    dbContext.Database.EnsureCreated();
                    Debug.WriteLine("Database initialized successfully.");
                }
                catch (Exception dbEx)
                {
                    Debug.WriteLine($"Database initialization error: {dbEx.Message}");
                    // If database init fails, show installation page
                    MainPage = new NavigationPage(new InstallPage());
                    return;
                }

                Debug.WriteLine("Creating AppShell...");
                MainPage = new AppShell();
                Debug.WriteLine("AppShell created successfully.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FATAL ERROR in App constructor: {ex}");
            
            // Show error to user
            MainPage = new ContentPage
            {
                Content = new VerticalStackLayout
                {
                    Padding = 30,
                    Spacing = 10,
                    Children =
                    {
                        new Label { Text = "Application Failed to Start", FontSize = 24, FontAttributes = FontAttributes.Bold },
                        new Label { Text = $"Error: {ex.Message}", TextColor = Colors.Red },
                        new Label { Text = ex.ToString(), FontSize = 12 },
                        new Button 
                        { 
                            Text = "Reinstall", 
                            Command = new Command(() => MainPage = new NavigationPage(new InstallPage()))
                        }
                    }
                }
            };
        }
    }

    private bool NeedsInstallation()
    {
        try
        {
            // Check if appsettings.json exists in app data
            var appSettingsPath = Path.Combine(FileSystem.AppDataDirectory, "appsettings.json");
            if (!File.Exists(appSettingsPath))
            {
                Debug.WriteLine("appsettings.json not found in AppDataDirectory");
                return true;
            }

            // Check if DataSettingsManager has valid configuration
            if (!DataSettingsManager.IsDatabaseConfigured())
            {
                Debug.WriteLine("Database not configured");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking installation: {ex.Message}");
            return true; // If we can't determine, assume installation is needed
        }
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            Debug.WriteLine($"UNHANDLED EXCEPTION: {ex}");
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Debug.WriteLine($"UNOBSERVED TASK EXCEPTION: {e.Exception}");
        e.SetObserved();
    }
}
