using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSSystem.Application.Services;
using POSSystem.Domain.Entities;
using System.Collections.ObjectModel;

namespace POSSystem.Maui.ViewModels;

public partial class CartItem : ObservableObject
{
    [ObservableProperty]
    private Product product = null!;

    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private decimal discount;

    public decimal Total => (Product.Price * Quantity) - Discount;
}

public partial class NewSaleViewModel : ObservableObject
{
    private readonly ProductService _productService;
    private readonly CustomerService _customerService;
    private readonly SaleService _saleService;

    [ObservableProperty]
    private ObservableCollection<Product> availableProducts = new();

    [ObservableProperty]
    private ObservableCollection<CartItem> cartItems = new();

    [ObservableProperty]
    private ObservableCollection<Customer> customers = new();

    [ObservableProperty]
    private Customer? selectedCustomer;

    [ObservableProperty]
    private decimal subTotal;

    [ObservableProperty]
    private decimal taxAmount;

    [ObservableProperty]
    private decimal discountAmount;

    [ObservableProperty]
    private decimal total;

    [ObservableProperty]
    private string paymentMethod = "Cash";

    [ObservableProperty]
    private bool isProcessing;

    public NewSaleViewModel(ProductService productService, CustomerService customerService, SaleService saleService)
    {
        _productService = productService;
        _customerService = customerService;
        _saleService = saleService;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        try
        {
            var products = await _productService.GetActiveProductsAsync();
            AvailableProducts.Clear();
            foreach (var product in products)
            {
                AvailableProducts.Add(product);
            }

            var customerList = await _customerService.GetActiveCustomersAsync();
            Customers.Clear();
            foreach (var customer in customerList)
            {
                Customers.Add(customer);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to initialize: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public void AddToCart(Product product)
    {
        if (product == null) return;

        var existingItem = CartItems.FirstOrDefault(ci => ci.Product.Id == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            CartItems.Add(new CartItem { Product = product, Quantity = 1, Discount = 0 });
        }

        CalculateTotals();
    }

    [RelayCommand]
    public void RemoveFromCart(CartItem item)
    {
        if (item == null) return;
        CartItems.Remove(item);
        CalculateTotals();
    }

    partial void OnTaxAmountChanged(decimal value) => CalculateTotals();
    partial void OnDiscountAmountChanged(decimal value) => CalculateTotals();

    private void CalculateTotals()
    {
        SubTotal = CartItems.Sum(item => item.Total);
        Total = SubTotal + TaxAmount - DiscountAmount;
    }

    [RelayCommand]
    public async Task ProcessSaleAsync()
    {
        if (CartItems.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "Cart is empty", "OK");
            return;
        }

        IsProcessing = true;
        try
        {
            var sale = new Sale
            {
                CustomerId = SelectedCustomer?.Id,
                SubTotal = SubTotal,
                TaxAmount = TaxAmount,
                DiscountAmount = DiscountAmount,
                TotalAmount = Total,
                PaymentMethod = PaymentMethod,
                Status = "Completed",
                SaleDate = DateTime.UtcNow,
                SaleItems = CartItems.Select(ci => new SaleItem
                {
                    ProductId = ci.Product.Id,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price,
                    Discount = ci.Discount,
                    TotalPrice = ci.Total
                }).ToList()
            };

            await _saleService.CreateSaleAsync(sale);
            await Shell.Current.DisplayAlert("Success", "Sale completed successfully", "OK");
            
            // Clear cart
            CartItems.Clear();
            SelectedCustomer = null;
            TaxAmount = 0;
            DiscountAmount = 0;
            CalculateTotals();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to process sale: {ex.Message}", "OK");
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
