using DreamAlchemist.ViewModels;

namespace DreamAlchemist.Views;

public partial class TravelPage : ContentPage
{
    private readonly TravelViewModel _viewModel;

    public TravelPage(TravelViewModel viewModel)
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
