# Architecture Overview

This document provides a detailed overview of the POS System architecture, design decisions, and implementation patterns.

## Table of Contents

- [Architecture Principles](#architecture-principles)
- [Layered Architecture](#layered-architecture)
- [Design Patterns](#design-patterns)
- [Data Flow](#data-flow)
- [Technology Stack](#technology-stack)
- [Cross-Cutting Concerns](#cross-cutting-concerns)

## Architecture Principles

### Clean Architecture

The application follows Clean Architecture principles:

1. **Independence**: Business logic is independent of frameworks, UI, and databases
2. **Testability**: Business logic can be tested without UI, database, or external dependencies
3. **UI Independence**: UI can change without affecting business logic
4. **Database Independence**: Can swap databases without changing business logic
5. **External Agency Independence**: Business rules don't depend on external systems

### Dependency Rule

Dependencies point inward:
```
UI Layer → Application Layer → Domain Layer ← Infrastructure Layer
```

The Domain layer has no dependencies on any other layer.

## Layered Architecture

### 1. Domain Layer (`POSSystem.Domain`)

**Purpose**: Contains the core business logic and domain entities

**Responsibilities**:
- Define business entities (Product, Customer, Sale, SaleItem)
- Define repository interfaces
- Contains no external dependencies

**Structure**:
```
POSSystem.Domain/
├── Entities/
│   ├── BaseEntity.cs          # Common entity properties
│   ├── Product.cs             # Product entity
│   ├── Customer.cs            # Customer entity
│   ├── Sale.cs                # Sale entity
│   └── SaleItem.cs            # Sale item entity
└── Interfaces/
    ├── IRepository.cs         # Generic repository interface
    ├── IProductRepository.cs  # Product-specific operations
    ├── ICustomerRepository.cs # Customer-specific operations
    ├── ISaleRepository.cs     # Sale-specific operations
    └── IUnitOfWork.cs         # Unit of Work pattern
```

**Key Entities**:

```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
```

All entities inherit from `BaseEntity` providing common properties.

### 2. Infrastructure Layer (`POSSystem.Infrastructure`)

**Purpose**: Implements data access and external concerns

**Responsibilities**:
- Implement repository interfaces
- Configure Entity Framework Core
- Manage database migrations
- Handle data persistence

**Structure**:
```
POSSystem.Infrastructure/
├── Data/
│   └── POSDbContext.cs        # EF Core DbContext
└── Repositories/
    ├── Repository.cs          # Generic repository implementation
    ├── ProductRepository.cs   # Product repository
    ├── CustomerRepository.cs  # Customer repository
    ├── SaleRepository.cs      # Sale repository
    └── UnitOfWork.cs          # Unit of Work implementation
```

**Database Context**:

```csharp
public class POSDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations
        // Relationships
        // Indexes
        // Seed data
    }
}
```

**Repository Pattern**:

```csharp
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly POSDbContext _context;
    protected readonly DbSet<T> _dbSet;

    // CRUD operations with async/await
    public virtual async Task<T?> GetByIdAsync(int id)
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    public virtual async Task<T> AddAsync(T entity)
    public virtual async Task<T> UpdateAsync(T entity)
    public virtual async Task<bool> DeleteAsync(int id)
}
```

### 3. Application Layer (`POSSystem.Application`)

**Purpose**: Implements business logic and orchestrates domain operations

**Responsibilities**:
- Coordinate between UI and domain
- Implement business workflows
- Handle transactions
- Apply business rules

**Structure**:
```
POSSystem.Application/
└── Services/
    ├── ProductService.cs      # Product business logic
    ├── CustomerService.cs     # Customer business logic
    └── SaleService.cs         # Sale business logic
```

**Service Example**:

```csharp
public class ProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Product> CreateProductAsync(Product product)
    {
        var result = await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }
}
```

### 4. Presentation Layer (`POSSystem.Maui`)

**Purpose**: Provides cross-platform user interface

**Responsibilities**:
- Display data to users
- Capture user input
- Navigate between screens
- Bind data to UI

**Structure**:
```
POSSystem.Maui/
├── Pages/                     # XAML views
│   ├── MainPage.xaml
│   ├── ProductsPage.xaml
│   ├── CustomersPage.xaml
│   ├── SalesPage.xaml
│   └── NewSalePage.xaml
├── ViewModels/                # MVVM ViewModels
│   ├── MainViewModel.cs
│   ├── ProductsViewModel.cs
│   ├── CustomersViewModel.cs
│   ├── SalesViewModel.cs
│   └── NewSaleViewModel.cs
├── Resources/                 # Assets
│   ├── Styles/
│   ├── Images/
│   └── Fonts/
├── Platforms/                 # Platform-specific code
│   ├── Android/
│   ├── iOS/
│   ├── MacCatalyst/
│   └── Windows/
├── App.xaml                   # Application resources
├── AppShell.xaml              # Navigation structure
└── MauiProgram.cs             # Dependency injection setup
```

## Design Patterns

### 1. Repository Pattern

**Purpose**: Abstracts data access logic

**Implementation**:
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}
```

**Benefits**:
- Separation of concerns
- Easy to test (mock repositories)
- Centralized data access logic
- Can switch data sources

### 2. Unit of Work Pattern

**Purpose**: Manages transactions across multiple repositories

**Implementation**:
```csharp
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICustomerRepository Customers { get; }
    ISaleRepository Sales { get; }
    Task<int> SaveChangesAsync();
}
```

**Benefits**:
- Ensures transaction consistency
- Single commit point
- Coordinates multiple repositories

### 3. MVVM Pattern

**Purpose**: Separates UI from business logic

**Implementation**:
```csharp
// ViewModel
public partial class ProductsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    [RelayCommand]
    public async Task LoadProductsAsync()
    {
        var items = await _productService.GetActiveProductsAsync();
        Products.Clear();
        foreach (var item in items) Products.Add(item);
    }
}

// View (XAML)
<CollectionView ItemsSource="{Binding Products}" />
```

**Benefits**:
- Testable ViewModels
- Reusable business logic
- Clean separation of concerns
- Data binding

### 4. Dependency Injection

**Purpose**: Loose coupling and testability

**Implementation**:
```csharp
// Registration (MauiProgram.cs)
builder.Services.AddDbContext<POSDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddTransient<ProductsViewModel>();

// Usage
public class ProductsViewModel
{
    private readonly ProductService _productService;
    
    public ProductsViewModel(ProductService productService)
    {
        _productService = productService;
    }
}
```

**Benefits**:
- Easy to test (inject mocks)
- Loose coupling
- Configuration in one place
- Lifetime management

## Data Flow

### Read Operation Flow

```
User Action
    ↓
View (XAML)
    ↓
ViewModel (Command)
    ↓
Service (Business Logic)
    ↓
Repository (Data Access)
    ↓
DbContext (EF Core)
    ↓
SQLite Database
    ↓
(Return path reverses)
```

### Write Operation Flow

```
User Input
    ↓
View (Binding)
    ↓
ViewModel (Command)
    ↓
Service (Validation + Logic)
    ↓
Repository (Add/Update)
    ↓
Unit of Work (SaveChangesAsync)
    ↓
DbContext (Transaction)
    ↓
SQLite Database
```

### Example: Creating a Sale

```csharp
// 1. User clicks "Complete Sale" button
// 2. View triggers Command in ViewModel
[RelayCommand]
public async Task ProcessSaleAsync()
{
    // 3. ViewModel prepares Sale entity
    var sale = new Sale { /* properties */ };
    
    // 4. ViewModel calls Service
    await _saleService.CreateSaleAsync(sale);
}

// 5. Service applies business rules
public async Task<Sale> CreateSaleAsync(Sale sale)
{
    // Calculate totals
    sale.TotalAmount = sale.SubTotal + sale.TaxAmount - sale.DiscountAmount;
    
    // 6. Service uses Repository through Unit of Work
    var result = await _unitOfWork.Sales.AddAsync(sale);
    
    // Update stock
    foreach (var item in sale.SaleItems)
        await _unitOfWork.Products.UpdateStockAsync(item.ProductId, -item.Quantity);
    
    // 7. Commit transaction
    await _unitOfWork.SaveChangesAsync();
    
    return result;
}
```

## Technology Stack

### Core Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 9.0 | Runtime and SDK |
| C# | 13.0 | Programming language |
| MAUI | 9.0 | Cross-platform UI framework |
| Entity Framework Core | 9.0 | ORM for data access |
| SQLite | Latest | Cross-platform database |

### Libraries

| Library | Purpose |
|---------|---------|
| CommunityToolkit.Maui | MAUI extensions and controls |
| CommunityToolkit.Mvvm | MVVM helpers (ObservableObject, RelayCommand) |
| Microsoft.EntityFrameworkCore.Sqlite | SQLite provider for EF Core |

## Cross-Cutting Concerns

### 1. Error Handling

**Strategy**: Try-catch in ViewModels with user-friendly messages

```csharp
[RelayCommand]
public async Task LoadProductsAsync()
{
    try
    {
        var items = await _productService.GetActiveProductsAsync();
        // Process items
    }
    catch (Exception ex)
    {
        await Shell.Current.DisplayAlert("Error", 
            $"Failed to load products: {ex.Message}", "OK");
    }
}
```

### 2. Logging

**Current**: Debug output
**Recommendation**: Add structured logging with Microsoft.Extensions.Logging

```csharp
// Future enhancement
private readonly ILogger<ProductService> _logger;

public async Task<Product> GetByIdAsync(int id)
{
    _logger.LogInformation("Retrieving product {ProductId}", id);
    // Implementation
}
```

### 3. Validation

**Entity-level validation**:
```csharp
public class Product : BaseEntity
{
    private string _name = string.Empty;
    
    public string Name 
    { 
        get => _name;
        set => _name = string.IsNullOrWhiteSpace(value) 
            ? throw new ArgumentException("Name is required")
            : value;
    }
}
```

### 4. Navigation

**Shell-based navigation**:
```csharp
// AppShell.xaml defines routes
<ShellContent Title="Products" Route="ProductsPage" />

// Navigation from ViewModel
await Shell.Current.GoToAsync("ProductsPage");
```

### 5. State Management

**ViewModel-based state**:
- Each page has its own ViewModel
- State is loaded in `OnAppearing`
- ObservableCollection for dynamic lists

## Performance Considerations

### Database Performance

1. **Async Operations**: All database calls use async/await
2. **Connection Pooling**: EF Core manages connections
3. **Indexes**: Added on frequently queried fields
4. **Eager Loading**: Use `.Include()` for related data

### UI Performance

1. **Virtualization**: CollectionView virtualizes items
2. **Async Loading**: Data loaded asynchronously
3. **Compiled XAML**: XAML is compiled for performance
4. **Efficient Binding**: Use ObservableObject from CommunityToolkit

## Security Considerations

1. **SQL Injection**: EF Core parameterizes queries
2. **Input Validation**: Validate at multiple layers
3. **Database Encryption**: Consider SQLCipher for sensitive data
4. **Secure Storage**: Use SecureStorage for credentials

## Testing Strategy

### Unit Tests
- Test services with mocked repositories
- Test repository logic with in-memory database
- Test ViewModels with mocked services

### Integration Tests
- Test full stack with test database
- Test API endpoints (if added)

### UI Tests
- Use MAUI UI Testing framework
- Test user workflows

## Future Enhancements

1. **Add API Layer**: RESTful API for web clients
2. **Implement Caching**: Redis or in-memory cache
3. **Add Message Queue**: For async processing
4. **Implement CQRS**: Separate read and write models
5. **Add Event Sourcing**: For audit trail
6. **Microservices**: Split into smaller services

---

This architecture provides a solid foundation for building a maintainable, testable, and scalable POS system.
