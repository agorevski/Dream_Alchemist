namespace DreamAlchemist;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute("MainPage", typeof(Views.MainPage));
        Routing.RegisterRoute("MarketPage", typeof(Views.MarketPage));
        Routing.RegisterRoute("LabPage", typeof(Views.LabPage));
        Routing.RegisterRoute("InventoryPage", typeof(Views.InventoryPage));
        Routing.RegisterRoute("TravelPage", typeof(Views.TravelPage));
        Routing.RegisterRoute("DiagnosticPage", typeof(Views.DiagnosticPage));
    }
}
