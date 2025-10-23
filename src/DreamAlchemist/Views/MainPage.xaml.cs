using DreamAlchemist.ViewModels;

namespace DreamAlchemist.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        
#if DEBUG
        // Show diagnostics button only in DEBUG builds and wire up click handler
        DiagnosticsButton.IsVisible = true;
        DiagnosticsButton.Clicked += OnDiagnosticsClicked;
#endif
    }

#if DEBUG
    private async void OnDiagnosticsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("DiagnosticPage");
    }
#endif

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearingAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _viewModel.OnDisappearingAsync();
    }
}
