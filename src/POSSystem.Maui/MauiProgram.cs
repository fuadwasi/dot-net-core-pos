using Microsoft.Extensions.Logging;
using POSSystem.Application.Services;
using POSSystem.Domain.Interfaces;
using POSSystem.Infrastructure.Data;
using POSSystem.Infrastructure.Repositories;
using POSSystem.Maui.Pages;
using POSSystem.Maui.ViewModels;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Maui;

namespace POSSystem.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
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

        // Register DbContext
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "pos.db");
        builder.Services.AddDbContext<POSDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

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
}
