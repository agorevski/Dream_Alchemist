using Microsoft.Extensions.Logging;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Services.Game;

namespace DreamAlchemist;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Core Services
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IGameStateService, GameStateService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        
        // Register Game Services
        builder.Services.AddSingleton<IEventService, EventService>();
        builder.Services.AddSingleton<IMarketService, MarketService>();
        builder.Services.AddSingleton<ICraftingService, CraftingService>();
        builder.Services.AddSingleton<IInventoryService, InventoryService>();
        builder.Services.AddSingleton<ITravelService, TravelService>();
        
        // Register ViewModels
        builder.Services.AddTransient<ViewModels.MainViewModel>();
        builder.Services.AddTransient<ViewModels.MarketViewModel>();
        builder.Services.AddTransient<ViewModels.LabViewModel>();
        builder.Services.AddTransient<ViewModels.InventoryViewModel>();
        builder.Services.AddTransient<ViewModels.TravelViewModel>();
        
        // Register Views
        builder.Services.AddTransient<Views.MainPage>();
        builder.Services.AddTransient<Views.MarketPage>();
        builder.Services.AddTransient<Views.LabPage>();
        builder.Services.AddTransient<Views.InventoryPage>();
        builder.Services.AddTransient<Views.TravelPage>();

#if DEBUG
        // Diagnostic tools only available in DEBUG builds
        builder.Services.AddTransient<ViewModels.DiagnosticViewModel>();
        builder.Services.AddTransient<Views.DiagnosticPage>();
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
