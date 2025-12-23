# Quick Start Guide

Get up and running with the POS System in 5 minutes!

## Prerequisites

- .NET 9 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/9.0))
- Visual Studio 2022 with MAUI workload OR VS Code with C# Dev Kit

## Installation

### Option 1: Visual Studio 2022 (Recommended)

1. **Install .NET MAUI workload**
   - Open Visual Studio Installer
   - Modify your VS 2022 installation
   - Check ".NET Multi-platform App UI development"
   - Click Modify

2. **Clone and Open**
   ```bash
   git clone https://github.com/fuadwasi/dot-net-core-pos.git
   cd dot-net-core-pos
   ```
   - Open `POSSystem.sln` in Visual Studio

3. **Run the App**
   - Select target platform: `Windows Machine`, `Android Emulator`, etc.
   - Press F5 or click the Run button
   - The app will build and launch

### Option 2: Command Line + VS Code

1. **Install MAUI workload**
   ```bash
   dotnet workload install maui
   ```

2. **Clone the repository**
   ```bash
   git clone https://github.com/fuadwasi/dot-net-core-pos.git
   cd dot-net-core-pos
   ```

3. **Build and Run**
   
   **For Windows:**
   ```bash
   cd src/POSSystem.Maui
   dotnet build -f net9.0-windows10.0.19041.0
   dotnet run -f net9.0-windows10.0.19041.0
   ```

   **For macOS:**
   ```bash
   cd src/POSSystem.Maui
   dotnet build -f net9.0-maccatalyst
   dotnet run -f net9.0-maccatalyst
   ```

   **For Android:**
   ```bash
   cd src/POSSystem.Maui
   dotnet build -f net9.0-android
   dotnet run -f net9.0-android
   ```

## First Launch

When you first run the app:

1. **Database Initialization**: The app automatically creates a SQLite database
2. **Sample Data**: Two sample products are seeded
3. **Dashboard**: You'll see the main dashboard with sales metrics (initially zero)

## Quick Tour

### 1. Dashboard (Home)
- View sales metrics: Today, This Week, This Month
- Quick action buttons to navigate to different sections

### 2. Products Page
- View all active products
- Search products by name, description, or barcode
- Swipe left on a product to delete it
- Pull down to refresh the list

### 3. Customers Page
- View all customers
- Search customers by name, email, or phone
- Swipe left to delete
- Pull down to refresh

### 4. New Sale Page
- **Left side**: Available products (tap to add to cart)
- **Right side**: Shopping cart
- Select a customer (optional)
- Choose payment method
- Add tax and discount amounts
- Click "Complete Sale" to process

### 5. Sales History
- View all completed sales
- Filter by date range
- See customer information and total amounts

## Common Tasks

### Adding a Product

Currently, products need to be added via code or database. To add a product through code:

1. Navigate to `POSDbContext.cs`
2. Add to the seed data in `OnModelCreating`
3. Delete the database file and restart the app

**Future Enhancement**: Add UI for product management.

### Processing a Sale

1. Navigate to "New Sale" page
2. Tap products to add to cart
3. Optionally select a customer
4. Enter tax and discount if applicable
5. Select payment method
6. Click "Complete Sale"

### Viewing Reports

1. Navigate to "Sales" page
2. Adjust date range using date pickers
3. Click "Filter" to view sales for that period

## Troubleshooting

### Build Errors

**Problem**: "MAUI workload not installed"
```bash
dotnet workload install maui
```

**Problem**: "Unable to find target framework"
- Ensure .NET 9 SDK is installed
- Run `dotnet --version` to verify

### Runtime Issues

**Problem**: Database error on startup
- Delete the database file and restart
- Location varies by platform (see README.md)

**Problem**: App crashes on Android
- Check Android SDK is properly configured
- Ensure API level 21 or higher

### Platform-Specific Issues

**Windows**:
- Requires Windows 10 version 1809 or higher
- Windows SDK must be installed

**macOS**:
- Requires macOS 13 (Ventura) or higher
- For iOS development, Xcode 14.3+ required

**Android**:
- Ensure Android SDK is installed
- Create an emulator or connect a physical device

## Development Tips

### Hot Reload

MAUI supports XAML Hot Reload:
1. Make changes to XAML files
2. Save the file
3. Changes appear immediately in running app

### Debugging

- Set breakpoints in ViewModels or Services
- Use Debug Output window for logs
- Check database contents with DB Browser for SQLite

### Database Location

Find the SQLite database:
- **Windows**: `%LOCALAPPDATA%\POSSystem\pos.db`
- **macOS**: `~/Library/Application Support/POSSystem/pos.db`
- **Android**: Use Android Device Monitor
- **iOS**: Use Xcode's Device and Simulator window

## Next Steps

1. **Read the Documentation**
   - [README.md](README.md) - Full documentation
   - [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture details
   - [MIGRATION.md](MIGRATION.md) - Migration guide

2. **Explore the Code**
   - Start with ViewModels to understand UI logic
   - Look at Services for business logic
   - Check Repositories for data access

3. **Customize the App**
   - Modify styles in `Resources/Styles/`
   - Add new pages following existing patterns
   - Extend entities with new properties

4. **Contribute**
   - See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines
   - Check open issues for tasks
   - Submit pull requests with improvements

## Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)

## Getting Help

- **Issues**: [GitHub Issues](https://github.com/fuadwasi/dot-net-core-pos/issues)
- **Discussions**: Check existing issues for solutions
- **Documentation**: Read the docs before asking questions

---

**Ready to build amazing POS features? Let's go! 🚀**
