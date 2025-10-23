using DreamAlchemist.ViewModels;

namespace DreamAlchemist.Views;

public partial class DiagnosticPage : ContentPage
{
    public DiagnosticPage(DiagnosticViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
