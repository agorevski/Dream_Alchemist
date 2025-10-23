# Font System Limitations in Dream Alchemist

## Current Status: ⚠️ Dynamic Font Switching Not Supported

While the **color theme system works perfectly** with real-time switching between themes, **font switching has a known limitation** in .NET MAUI.

## Why Fonts Don't Switch Dynamically

**Technical Limitation:**
- MAUI's `FontFamily` property cannot use `{DynamicResource}` bindings like colors can
- Font resources must be `{StaticResource}` which are resolved at compile-time
- Changing font resources at runtime doesn't trigger UI updates

**What This Means:**
- ✅ **Color themes switch instantly** (Vibrant Energy, Ethereal Dream, Mystical Jewel)
- ❌ **Font themes do NOT switch at runtime** (picker is non-functional)

## Current Fonts in Use

The app currently uses the **Classic Mystical** font scheme:
- **Headings:** Cinzel (elegant serif)
- **Body Text:** Outfit (clean sans-serif)
- **Accent:** Philosopher (stylized serif)
- **Numbers:** JetBrains Mono (monospace)

These fonts are already installed and working - they just can't be changed at runtime.

## Workaround: Manual Font Selection

If you want to use a different font scheme, you can manually edit the app before building:

### Option 1: Change Default Fonts in Colors.xaml

Edit `src/DreamAlchemist/Resources/Styles/Colors.xaml`:

**For Classic Mystical (Current Default):**
```xml
<x:String x:Key="FontDisplay">Cinzel</x:String>
<x:String x:Key="FontDisplayBold">CinzelBold</x:String>
<x:String x:Key="FontBody">Outfit</x:String>
<x:String x:Key="FontAccent">Philosopher</x:String>
```

**For Ethereal Light:**
```xml
<x:String x:Key="FontDisplay">CormorantGaramond</x:String>
<x:String x:Key="FontDisplayBold">CormorantGaramondBold</x:String>
<x:String x:Key="FontBody">Outfit</x:String>
<x:String x:Key="FontAccent">PoiretOne</x:String>
```

**For Futuristic Dream:**
```xml
<x:String x:Key="FontDisplay">Audiowide</x:String>
<x:String x:Key="FontDisplayBold">Audiowide</x:String>
<x:String x:Key="FontBody">Inter</x:String>
<x:String x:Key="FontAccent">Inter</x:String>
```

After editing, rebuild the app:
```bash
cd src/DreamAlchemist
dotnet build
```

### Option 2: Remove Font Picker from DEBUG Panel

Since the font picker doesn't work, you may want to remove it from the Welcome screen to avoid confusion.

Edit `src/DreamAlchemist/Views/WelcomePage.xaml` and remove:
```xml
<!-- Font Style Picker (remove this section) -->
<Label Text="Font Style:"
       TextColor="White"
       FontSize="14"
       Margin="0,10,0,0"/>
<Picker ItemsSource="{Binding FontOptions}"
        SelectedItem="{Binding SelectedFont}"
        TextColor="White"
        BackgroundColor="#25253E"/>
```

And in `src/DreamAlchemist/ViewModels/WelcomeViewModel.cs`, remove:
```csharp
[ObservableProperty]
private string selectedFont = "Classic Mystical";

public ObservableCollection<string> FontOptions { get; } = new()
{
    "Classic Mystical",
    "Ethereal Light",
    "Futuristic Dream"
};

partial void OnSelectedFontChanged(string value) { }
private void ApplyFontSelection() { }
```

## What DOES Work

✅ **All 3 color themes work perfectly:**
- Vibrant Energy - Bright purple & cyan
- Ethereal Dream - Soft purple & teal
- Mystical Jewel - Deep purple & blue

✅ **Real-time color switching:**
- Change theme in DEBUG panel
- Colors update instantly across all pages
- No rebuild required

✅ **All fonts are installed and working:**
- They're just fixed at app start, not dynamically switchable

## Future Solutions

Possible approaches for dynamic font switching in future versions:

1. **Rebuild Required Approach:**
   - Font picker saves preference
   - App restarts with new fonts
   - Requires app restart to see changes

2. **Platform-Specific Rendering:**
   - Custom renderers for Android
   - Manually update font at native level
   - Complex implementation

3. **Wait for MAUI Update:**
   - Microsoft may add dynamic font support
   - Monitor .NET MAUI roadmap

## Recommendation

**For Version 1.0:**
- Keep the beautiful Classic Mystical fonts (Cinzel + Outfit)
- Remove the non-functional font picker to avoid confusion
- Document that font customization requires manual editing
- Focus on the excellent color theme system which works perfectly

**For Future Versions:**
- Implement restart-based font switching
- Or wait for MAUI framework improvements

## Bottom Line

The color theme system is **fully functional and impressive**. The font system has a technical limitation that's beyond our control in the current .NET MAUI framework. The fonts we've chosen (Classic Mystical scheme) look great and fit the dream/mystical aesthetic perfectly.
