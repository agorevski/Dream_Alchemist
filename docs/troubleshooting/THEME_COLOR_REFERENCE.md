# Theme Color Reference - Visual Verification Guide

This document shows the exact hex color values for each theme so you can verify they are applying correctly.

## Theme A: Vibrant Energy (Default)

### Primary Colors
- **Primary:** `#A855F7` - Electric Purple (Bright)
- **PrimaryLight:** `#C084FC` - Lighter Purple
- **PrimaryDark:** `#7E22CE` - Deeper Purple

### Secondary Colors
- **Secondary:** `#06B6D4` - Cyber Cyan (Bright Teal/Blue)
- **SecondaryLight:** `#22D3EE` - Light Cyan
- **SecondaryDark:** `#0891B2` - Deep Cyan

### Backgrounds
- **BackgroundPrimary:** `#0A0E27` - Very Dark Blue (Almost Black)
- **BackgroundSecondary:** `#1A1F3A` - Dark Space Blue
- **BackgroundTertiary:** `#252B4D` - Medium Dark Blue
- **BackgroundElevated:** `#303660` - Elevated Surface (Lighter Blue)

## Theme B: Ethereal Dream

### Primary Colors
- **Primary:** `#C4B5FD` - Lavender (Much Lighter/Pastel)
- **PrimaryLight:** `#DDD6FE` - Very Light Lavender
- **PrimaryDark:** `#A78BFA` - Medium Lavender

### Secondary Colors
- **Secondary:** `#A7F3D0` - Mint Dream (Soft Green-Teal)
- **SecondaryLight:** `#D1FAE5` - Pale Mint
- **SecondaryDark:** `#6EE7B7` - Deep Mint

### Backgrounds
- **BackgroundPrimary:** `#1F1B2E` - Misty Twilight (Purple-ish Dark)
- **BackgroundSecondary:** `#2D2838` - Soft Purple
- **BackgroundTertiary:** `#3A3545` - Medium Purple-Gray
- **BackgroundElevated:** `#474252` - Light Purple-Gray

## Theme C: Mystical Jewel

### Primary Colors
- **Primary:** `#7C3AED` - Royal Amethyst (Deep Purple)
- **PrimaryLight:** `#9333EA` - Bright Amethyst
- **PrimaryDark:** `#5B21B6` - Very Deep Purple

### Secondary Colors
- **Secondary:** `#1E40AF` - Sapphire (Deep Blue, not cyan)
- **SecondaryLight:** `#2563EB` - Bright Sapphire
- **SecondaryDark:** `#1E3A8A` - Very Deep Sapphire

### Backgrounds
- **BackgroundPrimary:** `#0F0A1F` - Onyx (Very Dark Purple-Black)
- **BackgroundSecondary:** `#1A1433` - Deep Purple
- **BackgroundTertiary:** `#251E3D` - Medium Purple
- **BackgroundElevated:** `#302847` - Light Purple

---

## Key Differences to Look For

### Title "Dream Alchemist" (Primary Color)
- **Vibrant Energy:** Bright electric purple `#A855F7`
- **Ethereal Dream:** Soft pastel lavender `#C4B5FD` (MUCH lighter)
- **Mystical Jewel:** Deep royal amethyst `#7C3AED` (darker than Vibrant)

### Continue Button (Secondary Color)
- **Vibrant Energy:** Cyan/teal `#06B6D4` 
- **Ethereal Dream:** Mint green `#A7F3D0` (green-ish)
- **Mystical Jewel:** Deep blue `#1E40AF` (BLUE not cyan)

### Page Background (BackgroundPrimary)
- **Vibrant Energy:** Dark space blue `#0A0E27`
- **Ethereal Dream:** Purple twilight `#1F1B2E` (more purple)
- **Mystical Jewel:** Onyx black-purple `#0F0A1F` (very dark purple)

### Frame Background (BackgroundSecondary)
- **Vibrant Energy:** Space blue `#1A1F3A`
- **Ethereal Dream:** Soft purple `#2D2838` (brownish-purple)
- **Mystical Jewel:** Deep purple `#1A1433` (rich purple)

---

## Debugging Steps

If themes look the same:

1. **Check Debug Output** - Look for these lines in Visual Studio Output window:
   ```
   === Applying Theme: VibrantEnergy ===
   Primary Color: #A855F7
   Secondary Color: #06B6D4
   
   === Applying Theme: MysticalJewel ===
   Primary Color: #7C3AED
   Secondary Color: #1E40AF
   ```

2. **Verify Resources Are Updating**
   - Debug output should show "Updated ThemePrimary = #A855F7" etc.
   - If you see "Added" instead of "Updated", colors might not be binding

3. **Check XAML Binding**
   - Elements should use `{DynamicResource ThemePrimary}`
   - NOT `{StaticResource ThemePrimary}`
   - NOT hardcoded hex colors

4. **Test on Physical Device**
   - Emulator sometimes caches resources
   - Try on real Android device

5. **Rebuild**
   ```bash
   dotnet clean
   dotnet build
   ```

---

## Visual Comparison

When switching themes, you should see:

**Vibrant Energy → Ethereal Dream:**
- Title becomes MUCH LIGHTER (bright purple → soft lavender)
- Continue button changes from CYAN to MINT GREEN
- Overall feel: High energy → Soft dreamy

**Vibrant Energy → Mystical Jewel:**
- Title becomes DARKER (bright purple → deep royal purple)
- Continue button changes from CYAN to DEEP BLUE
- Overall feel: Bright energetic → Rich mysterious

**Ethereal Dream → Mystical Jewel:**
- Everything gets MUCH DARKER
- Pastels → Rich jewel tones
- Soft/calming → Deep/luxurious

---

## Font Differences

**NOTE:** Fonts will only show differences if you've downloaded the font files.

Without fonts downloaded:
- All will look like OpenSans (default fallback)

With fonts downloaded:
- **Classic Mystical:** Cinzel (elegant serif headers)
- **Ethereal Light:** Cormorant Garamond (literary serif)  
- **Futuristic Dream:** Audiowide (wide futuristic)

Font differences are most visible in the "Dream Alchemist" title.

---

## Still Not Working?

Check `Colors.xaml` - ensure colors use `DynamicResource`:
```xml
<!-- CORRECT -->
BackgroundColor="{DynamicResource ThemePrimary}"

<!-- WRONG -->
BackgroundColor="{StaticResource ThemePrimary}"
BackgroundColor="#A855F7"
```

Run app, check Debug Output for theme application logs.
