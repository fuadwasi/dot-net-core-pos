using POSSystem.Infrastructure.Data;

namespace POSSystem.Maui;

public partial class App : Application
{
    public App(POSDbContext dbContext)
    {
        InitializeComponent();

        // Initialize database
        dbContext.Database.EnsureCreated();

        MainPage = new AppShell();
    }
}
