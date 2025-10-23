# Dream Alchemist - Game Design Document (GDD)

---

## 1. Core Concept

**Dream Alchemist** is a narrative-driven economic simulation game inspired by *Drug Wars*, *Potion Craft*, and *Cultist Simulator*. The player becomes an underground dealer of dreams, traveling through surreal cities to buy, synthesize, and sell abstract dream ingredients for profit and power. Each decision shapes the player’s reputation, relationships, and access to the Dream Black Market.

---

## 2. Core Gameplay Loop

### 1. Travel

* Move between surreal mental districts (e.g., *Somnia*, *Neuro Harbor*, *Oblivion Alley*).
* Each district has its own economy, events, and dream supply/demand patterns.
* Travel consumes in-game time, which progresses the global dream market.

### 2. Buy/Sell

* Purchase dream ingredients at varying prices.
* Sell them in different districts for profit.
* Prices fluctuate daily and are affected by global or local dream events.

### 3. Synthesize Dreams

* Combine 2–3 ingredients in your lab to create crafted dreams.
* Recipes are either *known* (stable) or *experimental* (randomized discovery).
* Crafted dreams can be:

  * Sold to collectors (high profit)
  * Consumed for buffs (e.g., avoid raids, unlock dream recipes)
  * Used to influence city markets or trigger story events

### 4. Events

* Random and scripted events occur during travel or trade.
* Examples:

  * *Dream Police Raid*: Lose illegal inventory if caught.
  * *Lucidity Surge*: All calming ingredients triple in value.
  * *Dream Storm*: Randomizes all prices globally.
  * *Benefactor Offer*: Receive rare recipe at a cost.

---

## 3. Systems Overview

| System         | Description                                                                | Notes                                            |
| -------------- | -------------------------------------------------------------------------- | ------------------------------------------------ |
| **Economy**    | Supply/demand simulation driven by daily ticks and events.                 | Influenced by player sales and travel.           |
| **Crafting**   | Alchemy-like mixing of 2–3 ingredients with defined or discovered recipes. | Output rarity scales exponentially with synergy. |
| **Reputation** | Determines access to black markets and secret recipes.                     | Split into *Trust*, *Infamy*, *Lucidity*.        |
| **Inventory**  | Limited space; each ingredient has a dream-weight.                         | Weight tied to intensity or rarity.              |
| **Time**       | Days pass with travel or synthesis.                                        | Each day triggers new events and market updates. |
| **Story Arcs** | Hidden narrative threads triggered by rare combinations or actions.        | Optional deep lore layer.                        |

---

## 4. World & Aesthetic

### Setting

A fragmented dreamworld that merges memory, imagination, and reality. Players navigate between floating cities connected by REM trains, visiting markets like the *Hall of Forgotten Smiles* or *The Neon Mind Bazaar*.

### Art Direction

* Dreamlike surrealism with vaporwave/neon elements.
* 2D layered backgrounds with fog and shifting color gradients rendered using SkiaSharp or MAUI Graphics.
* UI styled as a lucid alchemist’s interface – glass jars, glowing sigils, shimmering price charts.

### Music & Sound

* Lo-fi ambient with evolving layers.
* Ethereal soundscapes that change with emotion of current city.

---

## 5. Example Gameplay Moment

> You arrive in **Somnia Terminal**. Rumor spreads that *Nightmare Seeds* are illegal this week. You sell your stash of *Echo Dust* for 12,000 LucidCoins. In your lab, you mix *Childhood Memory Dust* + *Rainstorm Essence* + *Echo of Loss*. You discover **The Forgotten Playground** – a melancholic dream desired by the Collector of Shadows. Selling it triples your profits, but your Infamy rises.

---

## 6. Player Progression

| Tier | Title               | Unlocks                                  |
| ---- | ------------------- | ---------------------------------------- |
| 1    | Novice Peddler      | Basic trade, 3 dream ingredients         |
| 2    | Dream Artisan       | Advanced recipes, black market access    |
| 3    | Dream Broker        | Manage apprentices, open shopfronts      |
| 4    | Dream Cartel Leader | Smuggling routes, passive income systems |
| 5    | Lucid Architect     | Can shape markets and rewrite dream laws |

---

## 7. Key Data Schemas (Example)

### Ingredient Object

```json
{
  "id": "echo_dust",
  "name": "Echo Dust",
  "rarity": 2,
  "tags": ["memory", "sound", "melancholy"],
  "base_value": 120,
  "volatile": false
}
```

### Recipe Object

```json
{
  "id": "forgotten_playground",
  "ingredients": ["childhood_memory_dust", "rainstorm_essence", "echo_of_loss"],
  "rarity": 5,
  "alignment": "melancholic",
  "value_multiplier": 4.2,
  "discovered": false
}
```

### City Object

```json
{
  "id": "somnia_terminal",
  "base_modifiers": {
    "memory": 1.2,
    "fear": 0.8,
    "joy": 0.9
  },
  "event_pool": ["dream_police_raid", "lucidity_surge"]
}
```

---

## 8. Market Simulation Formula (Example)

For each ingredient *i* per city *c*:

```
price_i,c = base_value_i × rarity_modifier_i × city_modifier_c × event_multiplier × noise(0.9–1.1)
```

Periodic updates each in-game day cause price fluctuations, modulated by:

* Global events (±50%)
* Player trade volume (±10%)
* Random noise (+/- 5%)

---

## 9. UI Mock Ideas

* **Main Screen:** Dream map with 5 travel destinations, each showing economic indicators (XAML-based layout optimized for mobile touch).
* **Market Screen:** CollectionView with ingredient cards showing dynamic prices and visual rarity cues.
* **Synthesis Lab:** Touch-friendly drag-and-drop interface using gesture recognizers for combining ingredients.
* **Event Pop-ups:** Modal pages with illustrated cards and short narrative blurbs.
* **HUD:** ContentView displaying time, LucidCoins, inventory weight, reputation bars (responsive to Android screen sizes).

---

## 10. Technical Implementation

| Component                 | Notes                                                                           |
| ------------------------- | ------------------------------------------------------------------------------- |
| Framework                 | .NET MAUI (.NET 8+) targeting Android (API 21+)                                 |
| Architecture              | MVVM pattern with CommunityToolkit.Mvvm for data binding and commands           |
| UI Framework              | XAML for declarative UI, with custom controls for unique dream aesthetics       |
| Graphics                  | SkiaSharp for custom 2D rendering (gradients, effects, particles)               |
| Save System               | SQLite-net-pcl for local database + Preferences API for settings                |
| Cloud Sync (Optional)     | Firebase or Azure Mobile Apps for cross-device save synchronization             |
| Procedural Systems        | Weighted randomization for prices, recipes, and events using System.Random      |
| Data Models               | C# classes/records with JSON serialization for game data                        |
| Animations                | MAUI Animations API + SkiaSharp animations for custom effects                   |
| Localization              | .NET resource files (.resx) with MAUI localization support                      |
| Audio                     | Plugin.Maui.Audio for ambient music and sound effects                           |
| Dependency Injection      | Built-in MAUI DI container for services and ViewModels                          |
| Android Permissions       | INTERNET, WRITE_EXTERNAL_STORAGE (for saves), VIBRATE (for haptic feedback)    |
| Performance               | Ahead-of-time (AOT) compilation for optimal Android performance                 |
| AI Integration (Optional) | HttpClient + REST API calls to GPT for dream/event generation                   |

---

## 11. Monetization Strategy

* **Free-to-Play (Google Play):**

  * Cosmetic lab upgrades via in-app purchases
  * Optional ad for market forecast (using Google AdMob)
  * Paid "Oracle" hints for discovering rare dream recipes
  * Rewarded video ads for bonus LucidCoins
* **Premium Version:** One-time in-app purchase, ad-free, story mode unlocked, exclusive dream ingredients.

**Android Considerations:**
* Implement Google Play Billing for in-app purchases
* AdMob integration for advertisements
* Comply with Google Play policies regarding loot boxes and randomized rewards
* Clear disclosure of in-app purchase pricing

---

## 12. Android Development Notes

### Device Support
* **Minimum SDK:** Android 5.0 (API 21)
* **Target SDK:** Android 14+ (API 34+)
* **Screen Sizes:** Optimized for phones (small to extra-large), tablet support optional
* **Orientation:** Portrait-primary with landscape support for synthesis lab

### Key Android Features
* **Touch Gestures:** Tap, long-press, swipe, and drag gestures for intuitive mobile gameplay
* **Haptic Feedback:** Vibration for critical events (raids, discoveries, sales)
* **Notifications:** Local notifications for time-based events and market changes
* **Battery Optimization:** Respect Android Doze mode, minimal background processing
* **Storage:** App-private storage for saves, respect scoped storage requirements

### Testing & Deployment
* **Emulator:** Android Emulator with various API levels and screen sizes
* **Physical Testing:** Test on multiple device manufacturers (Samsung, Google Pixel, etc.)
* **Google Play Console:** App signing, beta testing tracks, staged rollouts
* **Performance Profiling:** Monitor memory usage, frame rates, and battery consumption

### Build Configuration
```xml
<!-- Sample csproj configuration -->
<TargetFrameworks>net8.0-android</TargetFrameworks>
<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
<AndroidEnableProfiledAot>true</AndroidEnableProfiledAot>
<AndroidPackageFormat>apk</AndroidPackageFormat>
```

---

## 13. Expansion Ideas

* Multiplayer black market (player-to-player trade via Google Play Games Services)
* AI-generated dream cards and visualizations
* Seasonal events (e.g., Dream Eclipse where ingredients invert behavior)
* Android widget for quick market price checks
* Wear OS companion app for notifications and quick actions

---

## 14. Development Mode & Diagnostics

### Running in Development Mode

**Dream Alchemist** supports two build configurations:

1. **DEBUG Mode** (Development)
   - System Diagnostics page available
   - Debug logging enabled
   - Comprehensive test suite accessible
   - Intended for developers and testing

2. **RELEASE Mode** (Production)
   - All diagnostic features hidden
   - Optimized performance
   - No debug overhead
   - For end-users and deployment

### Switching Between Modes

#### Visual Studio 2022
1. Use the configuration dropdown in the toolbar
2. Select **Debug** or **Release**
3. Build and run the project

#### Visual Studio Code
1. In `launch.json`, set the configuration
2. Use F5 to build with selected configuration

#### Command Line
```bash
# Build in DEBUG mode (includes diagnostics)
dotnet build -c Debug

# Build in RELEASE mode (production)
dotnet build -c Release

# Run in DEBUG mode
dotnet run -c Debug --project src/DreamAlchemist/DreamAlchemist.csproj

# Create production APK
dotnet publish -c Release -f net9.0-android
```

### Accessing System Diagnostics (DEBUG Only)

When running in **DEBUG mode**:
1. Launch the app
2. On the main page, scroll down
3. Tap the red **🔧 System Diagnostics** button
4. Tap **▶ Run Full Diagnostic Suite**

The diagnostics will test:
- ✅ Database connectivity and data integrity
- ✅ All service initializations
- ✅ Game state management
- ✅ Market price calculations
- ✅ Crafting system logic
- ✅ Travel and city unlock systems
- ✅ Event system functionality
- ✅ Data relationships (recipes, ingredients, cities)
- ✅ Performance metrics (query speed, memory usage)

### Understanding Diagnostic Output

Results are formatted as:
- **✅ PASS** - Test succeeded
- **❌ FAIL** - Critical issue detected
- **⚠️ WARN** - Non-critical warning

Example output:
```
═══════════════════════════════════════════════
     DREAM ALCHEMIST SYSTEM DIAGNOSTICS
═══════════════════════════════════════════════
Run at: 2025-10-22 17:39:00
Platform: Android
Version: 14.0

┌─────────────────────────────────────────────┐
│           DATABASE TESTS                    │
└─────────────────────────────────────────────┘
✅ PASS: Database Connection
✅ PASS: Ingredients Loading
         15 ingredients loaded
✅ PASS: Recipes Loading
         20 recipes loaded
✅ PASS: Cities Loading
         5 cities loaded

...

Summary: 45 passed, 0 failed, 2 warnings
Overall Status: ✅ ALL SYSTEMS OPERATIONAL
```

### Build Configurations in Code

The app uses C# preprocessor directives to conditionally compile diagnostic code:

```csharp
#if DEBUG
    // Diagnostic code only included in DEBUG builds
    builder.Services.AddTransient<DiagnosticViewModel>();
    builder.Services.AddTransient<DiagnosticPage>();
#endif
```

This ensures:
- Zero diagnostic overhead in production builds
- Complete removal of debug UI elements in RELEASE mode
- Smaller APK size for production deployment
- No accidental diagnostic exposure to end users

### Production Deployment Checklist

Before deploying to Google Play:
- [ ] Build in **RELEASE** configuration
- [ ] Verify diagnostic button is not visible
- [ ] Test on physical Android devices
- [ ] Run performance profiling
- [ ] Enable Android APK signing
- [ ] Update version numbers in manifest
- [ ] Review Google Play policies compliance

---

## 15. Summary

**Dream Alchemist** is an immersive trading and crafting simulator with surreal aesthetics and economic tension. It blends the satisfying profit-chasing of *Drug Wars* with the creativity of *alchemy games*, wrapped in an evocative dream economy.

---
