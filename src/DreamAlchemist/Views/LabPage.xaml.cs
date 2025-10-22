using DreamAlchemist.ViewModels;

namespace DreamAlchemist.Views;

public partial class LabPage : ContentPage
{
    private readonly LabViewModel _viewModel;

    public LabPage(LabViewModel viewModel)
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
