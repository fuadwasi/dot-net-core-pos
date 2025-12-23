# Code Examples

This document provides practical examples of common tasks and extensions for the POS System.

## Table of Contents

- [Adding a New Entity](#adding-a-new-entity)
- [Adding a New Page](#adding-a-new-page)
- [Custom Repository Methods](#custom-repository-methods)
- [Business Logic in Services](#business-logic-in-services)
- [ViewModel Examples](#viewmodel-examples)
- [Navigation Examples](#navigation-examples)
- [Data Validation](#data-validation)
- [Error Handling](#error-handling)

## Adding a New Entity

Let's add a `Category` entity for organizing products.

### 1. Create the Entity (Domain Layer)

```csharp
// src/POSSystem.Domain/Entities/Category.cs
namespace POSSystem.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
```

### 2. Update Product Entity

```csharp
// src/POSSystem.Domain/Entities/Product.cs
public class Product : BaseEntity
{
    // ... existing properties ...
    
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}
```

### 3. Create Repository Interface

```csharp
// src/POSSystem.Domain/Interfaces/ICategoryRepository.cs
using POSSystem.Domain.Entities;

namespace POSSystem.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<Category?> GetByNameAsync(string name);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
}
```

### 4. Implement Repository

```csharp
// src/POSSystem.Infrastructure/Repositories/CategoryRepository.cs
using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(POSDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync();
    }
}
```

### 5. Update DbContext

```csharp
// src/POSSystem.Infrastructure/Data/POSDbContext.cs
public class POSDbContext : DbContext
{
    // ... existing DbSets ...
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Update Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            // ... existing configuration ...
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
```

### 6. Update Unit of Work

```csharp
// src/POSSystem.Domain/Interfaces/IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICustomerRepository Customers { get; }
    ISaleRepository Sales { get; }
    ICategoryRepository Categories { get; } // Add this
    Task<int> SaveChangesAsync();
}

// src/POSSystem.Infrastructure/Repositories/UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    private readonly POSDbContext _context;
    private IProductRepository? _products;
    private ICustomerRepository? _customers;
    private ISaleRepository? _sales;
    private ICategoryRepository? _categories; // Add this

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
    public ISaleRepository Sales => _sales ??= new SaleRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context); // Add this
    
    // ... rest of implementation
}
```

### 7. Create Service

```csharp
// src/POSSystem.Application/Services/CategoryService.cs
using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;

namespace POSSystem.Application.Services;

public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetActiveCategoriesAsync();
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        // Validate unique name
        var existing = await _unitOfWork.Categories.GetByNameAsync(category.Name);
        if (existing != null)
            throw new InvalidOperationException($"Category '{category.Name}' already exists");

        var result = await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _unitOfWork.Categories.GetProductsByCategoryAsync(categoryId);
    }
}
```

### 8. Register in DI Container

```csharp
// src/POSSystem.Maui/MauiProgram.cs
builder.Services.AddScoped<CategoryService>();
builder.Services.AddTransient<CategoriesViewModel>();
builder.Services.AddTransient<CategoriesPage>();
```

## Adding a New Page

Let's create a Categories management page.

### 1. Create ViewModel

```csharp
// src/POSSystem.Maui/ViewModels/CategoriesViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSSystem.Application.Services;
using POSSystem.Domain.Entities;
using System.Collections.ObjectModel;

namespace POSSystem.Maui.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly CategoryService _categoryService;

    [ObservableProperty]
    private ObservableCollection<Category> categories = new();

    [ObservableProperty]
    private string newCategoryName = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    public CategoriesViewModel(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [RelayCommand]
    public async Task LoadCategoriesAsync()
    {
        IsLoading = true;
        try
        {
            var items = await _categoryService.GetActiveCategoriesAsync();
            Categories.Clear();
            foreach (var item in items)
            {
                Categories.Add(item);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", 
                $"Failed to load categories: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task AddCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName))
        {
            await Shell.Current.DisplayAlert("Validation", 
                "Please enter a category name", "OK");
            return;
        }

        try
        {
            var category = new Category { Name = NewCategoryName };
            var result = await _categoryService.CreateCategoryAsync(category);
            Categories.Add(result);
            NewCategoryName = string.Empty;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", 
                $"Failed to add category: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task ViewProductsAsync(Category category)
    {
        await Shell.Current.GoToAsync($"CategoryProducts?categoryId={category.Id}");
    }
}
```

### 2. Create XAML Page

```xml
<!-- src/POSSystem.Maui/Pages/CategoriesPage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="POSSystem.Maui.Pages.CategoriesPage"
             Title="Categories">

    <Grid RowDefinitions="Auto,*" Padding="10">
        
        <!-- Add Category Section -->
        <Grid Grid.Row="0" ColumnDefinitions="*,Auto" ColumnSpacing="10" Margin="10">
            <Entry Grid.Column="0" 
                   Placeholder="Enter category name"
                   Text="{Binding NewCategoryName}"
                   ReturnCommand="{Binding AddCategoryCommand}" />
            <Button Grid.Column="1" 
                    Text="Add" 
                    Command="{Binding AddCategoryCommand}" />
        </Grid>

        <!-- Categories List -->
        <CollectionView Grid.Row="1" ItemsSource="{Binding Categories}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Frame Margin="10,5" Padding="15" CornerRadius="10">
                        <Frame.GestureRecognizers>
                            <TapGestureRecognizer 
                                Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, 
                                         Path=BindingContext.ViewProductsCommand}"
                                CommandParameter="{Binding .}" />
                        </Frame.GestureRecognizers>
                        <Grid>
                            <Label Text="{Binding Name}" 
                                   FontSize="18" 
                                   FontAttributes="Bold" />
                            <Label Text="View Products ›" 
                                   FontSize="12" 
                                   TextColor="Gray"
                                   HorizontalOptions="End" 
                                   VerticalOptions="Center" />
                        </Grid>
                    </Frame>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <ActivityIndicator Grid.Row="1" 
                         IsRunning="{Binding IsLoading}" 
                         IsVisible="{Binding IsLoading}" />
    </Grid>

</ContentPage>
```

### 3. Create Code-Behind

```csharp
// src/POSSystem.Maui/Pages/CategoriesPage.xaml.cs
using POSSystem.Maui.ViewModels;

namespace POSSystem.Maui.Pages;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesViewModel _viewModel;

    public CategoriesPage(CategoriesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCategoriesCommand.ExecuteAsync(null);
    }
}
```

### 4. Register Route in AppShell

```csharp
// src/POSSystem.Maui/AppShell.xaml.cs
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Pages.CategoriesPage), 
            typeof(Pages.CategoriesPage));
    }
}
```

## Custom Repository Methods

Example: Advanced product search with multiple criteria.

```csharp
// Interface
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria);
}

// Criteria class
public class ProductSearchCriteria
{
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? CategoryId { get; set; }
    public bool? InStock { get; set; }
}

// Implementation
public async Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria)
{
    var query = _dbSet.AsQueryable();

    if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
    {
        query = query.Where(p => 
            p.Name.Contains(criteria.SearchTerm) ||
            (p.Description != null && p.Description.Contains(criteria.SearchTerm)));
    }

    if (criteria.MinPrice.HasValue)
        query = query.Where(p => p.Price >= criteria.MinPrice.Value);

    if (criteria.MaxPrice.HasValue)
        query = query.Where(p => p.Price <= criteria.MaxPrice.Value);

    if (criteria.CategoryId.HasValue)
        query = query.Where(p => p.CategoryId == criteria.CategoryId.Value);

    if (criteria.InStock.HasValue && criteria.InStock.Value)
        query = query.Where(p => p.StockQuantity > 0);

    return await query
        .Include(p => p.Category)
        .OrderBy(p => p.Name)
        .ToListAsync();
}
```

## Business Logic in Services

Example: Handling complex sale creation with discounts and promotions.

```csharp
public class SaleService
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<SaleResult> CreateSaleWithPromotionsAsync(Sale sale)
    {
        // Validate sale
        if (sale.SaleItems == null || !sale.SaleItems.Any())
            throw new InvalidOperationException("Sale must have at least one item");

        // Calculate item totals
        foreach (var item in sale.SaleItems)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product {item.ProductId} not found");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for {product.Name}. Available: {product.StockQuantity}");

            item.UnitPrice = product.Price;
            item.TotalPrice = (item.UnitPrice * item.Quantity) - item.Discount;
        }

        // Apply automatic discounts
        sale.SubTotal = sale.SaleItems.Sum(i => i.TotalPrice);
        
        // Example: 10% discount for orders over $100
        if (sale.SubTotal > 100)
        {
            sale.DiscountAmount += sale.SubTotal * 0.10m;
        }

        // Calculate final total
        sale.TotalAmount = sale.SubTotal - sale.DiscountAmount + sale.TaxAmount;
        sale.SaleDate = DateTime.UtcNow;

        // Save sale
        var result = await _unitOfWork.Sales.AddAsync(sale);

        // Update stock
        foreach (var item in sale.SaleItems)
        {
            await _unitOfWork.Products.UpdateStockAsync(item.ProductId, -item.Quantity);
        }

        // Update customer
        if (sale.CustomerId.HasValue)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(sale.CustomerId.Value);
            if (customer != null)
            {
                customer.TotalPurchases += sale.TotalAmount;
                await _unitOfWork.Customers.UpdateAsync(customer);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return new SaleResult
        {
            Sale = result,
            DiscountApplied = sale.DiscountAmount,
            Message = "Sale completed successfully"
        };
    }
}

public class SaleResult
{
    public Sale Sale { get; set; } = null!;
    public decimal DiscountApplied { get; set; }
    public string Message { get; set; } = string.Empty;
}
```

## ViewModel Examples

### Example: Form with Validation

```csharp
public partial class ProductFormViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Product name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    private string name = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0.01, 999999, ErrorMessage = "Price must be between 0.01 and 999999")]
    private decimal price;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be positive")]
    private int stockQuantity;

    [RelayCommand(CanExecute = nameof(CanSave))]
    public async Task SaveAsync()
    {
        var product = new Product
        {
            Name = Name,
            Price = Price,
            StockQuantity = StockQuantity
        };

        await _productService.CreateProductAsync(product);
        await Shell.Current.GoToAsync("..");
    }

    private bool CanSave()
    {
        return !HasErrors && 
               !string.IsNullOrWhiteSpace(Name) &&
               Price > 0;
    }

    partial void OnNameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnPriceChanged(decimal value) => SaveCommand.NotifyCanExecuteChanged();
}
```

## Navigation Examples

### Passing Parameters

```csharp
// Navigate with query parameters
await Shell.Current.GoToAsync($"ProductDetail?id={product.Id}");

// Receive parameters
[QueryProperty(nameof(ProductId), "id")]
public partial class ProductDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private int productId;

    partial void OnProductIdChanged(int value)
    {
        LoadProductAsync(value);
    }
}
```

### Modal Navigation

```csharp
// Show modal
await Shell.Current.GoToAsync("EditProduct", true);

// Dismiss modal
await Shell.Current.GoToAsync("..");
```

## Data Validation

### Entity-Level Validation

```csharp
public class Product : BaseEntity, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Name is required", new[] { nameof(Name) });

        if (Price <= 0)
            yield return new ValidationResult("Price must be greater than 0", new[] { nameof(Price) });

        if (StockQuantity < 0)
            yield return new ValidationResult("Stock cannot be negative", new[] { nameof(StockQuantity) });
    }
}
```

## Error Handling

### Global Error Handler

```csharp
// In MauiProgram.cs
builder.Services.AddSingleton<IErrorHandler, ErrorHandler>();

public class ErrorHandler : IErrorHandler
{
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(ILogger<ErrorHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleErrorAsync(Exception exception)
    {
        _logger.LogError(exception, "An error occurred");

        var message = exception switch
        {
            InvalidOperationException => exception.Message,
            ArgumentException => exception.Message,
            _ => "An unexpected error occurred"
        };

        await Shell.Current.DisplayAlert("Error", message, "OK");
    }
}

// Usage in ViewModel
try
{
    await _productService.CreateProductAsync(product);
}
catch (Exception ex)
{
    await _errorHandler.HandleErrorAsync(ex);
}
```

---

These examples demonstrate common patterns and best practices for extending the POS System.
