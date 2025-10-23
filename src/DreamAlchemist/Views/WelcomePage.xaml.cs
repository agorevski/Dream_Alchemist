using DreamAlchemist.ViewModels;

namespace DreamAlchemist.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage(WelcomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is WelcomeViewModel viewModel)
        {
            await viewModel.OnAppearingAsync();
        }
    }
}
