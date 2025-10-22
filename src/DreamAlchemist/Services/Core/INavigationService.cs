namespace DreamAlchemist.Services.Core;

public interface INavigationService
{
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);
    Task NavigateBackAsync();
    Task PopToRootAsync();
}
