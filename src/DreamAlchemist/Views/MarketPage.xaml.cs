using DreamAlchemist.ViewModels;

namespace DreamAlchemist.Views;

public partial class MarketPage : ContentPage
{
    private readonly MarketViewModel _viewModel;

    public MarketPage(MarketViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearingAsync();
    }
}
