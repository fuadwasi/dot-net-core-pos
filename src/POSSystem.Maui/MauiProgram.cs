using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using POSSystem.Application.Services;
using POSSystem.Domain.Interfaces;
using POSSystem.Infrastructure.Data;
using POSSystem.Infrastructure.Repositories;
using POSSystem.Infrastructure.Configuration;
using POSSystem.Maui.Pages;
using POSSystem.Maui.ViewModels;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Maui;
using System.Reflection;

namespace POSSystem.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

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
            
            // If connection string is not set in appsettings, use default SQLite path
            if (string.IsNullOrEmpty(dataConfig.ConnectionString))
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "pos.db");
                dataConfig.ConnectionString = $"Data Source={dbPath}";
            }

            // Load data settings
            DataSettingsManager.LoadSettings(dataConfig);

            // Register DbContext with connection string from configuration
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
                else
                {
                    // Default to SQLite
                    options.UseSqlite(connectionString);
                }

                // Set command timeout if specified
                if (dataConfig.SQLCommandTimeout.HasValue && dataConfig.SQLCommandTimeout.Value >= 0)
                {
                    options.CommandTimeout(dataConfig.SQLCommandTimeout.Value);
                }
            });

            // Register repositories and Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register application services
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<SaleService>();

            // Register ViewModels
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<ProductsViewModel>();
            builder.Services.AddTransient<CustomersViewModel>();
            builder.Services.AddTransient<SalesViewModel>();
            builder.Services.AddTransient<NewSaleViewModel>();

            // Register Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ProductsPage>();
            builder.Services.AddTransient<CustomersPage>();
            builder.Services.AddTransient<SalesPage>();
            builder.Services.AddTransient<NewSalePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        catch (Exception ex)
        {
            // Log the exception or display it
            System.Diagnostics.Debug.WriteLine($"Error initializing application: {ex.Message}");
            throw;
        }
    }
}
