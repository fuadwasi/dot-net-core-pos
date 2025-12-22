using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSSystem.Application.Services;
using POSSystem.Domain.Entities;
using System.Collections.ObjectModel;

namespace POSSystem.Maui.ViewModels;

public partial class ProductsViewModel : ObservableObject
{
    private readonly ProductService _productService;

    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isRefreshing;

    public ProductsViewModel(ProductService productService)
    {
        _productService = productService;
    }

    [RelayCommand]
    public async Task LoadProductsAsync()
    {
        IsLoading = true;
        try
        {
            var items = await _productService.GetActiveProductsAsync();
            Products.Clear();
            foreach (var item in items)
            {
                Products.Add(item);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load products: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task RefreshProductsAsync()
    {
        IsRefreshing = true;
        await LoadProductsAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    public async Task SearchProductsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadProductsAsync();
            return;
        }

        IsLoading = true;
        try
        {
            var items = await _productService.SearchProductsAsync(SearchText);
            Products.Clear();
            foreach (var item in items)
            {
                Products.Add(item);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to search products: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteProductAsync(Product product)
    {
        if (product == null) return;

        bool answer = await Shell.Current.DisplayAlert("Confirm Delete", 
            $"Are you sure you want to delete {product.Name}?", "Yes", "No");
        
        if (!answer) return;

        try
        {
            await _productService.DeleteProductAsync(product.Id);
            Products.Remove(product);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to delete product: {ex.Message}", "OK");
        }
    }
}
