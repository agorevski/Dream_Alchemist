using DreamAlchemist.Services.Data;
using DreamAlchemist.Services.Core;

namespace DreamAlchemist;

public partial class App : Application
{
    private readonly IDatabaseService _databaseService;
    private readonly IThemeService _themeService;

    public App(IDatabaseService databaseService, IThemeService themeService)
    {
        InitializeComponent();
        
        _databaseService = databaseService;
        _themeService = themeService;
        
        // Initialize database tables and theme system
        InitializeApp();
    }

    private async void InitializeApp()
    {
        try
        {
            // Initialize theme system first
            _themeService.Initialize();
            System.Diagnostics.Debug.WriteLine("Theme system initialized");
            
            // Only initialize database structure, not game state
            // Game state will be initialized from WelcomePage based on user choice
            await _databaseService.InitializeDatabaseAsync();
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
