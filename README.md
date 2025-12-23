# POS System - .NET 9 with MAUI

A modern, cross-platform Point of Sale (POS) system built with .NET 9 and .NET MAUI. This application demonstrates clean architecture principles, the repository pattern, and dependency injection for building maintainable enterprise applications.

## 🚀 Features

- **Cross-Platform**: Runs on Windows, macOS, Android, and iOS using .NET MAUI
- **Clean Architecture**: Organized in layers (Domain, Infrastructure, Application, UI)
- **Repository Pattern**: Abstraction for data access with Unit of Work pattern
- **Dependency Injection**: Built-in DI for loose coupling and testability
- **Entity Framework Core**: Modern ORM with SQLite for cross-platform data storage
- **MVVM Pattern**: Model-View-ViewModel architecture for maintainable UI code
- **Responsive UI**: Adaptive layout that works on different screen sizes
- **Installation Wizard**: nopCommerce-style setup wizard for first-run configuration

### Core Functionality

- **Installation Wizard**: First-run setup with database configuration (SQLite or SQL Server)
- **Product Management**: Add, edit, delete, and search products
- **Customer Management**: Manage customer information and purchase history
- **Sales Processing**: Complete sales transactions with multiple payment methods
- **Sales History**: View and filter sales by date range
- **Dashboard**: Real-time sales metrics (daily, weekly, monthly)
- **Multi-Database Support**: Choose between SQLite (default) or SQL Server

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

### Required

- **.NET 9 SDK** or later
  - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
  - Verify installation: `dotnet --version`

### For MAUI Development

- **Visual Studio 2022 (17.8 or later)** - Recommended for Windows/Mac
  - Workload: ".NET Multi-platform App UI development"
  
  OR

- **Visual Studio Code** with C# Dev Kit extension
- **MAUI Workload**: Install using `dotnet workload install maui`

### Platform-Specific Requirements

#### Windows
- Windows 10 version 1809 or higher
- Windows SDK (installed with Visual Studio)

#### macOS
- macOS 13 (Ventura) or higher
- Xcode 14.3 or higher (for iOS/macCatalyst development)

#### Android
- Android SDK API 21 or higher (installed with Visual Studio or Android Studio)

#### iOS
- Xcode 14.3 or higher (macOS only)
- iOS 11.0 or higher

## 🏗️ Project Structure

```
POSSystem/
├── src/
│   ├── POSSystem.Domain/              # Domain entities and interfaces
│   │   ├── Entities/                  # Business entities (Product, Customer, Sale, etc.)
│   │   └── Interfaces/                # Repository interfaces
│   │
│   ├── POSSystem.Infrastructure/      # Data access implementation
│   │   ├── Data/                      # DbContext and configurations
│   │   └── Repositories/              # Repository implementations
│   │
│   ├── POSSystem.Application/         # Business logic layer
│   │   └── Services/                  # Application services
│   │
│   └── POSSystem.Maui/                # MAUI UI layer
│       ├── Pages/                     # XAML pages
│       ├── ViewModels/                # View models (MVVM)
│       ├── Resources/                 # Images, fonts, styles
│       └── Platforms/                 # Platform-specific code
│
├── POSSystem.sln                      # Solution file
└── README.md
```

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/fuadwasi/dot-net-core-pos.git
cd dot-net-core-pos
```

### 2. Install MAUI Workload (if not already installed)

```bash
dotnet workload install maui
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

## ▶️ Running the Application

> **💡 First Time Setup**: The application includes an installation wizard (like nopCommerce) that will guide you through database configuration on first run. See [INSTALL.md](INSTALL.md) for detailed instructions.

### Quick Start

1. Install MAUI workload: `dotnet workload install maui`
2. Open `POSSystem.sln` in Visual Studio 2022
3. Select "Windows Machine" as target
4. Press F5 to run
5. Complete the installation wizard (appears automatically)
6. Start using the application!

For detailed installation and troubleshooting, see **[INSTALL.md](INSTALL.md)**.

### Option 1: Using Visual Studio

1. Open `POSSystem.sln` in Visual Studio 2022
2. Select the target platform from the debug dropdown:
   - Windows Machine (for Windows)
   - Android Emulator (for Android)
   - iOS Simulator (for iOS - macOS only)
3. Press F5 or click "Start Debugging"
4. On first run, complete the installation wizard

### Option 2: Using .NET CLI

#### Windows

```bash
cd src/POSSystem.Maui
dotnet build -f net9.0-windows10.0.19041.0 -c Release
dotnet run -f net9.0-windows10.0.19041.0
```

#### macOS (MacCatalyst)

```bash
cd src/POSSystem.Maui
dotnet build -f net9.0-maccatalyst -c Release
dotnet run -f net9.0-maccatalyst
```

#### Android

```bash
cd src/POSSystem.Maui
dotnet build -f net9.0-android -c Release
# Deploy to connected device or emulator
dotnet run -f net9.0-android
```

#### iOS (macOS only)

```bash
cd src/POSSystem.Maui
dotnet build -f net9.0-ios -c Release
dotnet run -f net9.0-ios
```

## 🗄️ Database

The application uses SQLite for data storage by default, which is automatically created on first run. Database configuration is managed through `appsettings.json` following nopCommerce patterns.

### Database Configuration

Configuration is stored in `src/POSSystem.Maui/appsettings.json`:

```json
{
  "Data": {
    "ConnectionString": "",
    "DataProvider": "SQLite",
    "SQLCommandTimeout": null
  }
}
```

When the connection string is empty (default), the database is automatically created at:

- **Windows**: `%LOCALAPPDATA%\POSSystem\pos.db`
- **macOS**: `~/Library/Application Support/POSSystem/pos.db`
- **Android**: `/data/data/com.possystem.maui/files/pos.db`
- **iOS**: App sandbox container

### Switching to SQL Server

To use SQL Server instead of SQLite, update `appsettings.json`:

```json
{
  "Data": {
    "ConnectionString": "Server=localhost;Database=POSSystem;User Id=sa;Password=YourPassword;TrustServerCertificate=true;",
    "DataProvider": "SqlServer",
    "SQLCommandTimeout": 30
  }
}
```

For detailed configuration options, see [CONFIGURATION.md](CONFIGURATION.md).

### Sample Data

The application includes sample products that are automatically seeded on the first run. You can add more products, customers, and process sales through the UI.

## 🏛️ Architecture Overview

### Clean Architecture Layers

1. **Domain Layer** (`POSSystem.Domain`)
   - Contains core business entities (Product, Customer, Sale, SaleItem)
   - Defines repository interfaces
   - No dependencies on other layers

2. **Infrastructure Layer** (`POSSystem.Infrastructure`)
   - Implements repository interfaces
   - Contains Entity Framework Core DbContext
   - Database configurations and migrations

3. **Application Layer** (`POSSystem.Application`)
   - Business logic and services
   - Orchestrates domain objects and repositories
   - DTOs for data transfer (if needed)

4. **Presentation Layer** (`POSSystem.Maui`)
   - .NET MAUI UI implementation
   - MVVM pattern with ViewModels
   - Platform-specific code

### Design Patterns Used

- **Repository Pattern**: Abstracts data access
- **Unit of Work Pattern**: Manages transactions across repositories
- **Dependency Injection**: Loose coupling and testability
- **MVVM Pattern**: Separation of UI and business logic
- **Observer Pattern**: Through CommunityToolkit.Mvvm

## 🔧 Configuration

### Database Configuration

The application uses a centralized configuration approach with `appsettings.json`, following nopCommerce patterns. Configuration is managed through the `DataSettingsManager` class:

```csharp
// Configuration is loaded from appsettings.json at startup
var dataConfig = new DataConfig();
builder.Configuration.GetSection("Data").Bind(dataConfig);

// Load into DataSettingsManager
DataSettingsManager.LoadSettings(dataConfig);

// DbContext configuration supports multiple providers
builder.Services.AddDbContext<POSDbContext>(options =>
{
    if (dataConfig.DataProvider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(connectionString);
    }
    else if (dataConfig.DataProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
    }
});
```

For complete configuration documentation, see [CONFIGURATION.md](CONFIGURATION.md).

### Dependency Injection

Services are registered in `MauiProgram.cs`:

```csharp
// Register repositories and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register application services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<SaleService>();
```

## 📦 NuGet Packages

### Domain Layer
- No external dependencies (pure domain logic)

### Infrastructure Layer
- `Microsoft.EntityFrameworkCore.Sqlite` (9.0.0)
- `Microsoft.EntityFrameworkCore.SqlServer` (9.0.0)
- `Microsoft.EntityFrameworkCore.Design` (9.0.0)

### Application Layer
- No additional dependencies

### MAUI Layer
- `Microsoft.Maui.Controls` (9.0.101)
- `CommunityToolkit.Maui` (11.2.0)
- `CommunityToolkit.Mvvm` (8.4.0)
- `Microsoft.Extensions.Configuration.Json` (9.0.0)
- `Microsoft.Extensions.Configuration.Binder` (9.0.0)

## 🚀 Deployment

### Windows

```bash
dotnet publish -f net9.0-windows10.0.19041.0 -c Release -p:PublishSingleFile=true -p:PublishReadyToRun=true
```

### macOS

```bash
dotnet publish -f net9.0-maccatalyst -c Release -p:CreatePackage=true
```

### Android

```bash
dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=apk
```

### iOS

```bash
dotnet publish -f net9.0-ios -c Release
```

## 🔨 Development

### Adding a New Entity

1. Create entity in `POSSystem.Domain/Entities`
2. Add repository interface in `POSSystem.Domain/Interfaces`
3. Implement repository in `POSSystem.Infrastructure/Repositories`
4. Add DbSet to `POSDbContext`
5. Create service in `POSSystem.Application/Services`
6. Register service in `MauiProgram.cs`

### Adding a New Page

1. Create ViewModel in `POSSystem.Maui/ViewModels`
2. Create XAML page in `POSSystem.Maui/Pages`
3. Register in `MauiProgram.cs`
4. Add route in `AppShell.xaml`

## 📈 Performance Considerations

### Implemented Optimizations

- **Async/Await**: All database operations are asynchronous
- **Lazy Loading**: ViewModels load data on demand
- **Connection Pooling**: EF Core manages database connections efficiently
- **Compiled XAML**: XAML is compiled for better performance
- **ObservableObject**: Efficient property change notifications

### Recommended Improvements

1. **Caching**: Implement in-memory caching for frequently accessed data
2. **Pagination**: Add pagination for large datasets
3. **Background Tasks**: Process long-running operations in background
4. **Image Optimization**: Compress and resize product images
5. **Indexing**: Add database indexes for frequently queried fields

## 🧪 Testing

The project is structured to support unit testing and integration testing:

```bash
# Create test projects
dotnet new xunit -n POSSystem.Domain.Tests -o tests/POSSystem.Domain.Tests
dotnet new xunit -n POSSystem.Application.Tests -o tests/POSSystem.Application.Tests
dotnet new xunit -n POSSystem.Infrastructure.Tests -o tests/POSSystem.Infrastructure.Tests

# Run tests
dotnet test
```

## 🔐 Security Considerations

- **SQLite Encryption**: Consider using SQLCipher for database encryption
- **Input Validation**: Validate all user inputs
- **SQL Injection**: EF Core parameterizes queries automatically
- **Authentication**: Add user authentication for production use
- **Authorization**: Implement role-based access control

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Original inspiration from [.NET Point of Sale (POS) C#](https://github.com/livealvi/.NET-Point-of-Sale-POS--Csharp/)
- Built with [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- Uses [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- Database powered by [Entity Framework Core](https://docs.microsoft.com/ef/core/)

## 📞 Support

For questions, issues, or suggestions:

- Open an issue on GitHub
- Contact: [Your Contact Information]

## 🗺️ Roadmap

- [ ] Add barcode scanning functionality
- [ ] Implement receipt printing
- [ ] Add inventory management alerts
- [ ] Create comprehensive reports
- [ ] Add multi-language support
- [ ] Implement offline sync capability
- [ ] Add cloud backup integration
- [ ] Create admin dashboard
- [ ] Add employee management
- [ ] Implement discount and promotion system

---

**Built with ❤️ using .NET 9 and MAUI**
