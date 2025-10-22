using CommunityToolkit.Mvvm.ComponentModel;
using DreamAlchemist.Services.Core;

namespace DreamAlchemist.ViewModels.Base;

public abstract partial class BaseViewModel : ObservableObject
{
    protected readonly INavigationService NavigationService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    /// <summary>
    /// Called when the view appears
    /// </summary>
    public virtual Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the view disappears
    /// </summary>
    public virtual Task OnDisappearingAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Execute an async task with error handling and busy state
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> operation, string? errorMessage = null)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            await operation();
        }
        catch (Exception ex)
        {
            ErrorMessage = errorMessage ?? ex.Message;
            System.Diagnostics.Debug.WriteLine($"Error in {GetType().Name}: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Execute an async task that returns a result
    /// </summary>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, string? errorMessage = null)
    {
        if (IsBusy)
            return default;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            return await operation();
        }
        catch (Exception ex)
        {
            ErrorMessage = errorMessage ?? ex.Message;
            System.Diagnostics.Debug.WriteLine($"Error in {GetType().Name}: {ex.Message}");
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
