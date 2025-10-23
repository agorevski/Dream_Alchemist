using DreamAlchemist.Models.Enums;

namespace DreamAlchemist.Services.Core;

/// <summary>
/// Service for managing application theme and font schemes
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets the currently active theme variant
    /// </summary>
    ThemeVariant CurrentTheme { get; }

    /// <summary>
    /// Gets the currently active font scheme
    /// </summary>
    FontScheme CurrentFontScheme { get; }

    /// <summary>
    /// Event raised when the theme is changed
    /// </summary>
    event EventHandler<ThemeVariant>? ThemeChanged;

    /// <summary>
    /// Event raised when the font scheme is changed
    /// </summary>
    event EventHandler<FontScheme>? FontSchemeChanged;

    /// <summary>
    /// Sets the active theme variant and applies it to the application
    /// </summary>
    void SetTheme(ThemeVariant theme);

    /// <summary>
    /// Sets the active font scheme and applies it to the application
    /// </summary>
    void SetFontScheme(FontScheme scheme);

    /// <summary>
    /// Gets a color value from the current theme by key
    /// </summary>
    string GetColor(string colorKey);

    /// <summary>
    /// Gets a font name from the current font scheme by key
    /// </summary>
    string GetFont(string fontKey);

    /// <summary>
    /// Gets the complete color palette for the current theme
    /// </summary>
    Dictionary<string, string> GetCurrentColorPalette();

    /// <summary>
    /// Gets the complete font definitions for the current scheme
    /// </summary>
    Dictionary<string, string> GetCurrentFontScheme();

    /// <summary>
    /// Initializes the theme service by applying current theme and fonts to resources
    /// </summary>
    void Initialize();
}
