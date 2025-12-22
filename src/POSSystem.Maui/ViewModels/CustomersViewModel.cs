using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSSystem.Application.Services;
using POSSystem.Domain.Entities;
using System.Collections.ObjectModel;

namespace POSSystem.Maui.ViewModels;

public partial class CustomersViewModel : ObservableObject
{
    private readonly CustomerService _customerService;

    [ObservableProperty]
    private ObservableCollection<Customer> customers = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isRefreshing;

    public CustomersViewModel(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [RelayCommand]
    public async Task LoadCustomersAsync()
    {
        IsLoading = true;
        try
        {
            var items = await _customerService.GetActiveCustomersAsync();
            Customers.Clear();
            foreach (var item in items)
            {
                Customers.Add(item);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load customers: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task RefreshCustomersAsync()
    {
        IsRefreshing = true;
        await LoadCustomersAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    public async Task SearchCustomersAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadCustomersAsync();
            return;
        }

        IsLoading = true;
        try
        {
            var items = await _customerService.SearchCustomersAsync(SearchText);
            Customers.Clear();
            foreach (var item in items)
            {
                Customers.Add(item);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to search customers: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteCustomerAsync(Customer customer)
    {
        if (customer == null) return;

        bool answer = await Shell.Current.DisplayAlert("Confirm Delete", 
            $"Are you sure you want to delete {customer.Name}?", "Yes", "No");
        
        if (!answer) return;

        try
        {
            await _customerService.DeleteCustomerAsync(customer.Id);
            Customers.Remove(customer);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to delete customer: {ex.Message}", "OK");
        }
    }
}
