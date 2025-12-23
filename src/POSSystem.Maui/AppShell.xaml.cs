namespace POSSystem.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Pages.ProductsPage), typeof(Pages.ProductsPage));
        Routing.RegisterRoute(nameof(Pages.CustomersPage), typeof(Pages.CustomersPage));
        Routing.RegisterRoute(nameof(Pages.SalesPage), typeof(Pages.SalesPage));
        Routing.RegisterRoute(nameof(Pages.NewSalePage), typeof(Pages.NewSalePage));
    }
}
