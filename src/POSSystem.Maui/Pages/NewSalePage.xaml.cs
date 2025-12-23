using POSSystem.Maui.ViewModels;

namespace POSSystem.Maui.Pages;

public partial class NewSalePage : ContentPage
{
    private readonly NewSaleViewModel _viewModel;

    public NewSalePage(NewSaleViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeCommand.ExecuteAsync(null);
    }
}
