# Font Setup Guide for Dream Alchemist

This guide explains how to download and install the custom fonts for the theme system.

## Required Fonts

### 1. Cinzel (Classic Mystical - Display)
- **Source:** Google Fonts
- **URL:** https://fonts.google.com/specimen/Cinzel
- **Weights Needed:** Regular (400), Bold (700)
- **Files:**
  - `Cinzel-Regular.ttf`
  - `Cinzel-Bold.ttf`

### 2. Cormorant Garamond (Ethereal Light - Display)
- **Source:** Google Fonts
- **URL:** https://fonts.google.com/specimen/Cormorant+Garamond
- **Weights Needed:** Regular (400), Bold (700)
- **Files:**
  - `CormorantGaramond-Regular.ttf`
  - `CormorantGaramond-Bold.ttf`

### 3. Audiowide (Futuristic Dream - Display)
- **Source:** Google Fonts
- **URL:** https://fonts.google.com/specimen/Audiowide
- **Weights Needed:** Regular (400)
- **Files:**
  - `Audiowide-Regular.ttf`

### 4. Outfit (Body Font for all schemes)
- **Source:** Google Fonts
- **URL:** https://fonts.google.com/specimen/Outfit
- **Weights Needed:** Regular (400), SemiBold (600), Bold (700)
- **Files:**
  - `Outfit-Regular.ttf`
  - `Outfit-SemiBold.ttf`
  - `Outfit-Bold.ttf`

### 5. Inter (Futuristic Dream - Body)
- **Source:** Google Fonts
- **URL:** https://fonts.google.com/specimen/Inter
- **Weights Needed:** Regular (400), SemiBold (600), Bold (700)
- **Files:**
  - `Inter-Regular.ttf`
  - `Inter-SemiBold.ttf`
  - `Inter-Bold.ttf`

### 6. Philosopher (Accent Font)
- **Source:** Google Fonts
- **URL:** https://fonts.google.com/specimen/Philosopher
- **Weights Needed:** Regular (400), Bold (700)
- **Files:**
  - `Philosopher-Regular.ttf`
  - `Philosopher-Bold.ttf`

### 7. Poiret One (Ethereal Light - Accent)
- **Source:** Google Fonts
- **URL:** https://fonts.google.com/specimen/Poiret+One
- **Weights Needed:** Regular (400)
- **Files:**
  - `PoiretOne-Regular.ttf`

### 8. JetBrains Mono (Monospace/Technical)
- **Source:** Google Fonts
- **URL:** https://fonts.google.com/specimen/JetBrains+Mono
- **Weights Needed:** Regular (400)
- **Files:**
  - `JetBrainsMono-Regular.ttf`

## Installation Steps

### Step 1: Download Fonts

1. Visit each Google Fonts URL listed above
2. Click the "Download family" button
3. Extract the downloaded ZIP files
4. Locate the TTF files (in the `static` folder if present)

### Step 2: Add to Project

1. Copy all the TTF files to: `src/DreamAlchemist/Resources/Fonts/`
2. The final structure should look like:
   ```
   src/DreamAlchemist/Resources/Fonts/
   ├── OpenSans-Regular.ttf (existing)
   ├── OpenSans-Semibold.ttf (existing)
   ├── Cinzel-Regular.ttf (existing)
   ├── Cinzel-Bold.ttf (existing)
   ├── CormorantGaramond-Regular.ttf (existing)
   ├── CormorantGaramond-Bold.ttf (existing)
   ├── Audiowide-Regular.ttf (existing)
   ├── Outfit-Regular.ttf (existing)
   ├── Outfit-SemiBold.ttf (existing)
   ├── Outfit-Bold.ttf (existing)
   ├── Inter-Regular.ttf (existing)
   ├── Inter-SemiBold.ttf (existing)
   ├── Inter-Bold.ttf (existing)
   ├── Philosopher-Regular.ttf (existing)
   ├── Philosopher-Bold.ttf (existing)
   ├── PoiretOne-Regular.ttf (existing)
   └── JetBrainsMono-Regular.ttf (existing)
   ```

### Step 3: Register Fonts in MauiProgram.cs

The fonts are automatically registered in `MauiProgram.cs`. The registration code should look like this:

```csharp
.ConfigureFonts(fonts =>
{
    // Existing fonts
    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    
    // Classic Mystical fonts
    fonts.AddFont("Cinzel-Regular.ttf", "Cinzel");
    fonts.AddFont("Cinzel-Bold.ttf", "CinzelBold");
    fonts.AddFont("Philosopher-Regular.ttf", "Philosopher");
    fonts.AddFont("Philosopher-Bold.ttf", "PhilosopherBold");
    
    // Ethereal Light fonts
    fonts.AddFont("CormorantGaramond-Regular.ttf", "CormorantGaramond");
    fonts.AddFont("CormorantGaramond-Bold.ttf", "CormorantGaramondBold");
    fonts.AddFont("PoiretOne-Regular.ttf", "PoiretOne");
    
    // Futuristic Dream fonts
    fonts.AddFont("Audiowide-Regular.ttf", "Audiowide");
    fonts.AddFont("Inter-Regular.ttf", "Inter");
    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
    fonts.AddFont("Inter-Bold.ttf", "InterBold");
    
    // Body fonts (shared)
    fonts.AddFont("Outfit-Regular.ttf", "Outfit");
    fonts.AddFont("Outfit-SemiBold.ttf", "OutfitSemiBold");
    fonts.AddFont("Outfit-Bold.ttf", "OutfitBold");
    
    // Monospace
    fonts.AddFont("JetBrainsMono-Regular.ttf", "JetBrainsMono");
});
```

## Alternative: Quick Download Script

If you have Python installed, you can use this script to download all fonts:

```python
import requests
import zipfile
import io
import os

fonts = {
    "Cinzel": "https://fonts.google.com/download?family=Cinzel",
    "Cormorant_Garamond": "https://fonts.google.com/download?family=Cormorant%20Garamond",
    "Audiowide": "https://fonts.google.com/download?family=Audiowide",
    "Outfit": "https://fonts.google.com/download?family=Outfit",
    "Inter": "https://fonts.google.com/download?family=Inter",
    "Philosopher": "https://fonts.google.com/download?family=Philosopher",
    "Poiret_One": "https://fonts.google.com/download?family=Poiret%20One",
    "JetBrains_Mono": "https://fonts.google.com/download?family=JetBrains%20Mono"
}

output_dir = "src/DreamAlchemist/Resources/Fonts/"
os.makedirs(output_dir, exist_ok=True)

for name, url in fonts.items():
    print(f"Downloading {name}...")
    # Note: Actual download requires handling Google Fonts API
    # This is a simplified example
```

## Verification

After adding fonts, build the project and check for:

1. No build errors related to fonts
2. Fonts appear in the app when theme switcher is used
3. Text renders correctly with new fonts

## Troubleshooting

### Font Not Appearing
- Ensure TTF file is in `Resources/Fonts/` directory
- Verify font is registered in `MauiProgram.cs`
- Check font name matches exactly (case-sensitive)
- Clean and rebuild the project

### Build Errors
- Ensure all font files have correct file extension (`.ttf`)
- Verify no duplicate font registrations
- Check that font files aren't corrupted

### Performance Issues
- Fonts are cached after first load
- No performance impact after initial loading

## License Information

All fonts listed are available under SIL Open Font License (OFL) from Google Fonts, which allows:
- Free commercial use
- Modification and redistribution
- Embedding in applications

Always verify current license terms at the Google Fonts page for each font.
