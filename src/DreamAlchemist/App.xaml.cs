using DreamAlchemist.Services.Core;

namespace DreamAlchemist;

public partial class App : Application
{
    private readonly IGameStateService _gameStateService;

    public App(IGameStateService gameStateService)
    {
        InitializeComponent();
        
        _gameStateService = gameStateService;
        
        // Initialize game state asynchronously
        InitializeApp();
    }

    private async void InitializeApp()
    {
        try
        {
            await _gameStateService.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing app: {ex.Message}");
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell()) 
        {
            Title = "Dream Alchemist"
        };
    }
}
