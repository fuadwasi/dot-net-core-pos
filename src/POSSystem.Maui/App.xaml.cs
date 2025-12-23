using POSSystem.Infrastructure.Data;
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

            // Initialize database
            Debug.WriteLine("Initializing database...");
            dbContext.Database.EnsureCreated();
            Debug.WriteLine("Database initialized successfully.");

            Debug.WriteLine("Creating AppShell...");
            MainPage = new AppShell();
            Debug.WriteLine("AppShell created successfully.");
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
                        new Label { Text = ex.ToString(), FontSize = 12 }
                    }
                }
            };
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
