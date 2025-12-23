using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSSystem.Application.Services;
using POSSystem.Domain.Entities;
using System.Collections.ObjectModel;

namespace POSSystem.Maui.ViewModels;

public partial class SalesViewModel : ObservableObject
{
    private readonly SaleService _saleService;

    [ObservableProperty]
    private ObservableCollection<Sale> sales = new();

    [ObservableProperty]
    private DateTime startDate = DateTime.Today.AddDays(-30);

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isRefreshing;

    public SalesViewModel(SaleService saleService)
    {
        _saleService = saleService;
    }

    [RelayCommand]
    public async Task LoadSalesAsync()
    {
        IsLoading = true;
        try
        {
            var items = await _saleService.GetSalesByDateRangeAsync(StartDate, EndDate.AddDays(1));
            Sales.Clear();
            foreach (var item in items)
            {
                Sales.Add(item);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load sales: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task RefreshSalesAsync()
    {
        IsRefreshing = true;
        await LoadSalesAsync();
        IsRefreshing = false;
    }
}
