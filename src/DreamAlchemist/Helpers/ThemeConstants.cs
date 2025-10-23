using DreamAlchemist.Models.Enums;

namespace DreamAlchemist.Helpers;

/// <summary>
/// Comprehensive theme constants including colors, fonts, spacing, and typography
/// </summary>
public static class ThemeConstants
{
    // ==================== TYPOGRAPHY SCALE ====================
    
    public const int FONT_SIZE_TINY = 10;
    public const int FONT_SIZE_SMALL = 12;
    public const int FONT_SIZE_BODY = 14;
    public const int FONT_SIZE_LARGE = 16;
    public const int FONT_SIZE_SUBHEADING = 18;
    public const int FONT_SIZE_HEADING = 20;
    public const int FONT_SIZE_TITLE = 24;
    public const int FONT_SIZE_DISPLAY = 32;
    public const int FONT_SIZE_HERO = 42;
    public const int FONT_SIZE_MEGA = 60;

    // ==================== SPACING ====================
    
    public const int SPACING_TINY = 5;
    public const int SPACING_SMALL = 10;
    public const int SPACING_MEDIUM = 15;
    public const int SPACING_LARGE = 20;
    public const int SPACING_XLARGE = 30;
    public const int SPACING_XXLARGE = 40;

    // ==================== CORNER RADIUS ====================
    
    public const int CORNER_RADIUS_SMALL = 8;
    public const int CORNER_RADIUS_MEDIUM = 12;
    public const int CORNER_RADIUS_LARGE = 15;
    public const int CORNER_RADIUS_XLARGE = 20;

    // ==================== SHADOWS ====================
    
    public const int SHADOW_RADIUS_SMALL = 5;
    public const int SHADOW_RADIUS_MEDIUM = 10;
    public const int SHADOW_RADIUS_LARGE = 15;
    public const double SHADOW_OPACITY_SUBTLE = 0.3;
    public const double SHADOW_OPACITY_NORMAL = 0.5;
    public const double SHADOW_OPACITY_STRONG = 0.7;

    // ==================== THEME A: VIBRANT ENERGY ====================
    
    public static class VibrantColors
    {
        // Primary Colors
        public const string Primary = "#A855F7";           // Electric Purple
        public const string PrimaryLight = "#C084FC";      // Lighter Purple
        public const string PrimaryDark = "#7E22CE";       // Deeper Purple
        
        // Secondary Colors
        public const string Secondary = "#06B6D4";         // Cyber Cyan
        public const string SecondaryLight = "#22D3EE";    // Light Cyan
        public const string SecondaryDark = "#0891B2";     // Deep Cyan
        
        // Backgrounds
        public const string BackgroundPrimary = "#0A0E27";    // Deep Space
        public const string BackgroundSecondary = "#1A1F3A";  // Space Blue
        public const string BackgroundTertiary = "#252B4D";   // Lighter Space
        public const string BackgroundElevated = "#303660";   // Elevated Surface
        
        // Accents
        public const string AccentPink = "#EC4899";           // Neon Pink
        public const string AccentBlue = "#3B82F6";           // Electric Blue
        public const string AccentGold = "#FCD34D";           // Bright Gold
        public const string AccentGreen = "#10B981";          // Bright Green
        
        // Status Colors
        public const string Success = "#10B981";              // Emerald
        public const string Danger = "#EF4444";               // Bright Red
        public const string Warning = "#F59E0B";              // Amber
        public const string Info = "#06B6D4";                 // Cyan
        
        // Text Colors
        public const string TextPrimary = "#FFFFFF";          // Pure White
        public const string TextSecondary = "#E5E7EB";        // Light Gray
        public const string TextTertiary = "#9CA3AF";         // Medium Gray
        public const string TextDisabled = "#6B7280";         // Dark Gray
        
        // Rarity Colors (Dream-themed)
        public const string RarityCommon = "#94A3B8";         // Silver
        public const string RarityUncommon = "#22D3EE";       // Aqua
        public const string RarityRare = "#A855F7";           // Violet
        public const string RarityEpic = "#F59E0B";           // Gold
        public const string RarityMythic = "#EC4899";         // Prismatic Pink
        
        // Borders & Dividers
        public const string Border = "#374151";
        public const string BorderFocus = "#A855F7";
        public const string Divider = "#4B5563";
    }

    // ==================== THEME B: ETHEREAL DREAM ====================
    
    public static class EtherealColors
    {
        // Primary Colors
        public const string Primary = "#C4B5FD";           // Lavender
        public const string PrimaryLight = "#DDD6FE";      // Light Lavender
        public const string PrimaryDark = "#A78BFA";       // Deep Lavender
        
        // Secondary Colors
        public const string Secondary = "#A7F3D0";         // Mint Dream
        public const string SecondaryLight = "#D1FAE5";    // Pale Mint
        public const string SecondaryDark = "#6EE7B7";     // Deep Mint
        
        // Backgrounds
        public const string BackgroundPrimary = "#1F1B2E";    // Misty Twilight
        public const string BackgroundSecondary = "#2D2838";  // Soft Purple
        public const string BackgroundTertiary = "#3A3545";   // Medium Purple
        public const string BackgroundElevated = "#474252";   // Light Purple
        
        // Accents
        public const string AccentRose = "#FBCFE8";           // Rose Quartz
        public const string AccentSky = "#BAE6FD";            // Sky Blue
        public const string AccentGold = "#FDE68A";           // Soft Gold
        public const string AccentGreen = "#BBF7D0";          // Soft Green
        
        // Status Colors
        public const string Success = "#86EFAC";              // Soft Emerald
        public const string Danger = "#FCA5A5";               // Soft Red
        public const string Warning = "#FDE047";              // Soft Yellow
        public const string Info = "#BAE6FD";                 // Soft Blue
        
        // Text Colors
        public const string TextPrimary = "#F9FAFB";          // Almost White
        public const string TextSecondary = "#E5E7EB";        // Light Gray
        public const string TextTertiary = "#D1D5DB";         // Soft Gray
        public const string TextDisabled = "#9CA3AF";         // Medium Gray
        
        // Rarity Colors (Dream-themed)
        public const string RarityCommon = "#CBD5E1";         // Moonlight Silver
        public const string RarityUncommon = "#BAE6FD";       // Dream Aqua
        public const string RarityRare = "#C4B5FD";           // Ethereal Violet
        public const string RarityEpic = "#FDE68A";           // Sunbeam Gold
        public const string RarityMythic = "#FBCFE8";         // Cosmic Rose
        
        // Borders & Dividers
        public const string Border = "#4B5563";
        public const string BorderFocus = "#C4B5FD";
        public const string Divider = "#6B7280";
    }

    // ==================== THEME C: MYSTICAL JEWEL ====================
    
    public static class MysticalColors
    {
        // Primary Colors
        public const string Primary = "#7C3AED";           // Royal Amethyst
        public const string PrimaryLight = "#9333EA";      // Bright Amethyst
        public const string PrimaryDark = "#5B21B6";       // Deep Amethyst
        
        // Secondary Colors
        public const string Secondary = "#1E40AF";         // Sapphire
        public const string SecondaryLight = "#2563EB";    // Bright Sapphire
        public const string SecondaryDark = "#1E3A8A";     // Deep Sapphire
        
        // Backgrounds
        public const string BackgroundPrimary = "#0F0A1F";    // Onyx
        public const string BackgroundSecondary = "#1A1433";  // Deep Purple
        public const string BackgroundTertiary = "#251E3D";   // Medium Purple
        public const string BackgroundElevated = "#302847";   // Light Purple
        
        // Accents
        public const string AccentRuby = "#DC2626";           // Ruby Red
        public const string AccentEmerald = "#059669";        // Emerald Green
        public const string AccentTopaz = "#D97706";          // Topaz Orange
        public const string AccentDiamond = "#E0E7FF";        // Diamond White
        
        // Status Colors
        public const string Success = "#059669";              // Deep Emerald
        public const string Danger = "#DC2626";               // Deep Ruby
        public const string Warning = "#D97706";              // Deep Topaz
        public const string Info = "#2563EB";                 // Deep Sapphire
        
        // Text Colors
        public const string TextPrimary = "#F9FAFB";          // Pure White
        public const string TextSecondary = "#E0E7FF";        // Indigo Tint
        public const string TextTertiary = "#C7D2FE";         // Soft Indigo
        public const string TextDisabled = "#818CF8";         // Muted Indigo
        
        // Rarity Colors (Mystical)
        public const string RarityCommon = "#9CA3AF";         // Ash
        public const string RarityUncommon = "#C7D2FE";       // Moonlight
        public const string RarityRare = "#9333EA";           // Stardust
        public const string RarityEpic = "#F59E0B";           // Sunfire
        public const string RarityMythic = "#7C3AED";         // Cosmic Purple
        
        // Borders & Dividers
        public const string Border = "#312E81";
        public const string BorderFocus = "#7C3AED";
        public const string Divider = "#4C1D95";
    }

    // ==================== FONT SCHEME A: CLASSIC MYSTICAL ====================
    
    public static class ClassicMysticalFonts
    {
        public const string Display = "Cinzel";              // Elegant headers
        public const string DisplayBold = "CinzelBold";      // Strong headers
        public const string Body = "Outfit";                 // Readable content
        public const string BodySemiBold = "OutfitSemiBold"; // Emphasis
        public const string BodyBold = "OutfitBold";         // Strong emphasis
        public const string Accent = "Philosopher";          // Mystical elements
        public const string AccentBold = "PhilosopherBold";  // Strong mystical
        public const string Mono = "JetBrainsMono";          // Numbers, stats
    }

    // ==================== FONT SCHEME B: ETHEREAL LIGHT ====================
    
    public static class EtherealLightFonts
    {
        public const string Display = "CormorantGaramond";          // Elegant, literary
        public const string DisplayBold = "CormorantGaramondBold";  // Strong elegance
        public const string Body = "Outfit";                        // Clean reading
        public const string BodySemiBold = "OutfitSemiBold";        // Gentle emphasis
        public const string BodyBold = "OutfitBold";                // Strong emphasis
        public const string Accent = "PoiretOne";                   // Ethereal, delicate
        public const string AccentBold = "PoiretOne";               // (No bold variant)
        public const string Mono = "JetBrainsMono";                 // Technical
    }

    // ==================== FONT SCHEME C: FUTURISTIC DREAM ====================
    
    public static class FuturisticDreamFonts
    {
        public const string Display = "Audiowide";           // Wide, futuristic
        public const string DisplayBold = "Audiowide";       // (No separate bold)
        public const string Body = "Inter";                  // Modern, readable
        public const string BodySemiBold = "InterSemiBold";  // Emphasis
        public const string BodyBold = "InterBold";          // Strong emphasis
        public const string Accent = "Philosopher";          // Mystical contrast
        public const string AccentBold = "PhilosopherBold";  // Strong mystical
        public const string Mono = "JetBrainsMono";          // Technical
    }

    // ==================== HELPER METHODS ====================
    
    /// <summary>
    /// Gets the color palette for the specified theme variant
    /// </summary>
    public static Dictionary<string, string> GetColorPalette(ThemeVariant theme)
    {
        return theme switch
        {
            ThemeVariant.VibrantEnergy => GetVibrantPalette(),
            ThemeVariant.EtherealDream => GetEtherealPalette(),
            ThemeVariant.MysticalJewel => GetMysticalPalette(),
            _ => GetVibrantPalette()
        };
    }

    private static Dictionary<string, string> GetVibrantPalette()
    {
        return new Dictionary<string, string>
        {
            ["Primary"] = VibrantColors.Primary,
            ["PrimaryLight"] = VibrantColors.PrimaryLight,
            ["PrimaryDark"] = VibrantColors.PrimaryDark,
            ["Secondary"] = VibrantColors.Secondary,
            ["SecondaryLight"] = VibrantColors.SecondaryLight,
            ["SecondaryDark"] = VibrantColors.SecondaryDark,
            ["BackgroundPrimary"] = VibrantColors.BackgroundPrimary,
            ["BackgroundSecondary"] = VibrantColors.BackgroundSecondary,
            ["BackgroundTertiary"] = VibrantColors.BackgroundTertiary,
            ["BackgroundElevated"] = VibrantColors.BackgroundElevated,
            ["AccentPink"] = VibrantColors.AccentPink,
            ["AccentBlue"] = VibrantColors.AccentBlue,
            ["AccentGold"] = VibrantColors.AccentGold,
            ["AccentGreen"] = VibrantColors.AccentGreen,
            ["Success"] = VibrantColors.Success,
            ["Danger"] = VibrantColors.Danger,
            ["Warning"] = VibrantColors.Warning,
            ["Info"] = VibrantColors.Info,
            ["TextPrimary"] = VibrantColors.TextPrimary,
            ["TextSecondary"] = VibrantColors.TextSecondary,
            ["TextTertiary"] = VibrantColors.TextTertiary,
            ["TextDisabled"] = VibrantColors.TextDisabled,
            ["RarityCommon"] = VibrantColors.RarityCommon,
            ["RarityUncommon"] = VibrantColors.RarityUncommon,
            ["RarityRare"] = VibrantColors.RarityRare,
            ["RarityEpic"] = VibrantColors.RarityEpic,
            ["RarityMythic"] = VibrantColors.RarityMythic,
            ["Border"] = VibrantColors.Border,
            ["BorderFocus"] = VibrantColors.BorderFocus,
            ["Divider"] = VibrantColors.Divider
        };
    }

    private static Dictionary<string, string> GetEtherealPalette()
    {
        return new Dictionary<string, string>
        {
            ["Primary"] = EtherealColors.Primary,
            ["PrimaryLight"] = EtherealColors.PrimaryLight,
            ["PrimaryDark"] = EtherealColors.PrimaryDark,
            ["Secondary"] = EtherealColors.Secondary,
            ["SecondaryLight"] = EtherealColors.SecondaryLight,
            ["SecondaryDark"] = EtherealColors.SecondaryDark,
            ["BackgroundPrimary"] = EtherealColors.BackgroundPrimary,
            ["BackgroundSecondary"] = EtherealColors.BackgroundSecondary,
            ["BackgroundTertiary"] = EtherealColors.BackgroundTertiary,
            ["BackgroundElevated"] = EtherealColors.BackgroundElevated,
            ["AccentPink"] = EtherealColors.AccentRose,
            ["AccentBlue"] = EtherealColors.AccentSky,
            ["AccentGold"] = EtherealColors.AccentGold,
            ["AccentGreen"] = EtherealColors.AccentGreen,
            ["Success"] = EtherealColors.Success,
            ["Danger"] = EtherealColors.Danger,
            ["Warning"] = EtherealColors.Warning,
            ["Info"] = EtherealColors.Info,
            ["TextPrimary"] = EtherealColors.TextPrimary,
            ["TextSecondary"] = EtherealColors.TextSecondary,
            ["TextTertiary"] = EtherealColors.TextTertiary,
            ["TextDisabled"] = EtherealColors.TextDisabled,
            ["RarityCommon"] = EtherealColors.RarityCommon,
            ["RarityUncommon"] = EtherealColors.RarityUncommon,
            ["RarityRare"] = EtherealColors.RarityRare,
            ["RarityEpic"] = EtherealColors.RarityEpic,
            ["RarityMythic"] = EtherealColors.RarityMythic,
            ["Border"] = EtherealColors.Border,
            ["BorderFocus"] = EtherealColors.BorderFocus,
            ["Divider"] = EtherealColors.Divider
        };
    }

    private static Dictionary<string, string> GetMysticalPalette()
    {
        return new Dictionary<string, string>
        {
            ["Primary"] = MysticalColors.Primary,
            ["PrimaryLight"] = MysticalColors.PrimaryLight,
            ["PrimaryDark"] = MysticalColors.PrimaryDark,
            ["Secondary"] = MysticalColors.Secondary,
            ["SecondaryLight"] = MysticalColors.SecondaryLight,
            ["SecondaryDark"] = MysticalColors.SecondaryDark,
            ["BackgroundPrimary"] = MysticalColors.BackgroundPrimary,
            ["BackgroundSecondary"] = MysticalColors.BackgroundSecondary,
            ["BackgroundTertiary"] = MysticalColors.BackgroundTertiary,
            ["BackgroundElevated"] = MysticalColors.BackgroundElevated,
            ["AccentPink"] = MysticalColors.AccentRuby,
            ["AccentBlue"] = MysticalColors.AccentEmerald,
            ["AccentGold"] = MysticalColors.AccentTopaz,
            ["AccentGreen"] = MysticalColors.AccentDiamond,
            ["Success"] = MysticalColors.Success,
            ["Danger"] = MysticalColors.Danger,
            ["Warning"] = MysticalColors.Warning,
            ["Info"] = MysticalColors.Info,
            ["TextPrimary"] = MysticalColors.TextPrimary,
            ["TextSecondary"] = MysticalColors.TextSecondary,
            ["TextTertiary"] = MysticalColors.TextTertiary,
            ["TextDisabled"] = MysticalColors.TextDisabled,
            ["RarityCommon"] = MysticalColors.RarityCommon,
            ["RarityUncommon"] = MysticalColors.RarityUncommon,
            ["RarityRare"] = MysticalColors.RarityRare,
            ["RarityEpic"] = MysticalColors.RarityEpic,
            ["RarityMythic"] = MysticalColors.RarityMythic,
            ["Border"] = MysticalColors.Border,
            ["BorderFocus"] = MysticalColors.BorderFocus,
            ["Divider"] = MysticalColors.Divider
        };
    }

    /// <summary>
    /// Gets the font definitions for the specified font scheme
    /// </summary>
    public static Dictionary<string, string> GetFontScheme(FontScheme scheme)
    {
        return scheme switch
        {
            FontScheme.ClassicMystical => GetClassicMysticalFonts(),
            FontScheme.EtherealLight => GetEtherealLightFonts(),
            FontScheme.FuturisticDream => GetFuturisticDreamFonts(),
            _ => GetClassicMysticalFonts()
        };
    }

    private static Dictionary<string, string> GetClassicMysticalFonts()
    {
        return new Dictionary<string, string>
        {
            ["Display"] = ClassicMysticalFonts.Display,
            ["DisplayBold"] = ClassicMysticalFonts.DisplayBold,
            ["Body"] = ClassicMysticalFonts.Body,
            ["BodySemiBold"] = ClassicMysticalFonts.BodySemiBold,
            ["BodyBold"] = ClassicMysticalFonts.BodyBold,
            ["Accent"] = ClassicMysticalFonts.Accent,
            ["AccentBold"] = ClassicMysticalFonts.AccentBold,
            ["Mono"] = ClassicMysticalFonts.Mono
        };
    }

    private static Dictionary<string, string> GetEtherealLightFonts()
    {
        return new Dictionary<string, string>
        {
            ["Display"] = EtherealLightFonts.Display,
            ["DisplayBold"] = EtherealLightFonts.DisplayBold,
            ["Body"] = EtherealLightFonts.Body,
            ["BodySemiBold"] = EtherealLightFonts.BodySemiBold,
            ["BodyBold"] = EtherealLightFonts.BodyBold,
            ["Accent"] = EtherealLightFonts.Accent,
            ["AccentBold"] = EtherealLightFonts.AccentBold,
            ["Mono"] = EtherealLightFonts.Mono
        };
    }

    private static Dictionary<string, string> GetFuturisticDreamFonts()
    {
        return new Dictionary<string, string>
        {
            ["Display"] = FuturisticDreamFonts.Display,
            ["DisplayBold"] = FuturisticDreamFonts.DisplayBold,
            ["Body"] = FuturisticDreamFonts.Body,
            ["BodySemiBold"] = FuturisticDreamFonts.BodySemiBold,
            ["BodyBold"] = FuturisticDreamFonts.BodyBold,
            ["Accent"] = FuturisticDreamFonts.Accent,
            ["AccentBold"] = FuturisticDreamFonts.AccentBold,
            ["Mono"] = FuturisticDreamFonts.Mono
        };
    }
}
