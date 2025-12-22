using POSSystem.Maui.ViewModels;

namespace POSSystem.Maui.Pages;

public partial class ProductsPage : ContentPage
{
    private readonly ProductsViewModel _viewModel;

    public ProductsPage(ProductsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProductsCommand.ExecuteAsync(null);
    }
}
