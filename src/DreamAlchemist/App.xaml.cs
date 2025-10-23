using DreamAlchemist.Services.Data;

namespace DreamAlchemist;

public partial class App : Application
{
    private readonly IDatabaseService _databaseService;

    public App(IDatabaseService databaseService)
    {
        InitializeComponent();
        
        _databaseService = databaseService;
        
        // Initialize database tables only (no game state yet)
        InitializeApp();
    }

    private async void InitializeApp()
    {
        try
        {
            // Only initialize database structure, not game state
            // Game state will be initialized from WelcomePage based on user choice
            await _databaseService.InitializeDatabaseAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
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
