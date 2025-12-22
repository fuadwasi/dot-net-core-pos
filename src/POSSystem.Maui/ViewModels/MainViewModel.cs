using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSSystem.Application.Services;

namespace POSSystem.Maui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SaleService _saleService;

    [ObservableProperty]
    private decimal todaySales;

    [ObservableProperty]
    private decimal weekSales;

    [ObservableProperty]
    private decimal monthSales;

    [ObservableProperty]
    private bool isLoading;

    public MainViewModel(SaleService saleService)
    {
        _saleService = saleService;
    }

    [RelayCommand]
    public async Task LoadDashboardDataAsync()
    {
        IsLoading = true;
        try
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            TodaySales = await _saleService.GetTotalSalesAmountAsync(today, today.AddDays(1));
            WeekSales = await _saleService.GetTotalSalesAmountAsync(startOfWeek, today.AddDays(1));
            MonthSales = await _saleService.GetTotalSalesAmountAsync(startOfMonth, today.AddDays(1));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load dashboard data: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
