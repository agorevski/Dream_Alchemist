using DreamAlchemist.Helpers;
using DreamAlchemist.Models.Enums;

namespace DreamAlchemist.Services.Core;

/// <summary>
/// Implementation of theme management service with runtime theme switching
/// </summary>
public class ThemeService : IThemeService
{
    private ThemeVariant _currentTheme;
    private FontScheme _currentFontScheme;
    private Dictionary<string, string> _currentColors;
    private Dictionary<string, string> _currentFonts;

    public ThemeVariant CurrentTheme => _currentTheme;
    public FontScheme CurrentFontScheme => _currentFontScheme;

    public event EventHandler<ThemeVariant>? ThemeChanged;
    public event EventHandler<FontScheme>? FontSchemeChanged;

    public ThemeService()
    {
        // Initialize with default theme
        _currentTheme = ThemeVariant.VibrantEnergy;
        _currentFontScheme = FontScheme.ClassicMystical;
        _currentColors = ThemeConstants.GetColorPalette(_currentTheme);
        _currentFonts = ThemeConstants.GetFontScheme(_currentFontScheme);
    }

    public void SetTheme(ThemeVariant theme)
    {
        if (_currentTheme == theme)
            return;

        _currentTheme = theme;
        _currentColors = ThemeConstants.GetColorPalette(theme);

        // Update application resource dictionary
        ApplyThemeToResources();

        // Notify listeners
        ThemeChanged?.Invoke(this, theme);

        System.Diagnostics.Debug.WriteLine($"Theme changed to: {theme}");
    }

    public void SetFontScheme(FontScheme scheme)
    {
        if (_currentFontScheme == scheme)
            return;

        _currentFontScheme = scheme;
        _currentFonts = ThemeConstants.GetFontScheme(scheme);

        // Update application resource dictionary
        ApplyFontsToResources();

        // Notify listeners
        FontSchemeChanged?.Invoke(this, scheme);

        System.Diagnostics.Debug.WriteLine($"Font scheme changed to: {scheme}");
    }

    public string GetColor(string colorKey)
    {
        return _currentColors.TryGetValue(colorKey, out var color) ? color : "#FFFFFF";
    }

    public string GetFont(string fontKey)
    {
        return _currentFonts.TryGetValue(fontKey, out var font) ? font : "OpenSansRegular";
    }

    public Dictionary<string, string> GetCurrentColorPalette()
    {
        return new Dictionary<string, string>(_currentColors);
    }

    public Dictionary<string, string> GetCurrentFontScheme()
    {
        return new Dictionary<string, string>(_currentFonts);
    }

    /// <summary>
    /// Applies the current theme colors to the application's resource dictionary
    /// </summary>
    private void ApplyThemeToResources()
    {
        if (Application.Current?.Resources == null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: Application.Current.Resources is null!");
            return;
        }

        var resources = Application.Current.Resources;

        try
        {
            System.Diagnostics.Debug.WriteLine($"=== Applying Theme: {_currentTheme} ===");
            
            // Update color resources
            foreach (var (key, value) in _currentColors)
            {
                var resourceKey = $"Theme{key}";
                var color = Color.FromArgb(value);
                
                if (resources.ContainsKey(resourceKey))
                {
                    resources[resourceKey] = color;
                    System.Diagnostics.Debug.WriteLine($"Updated {resourceKey} = {value}");
                }
                else
                {
                    resources.Add(resourceKey, color);
                    System.Diagnostics.Debug.WriteLine($"Added {resourceKey} = {value}");
                }
            }

            // Log key colors for verification
            System.Diagnostics.Debug.WriteLine($"Primary Color: {_currentColors["Primary"]}");
            System.Diagnostics.Debug.WriteLine($"Secondary Color: {_currentColors["Secondary"]}");
            System.Diagnostics.Debug.WriteLine($"Background: {_currentColors["BackgroundPrimary"]}");
            System.Diagnostics.Debug.WriteLine($"Applied {_currentColors.Count} colors to resources");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying theme: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Applies the current font scheme to the application's resource dictionary
    /// </summary>
    private void ApplyFontsToResources()
    {
        if (Application.Current?.Resources == null)
            return;

        var resources = Application.Current.Resources;

        try
        {
            // Update font resources
            foreach (var (key, value) in _currentFonts)
            {
                var resourceKey = $"Font{key}";
                if (resources.ContainsKey(resourceKey))
                {
                    resources[resourceKey] = value;
                }
                else
                {
                    resources.Add(resourceKey, value);
                }
            }

            System.Diagnostics.Debug.WriteLine($"Applied {_currentFonts.Count} fonts to resources");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying fonts: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes the theme service by applying current theme and fonts
    /// </summary>
    public void Initialize()
    {
        ApplyThemeToResources();
        ApplyFontsToResources();
        System.Diagnostics.Debug.WriteLine("ThemeService initialized");
    }
}
