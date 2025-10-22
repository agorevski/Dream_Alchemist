# 01 - Project Setup

## Overview

This document covers the initial .NET MAUI project creation, configuration, and dependency setup for Dream Alchemist.

## Prerequisites

### Required Software
- **Visual Studio 2022** (17.8+) or **Visual Studio Code** with C# Dev Kit
- **.NET 8.0 SDK** or later
- **Android SDK** (API 21 minimum, API 34 target)
- **Android Emulator** or physical Android device for testing

### Verify Installation
```bash
dotnet --version                    # Should show 8.0.x or higher
dotnet workload list                # Should show maui-android installed
```

### Install MAUI Workload (if needed)
```bash
dotnet workload install maui-android
```

## Project Creation

### Step 1: Create MAUI Project

```bash
cd c:\GIT\agorevski\DreamAlchemist2
dotnet new maui -n DreamAlchemist -o src/DreamAlchemist
```

This creates the basic MAUI project structure with Android support.

### Step 2: Configure Project File

Edit `src/DreamAlchemist/DreamAlchemist.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net8.0-android</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>DreamAlchemist</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <!-- Display name -->
    <ApplicationTitle>Dream Alchemist</ApplicationTitle>

    <!-- App Identifier -->
    <ApplicationId>com.dreamalchemist.game</ApplicationId>
    <ApplicationIdGuid>YOUR-GUID-HERE</ApplicationIdGuid>

    <!-- Versions -->
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <ApplicationVersion>1</ApplicationVersion>

    <!-- Android Specific -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
    <AndroidEnableProfiledAot>true</AndroidEnableProfiledAot>
    <AndroidPackageFormat>apk</AndroidPackageFormat>
    <AndroidSigningKeyStore></AndroidSigningKeyStore>
    <AndroidSigningKeyAlias></AndroidSigningKeyAlias>
    <AndroidSigningKeyPass></AndroidSigningKeyPass>
    <AndroidSigningStorePass></AndroidSigningStorePass>
  </PropertyGroup>

  <ItemGroup>
    <!-- App Icon -->
    <MauiIcon Include="Resources\AppIcon\appicon.svg" ForegroundFile="Resources\AppIcon\appiconfg.svg" Color="#512BD4" />

    <!-- Splash Screen -->
    <MauiSplashScreen Include="Resources\Splash\splash.svg" Color="#512BD4" BaseSize="128,128" />

    <!-- Images -->
    <MauiImage Include="Resources\Images\*" />

    <!-- Fonts -->
    <MauiFont Include="Resources\Fonts\*" />

    <!-- Raw Assets -->
    <MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
  </ItemGroup>

  <ItemGroup>
    <!-- Core Dependencies -->
    <PackageReference Include="Microsoft.Maui.Controls" Version="8.0.90" />
    <PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="8.0.90" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="8.0.0" />
    
    <!-- MVVM Toolkit -->
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    
    <!-- Database -->
    <PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
    <PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.8" />
    
    <!-- Graphics -->
    <PackageReference Include="SkiaSharp.Views.Maui.Controls" Version="2.88.7" />
    <PackageReference Include="SkiaSharp.Extended.UI.Maui" Version="2.0.0" />
    
    <!-- JSON -->
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    
    <!-- Audio -->
    <PackageReference Include="Plugin.Maui.Audio" Version="3.0.0" />
    
    <!-- HTTP Client for AI -->
    <PackageReference Include="Azure.AI.OpenAI" Version="1.0.0-beta.17" />
    
    <!-- Monetization (Add later in Phase 7) -->
    <!-- <PackageReference Include="Xamarin.Google.Android.Play.Billing" Version="6.2.1" /> -->
    <!-- <PackageReference Include="Xamarin.GooglePlayServices.Ads" Version="23.0.0" /> -->
  </ItemGroup>

</Project>
```

### Step 3: Create Folder Structure

```bash
cd src/DreamAlchemist
mkdir Models
mkdir ViewModels
mkdir Views
mkdir Services
mkdir Data
mkdir Helpers
mkdir Resources/Audio
mkdir Resources/Data
```

Folder purposes:
- **Models/** - Data models and entities
- **ViewModels/** - MVVM ViewModels
- **Views/** - XAML pages and controls
- **Services/** - Business logic services
- **Data/** - Database context and repositories
- **Helpers/** - Utility classes and extensions
- **Resources/Audio/** - Sound effects and music
- **Resources/Data/** - JSON seed data files

## Android Configuration

### Step 4: Configure AndroidManifest.xml

Create/edit `src/DreamAlchemist/Platforms/Android/AndroidManifest.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application 
        android:allowBackup="true" 
        android:icon="@mipmap/appicon" 
        android:roundIcon="@mipmap/appicon_round"
        android:supportsRtl="true"
        android:label="Dream Alchemist"
        android:theme="@style/Maui.SplashTheme">
    </application>
    
    <!-- Permissions -->
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.VIBRATE" />
    
    <!-- Optional: For cloud saves in future -->
    <!-- <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" 
                         android:maxSdkVersion="32" /> -->
</manifest>
```

### Step 5: Configure MainActivity

Edit `src/DreamAlchemist/Platforms/Android/MainActivity.cs`:

```csharp
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace DreamAlchemist;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | 
                          ConfigChanges.Orientation | 
                          ConfigChanges.UiMode | 
                          ConfigChanges.ScreenLayout | 
                          ConfigChanges.SmallestScreenSize | 
                          ConfigChanges.Density,
    ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Request permissions if needed
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            RequestPermissions(new[] { 
                Android.Manifest.Permission.PostNotifications 
            }, 0);
        }
    }
}
```

## Dependency Injection Setup

### Step 6: Configure MauiProgram.cs

Edit `src/DreamAlchemist/MauiProgram.cs`:

```csharp
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Plugin.Maui.Audio;

namespace DreamAlchemist;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services (will be implemented in Phase 1)
        RegisterServices(builder.Services);
        
        // Register ViewModels
        RegisterViewModels(builder.Services);
        
        // Register Views
        RegisterViews(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Core Services
        services.AddSingleton<IGameStateService, GameStateService>();
        services.AddSingleton<ISaveService, SaveService>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        
        // Game Logic Services
        services.AddSingleton<IMarketService, MarketService>();
        services.AddSingleton<ICraftingService, CraftingService>();
        services.AddSingleton<IInventoryService, InventoryService>();
        services.AddSingleton<IEventService, EventService>();
        services.AddSingleton<IReputationService, ReputationService>();
        services.AddSingleton<ITravelService, TravelService>();
        
        // AI Service
        services.AddSingleton<IAIService, AIService>();
        
        // Audio Service
        services.AddSingleton(AudioManager.Current);
        services.AddSingleton<IAudioService, AudioService>();
        
        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<MainViewModel>();
        services.AddTransient<MarketViewModel>();
        services.AddTransient<LabViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<TravelViewModel>();
        services.AddTransient<EventViewModel>();
        services.AddTransient<SettingsViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddTransient<MainPage>();
        services.AddTransient<MarketPage>();
        services.AddTransient<LabPage>();
        services.AddTransient<InventoryPage>();
        services.AddTransient<TravelPage>();
        services.AddTransient<EventPage>();
        services.AddTransient<SettingsPage>();
    }
}
```

## App.xaml Configuration

### Step 7: Configure App.xaml

Edit `src/DreamAlchemist/App.xaml`:

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="DreamAlchemist.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>
            
            <!-- Dream Alchemist Color Palette -->
            <Color x:Key="DreamPurple">#8B5CF6</Color>
            <Color x:Key="DreamPink">#EC4899</Color>
            <Color x:Key="DreamCyan">#06B6D4</Color>
            <Color x:Key="DreamYellow">#FCD34D</Color>
            <Color x:Key="DarkBackground">#0F0F1E</Color>
            <Color x:Key="DarkSurface">#1A1A2E</Color>
            <Color x:Key="DarkCard">#25253E</Color>
            
            <!-- Gradients -->
            <LinearGradientBrush x:Key="DreamGradient" StartPoint="0,0" EndPoint="1,1">
                <GradientStop Color="{StaticResource DreamPurple}" Offset="0.0" />
                <GradientStop Color="{StaticResource DreamPink}" Offset="1.0" />
            </LinearGradientBrush>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### Step 8: Configure App.xaml.cs

Edit `src/DreamAlchemist/App.xaml.cs`:

```csharp
namespace DreamAlchemist;

public partial class App : Application
{
    private readonly IGameStateService _gameStateService;
    private readonly ISaveService _saveService;

    public App(IGameStateService gameStateService, ISaveService saveService)
    {
        InitializeComponent();
        
        _gameStateService = gameStateService;
        _saveService = saveService;
        
        // Set up the app
        InitializeApp();
        
        MainPage = new AppShell();
    }

    private async void InitializeApp()
    {
        // Load game state if exists
        await _saveService.LoadGameAsync();
        
        // Initialize game systems
        await _gameStateService.InitializeAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        
        // Set window properties for Android
        window.Title = "Dream Alchemist";
        
        return window;
    }
}
```

## Git Configuration

### Step 9: Create .gitignore

Create `.gitignore` in project root:

```gitignore
## .NET Core
bin/
obj/
*.user
*.suo
*.cache
*.dll
*.exe

## Visual Studio
.vs/
*.vsidx
*.vssscc

## MAUI
**/Android/bin/
**/Android/obj/
**/iOS/bin/
**/iOS/obj/

## Rider
.idea/

## User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

## Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
x64/
x86/
build/
bld/
[Bb]in/
[Oo]bj/

## NuGet
*.nupkg
**/packages/*
!**/packages/build/

## Android
*.apk
*.aab
*.dex
*.ap_
*.keystore

## Sensitive data
appsettings.local.json
*.pfx
secrets.json

## Logs
*.log
```

## Verification Steps

### Step 10: Build and Test

```bash
cd src/DreamAlchemist

# Restore packages
dotnet restore

# Build project
dotnet build

# Run on Android emulator
dotnet build -t:Run -f net9.0-android
```

### Expected Output
- Project builds without errors
- All NuGet packages restore successfully
- App launches on Android emulator
- Default MAUI page displays

## Troubleshooting

### Common Issues

**Issue:** MAUI workload not found
```bash
# Solution:
dotnet workload install maui-android
```

**Issue:** Android SDK not found
```bash
# Solution: Set ANDROID_HOME environment variable
# Windows: Control Panel > System > Advanced > Environment Variables
# Add: ANDROID_HOME = C:\Program Files (x86)\Android\android-sdk
```

**Issue:** Build fails with "SkiaSharp not found"
```bash
# Solution:
dotnet clean
dotnet restore
dotnet build
```

**Issue:** Emulator won't start
- Open Android SDK Manager
- Install Android Emulator
- Create AVD with API 34
- Enable hardware acceleration in BIOS

## Next Steps

Once project setup is complete:
1. Verify build succeeds
2. Test on emulator
3. Proceed to **02-architecture.md** for MVVM structure
4. Begin implementing data models in **03-data-models.md**

## Checklist

- [ ] .NET 8 SDK installed
- [ ] MAUI workload installed
- [ ] Android SDK configured
- [ ] Project created
- [ ] Dependencies added to .csproj
- [ ] Folder structure created
- [ ] AndroidManifest.xml configured
- [ ] MauiProgram.cs configured
- [ ] App.xaml resources defined
- [ ] .gitignore created
- [ ] Project builds successfully
- [ ] App runs on emulator
