# Dream Alchemist - Implementation Plan

**Project:** Dream Alchemist - Android Game  
**Framework:** .NET MAUI (.NET 8+)  
**Target Platform:** Android (API 21+)  
**Architecture:** MVVM with CommunityToolkit.Mvvm  
**Developer:** Solo (AI-Assisted Development)  
**Scope:** Full feature set with AI integration

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Technical Architecture](#technical-architecture)
3. [Development Phases](#development-phases)
4. [File Structure](#file-structure)
5. [Data Models](#data-models)
6. [Implementation Roadmap](#implementation-roadmap)
7. [Testing Strategy](#testing-strategy)
8. [Deployment Checklist](#deployment-checklist)

---

## Project Overview

### Objectives
- Implement complete Dream Alchemist game as per GDD
- AI-powered dream generation using GPT API
- Procedural visual generation using SkiaSharp
- Full economic simulation with dynamic markets
- Android-optimized touch controls
- In-app purchases and ad monetization
- Comprehensive save system with cloud sync option

### Success Criteria
- Smooth 60 FPS gameplay on mid-range Android devices
- Complete core gameplay loop (travel, trade, synthesize)
- All 5 player progression tiers functional
- Event system with 20+ unique events
- 50+ dream ingredients with procedural recipes
- Monetization integration (IAP + ads)
- Successful Google Play deployment

---

## Technical Architecture

### Core Technology Stack

```
.NET MAUI (.NET 8+)
├── UI Layer (XAML + SkiaSharp)
│   ├── Views (Pages/ContentViews)
│   ├── Custom Controls
│   └── Animations
├── ViewModel Layer (MVVM)
│   ├── ViewModels (CommunityToolkit.Mvvm)
│   ├── Commands (RelayCommand)
│   └── Observable Properties
├── Business Logic Layer
│   ├── Services (Game Logic)
│   ├── Managers (State Management)
│   └── Engines (Economy, Events, AI)
├── Data Layer
│   ├── Models (Domain Objects)
│   ├── DTOs (Data Transfer)
│   └── Repositories
└── Infrastructure Layer
    ├── Database (SQLite)
    ├── Networking (HttpClient)
    ├── Storage (Preferences API)
    └── Platform Services
```

### Key Dependencies

```xml
<!-- Core MAUI -->
<PackageReference Include="Microsoft.Maui.Controls" Version="8.0.0" />
<PackageReference Include="Microsoft.Maui.Graphics" Version="8.0.0" />

<!-- MVVM Toolkit -->
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />

<!-- Database -->
<PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.8" />

<!-- Graphics -->
<PackageReference Include="SkiaSharp" Version="2.88.7" />
<PackageReference Include="SkiaSharp.Views.Maui.Controls" Version="2.88.7" />

<!-- Audio -->
<PackageReference Include="Plugin.Maui.Audio" Version="2.1.0" />

<!-- HTTP Client -->
<PackageReference Include="System.Net.Http.Json" Version="8.0.0" />

<!-- JSON Serialization -->
<PackageReference Include="System.Text.Json" Version="8.0.0" />

<!-- Monetization -->
<PackageReference Include="Xamarin.Google.Android.Play.Billing" Version="6.2.0" />
<!-- AdMob will be configured via Android native bindings -->

<!-- Optional Cloud Sync -->
<PackageReference Include="FirebaseDatabase.net" Version="4.2.0" />
```

### Architecture Patterns

1. **MVVM Pattern**
   - Separation of UI and business logic
   - Data binding for reactive UI updates
   - Commands for user interactions

2. **Repository Pattern**
   - Abstract data access
   - Swappable data sources (SQLite, cloud)
   - Unit testable data operations

3. **Service Locator / Dependency Injection**
   - MAUI built-in DI container
   - Singleton services for game state
   - Scoped ViewModels

4. **Observer Pattern**
   - Event aggregator for cross-component communication
   - Market price updates
   - Reputation changes

5. **Strategy Pattern**
   - Different pricing algorithms per city
   - Event resolution strategies
   - Recipe discovery mechanisms

---

## Development Phases

### Phase 0: Project Setup & Foundation (Week 1)

**Objective:** Bootstrap .NET MAUI project with proper structure and dependencies

#### Tasks:
1. Create new .NET MAUI project targeting Android
2. Configure Android manifest and permissions
3. Set up project folder structure
4. Install all NuGet dependencies
5. Configure dependency injection container
6. Set up basic navigation shell
7. Create base classes (ViewModelBase, PageBase)
8. Implement app theme and resource dictionaries
9. Configure debug/release build configurations
10. Set up version control ignore patterns

#### Deliverables:
- Functional .NET MAUI Android project
- Project structure established
- All dependencies installed
- Basic app shell with navigation
- Theme resources configured

#### Files to Create:
```
DreamAlchemist/
├── MauiProgram.cs (DI configuration)
├── App.xaml (App resources)
├── App.xaml.cs (App initialization)
├── AppShell.xaml (Navigation shell)
├── AppShell.xaml.cs
├── Platforms/Android/
│   ├── AndroidManifest.xml
│   └── MainActivity.cs
└── Resources/
    ├── Styles/
    │   └── Colors.xaml
    └── Fonts/
```

#### Testing:
- [ ] Project builds successfully
- [ ] App launches on Android emulator
- [ ] Navigation shell displays
- [ ] No dependency errors

---

### Phase 1: Core Data Models & Infrastructure (Week 1-2)

**Objective:** Implement all game data models, database schema, and core services

#### Tasks:

##### 1.1 Data Models
Create all domain models matching GDD schemas:

**Models/Ingredient.cs**
```csharp
public class Ingredient
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Rarity { get; set; }
    public List<string> Tags { get; set; }
    public int BaseValue { get; set; }
    public bool IsVolatile { get; set; }
    public float DreamWeight { get; set; }
    public string Description { get; set; }
}
```

**Models/Recipe.cs**
```csharp
public class Recipe
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> IngredientIds { get; set; }
    public int Rarity { get; set; }
    public string Alignment { get; set; } // melancholic, euphoric, etc.
    public float ValueMultiplier { get; set; }
    public bool IsDiscovered { get; set; }
    public string ResultDescription { get; set; }
}
```

**Models/City.cs**
```csharp
public class City
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, float> BaseModifiers { get; set; }
    public List<string> EventPool { get; set; }
    public int TravelCost { get; set; }
    public int TravelDays { get; set; }
}
```

**Models/MarketPrice.cs**
```csharp
public class MarketPrice
{
    public string IngredientId { get; set; }
    public string CityId { get; set; }
    public int CurrentPrice { get; set; }
    public int Supply { get; set; }
    public int Demand { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

**Models/GameEvent.cs**
```csharp
public class GameEvent
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public EventType Type { get; set; }
    public Dictionary<string, object> Effects { get; set; }
    public int Duration { get; set; }
    public float Probability { get; set; }
}
```

**Models/PlayerState.cs**
```csharp
public class PlayerState
{
    public string PlayerId { get; set; }
    public int LucidCoins { get; set; }
    public int CurrentTier { get; set; }
    public string CurrentCityId { get; set; }
    public int CurrentDay { get; set; }
    public float TrustReputation { get; set; }
    public float InfamyReputation { get; set; }
    public float LucidityReputation { get; set; }
    public int InventoryCapacity { get; set; }
    public DateTime LastPlayed { get; set; }
}
```

**Models/InventoryItem.cs**
```csharp
public class InventoryItem
{
    public string InventoryItemId { get; set; }
    public string PlayerId { get; set; }
    public string IngredientId { get; set; }
    public int Quantity { get; set; }
    public int PurchasePrice { get; set; }
    public string PurchaseCityId { get; set; }
}
```

##### 1.2 Database Layer
Implement SQLite database with repositories:

**Data/DreamAlchemistDatabase.cs**
- Initialize SQLite connection
- Create tables for all models
- Implement migrations

**Data/Repositories/**
- IIngredientRepository & IngredientRepository
- IRecipeRepository & RecipeRepository
- ICityRepository & CityRepository
- IMarketPriceRepository & MarketPriceRepository
- IPlayerStateRepository & PlayerStateRepository
- IInventoryRepository & InventoryRepository
- IGameEventRepository & GameEventRepository

##### 1.3 Core Services

**Services/SaveGameService.cs**
- Save/Load player state
- Auto-save functionality
- Cloud sync preparation (Firebase)

**Services/GameStateManager.cs**
- Manage current game session
- Track player state changes
- Notify observers of state changes

**Services/TimeManager.cs**
- Track in-game days
- Progress time on travel/synthesis
- Trigger daily market updates

**Services/DataSeedService.cs**
- Seed initial ingredients (50+)
- Seed cities (5+)
- Seed base recipes
- Seed event definitions

#### Deliverables:
- Complete data model classes
- SQLite database implementation
- All repository interfaces and implementations
- Core service classes
- Seed data for testing

#### Files to Create:
```
Models/
├── Ingredient.cs
├── Recipe.cs
├── City.cs
├── MarketPrice.cs
├── GameEvent.cs
├── PlayerState.cs
├── InventoryItem.cs
├── Enums.cs
└── DTOs/ (if needed)

Data/
├── DreamAlchemistDatabase.cs
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
└── SeedData/
    ├── IngredientsData.json
    ├── CitiesData.json
    └── RecipesData.json

Services/
├── SaveGameService.cs
├── GameStateManager.cs
├── TimeManager.cs
└── DataSeedService.cs
```

#### Testing:
- [ ] All models serialize/deserialize correctly
- [ ] Database creates tables successfully
- [ ] CRUD operations work for all repositories
- [ ] Seed data populates correctly
- [ ] Save/load game state works
- [ ] Time progression updates correctly

---

### Phase 2: Economy & Market System (Week 2-3)

**Objective:** Implement dynamic market pricing and economic simulation

#### Tasks:

##### 2.1 Market Price Engine

**Services/MarketEngine.cs**
- Implement price calculation formula:
  ```
  price = base_value × rarity_mod × city_mod × event_mult × noise(0.9-1.1)
  ```
- Track supply/demand per city
- Apply global and local events
- Simulate player impact on markets

##### 2.2 Economic Simulation

**Services/EconomySimulator.cs**
- Daily market tick system
- Price fluctuation algorithms
- Supply/demand balancing
- Historical price tracking (optional charts)

##### 2.3 City Economics

**Services/CityEconomyManager.cs**
- Per-city modifier application
- City-specific event triggers
- Black market access calculation based on reputation

##### 2.4 Market Data Providers

**Services/IMarketDataProvider.cs**
- Current prices per city
- Price history (7-day trends)
- Supply/demand indicators
- Market forecasts (premium feature)

#### Deliverables:
- Functional market price engine
- Daily economic simulation
- Per-city pricing variations
- Price history tracking

#### Files to Create:
```
Services/Economy/
├── MarketEngine.cs
├── EconomySimulator.cs
├── CityEconomyManager.cs
├── IMarketDataProvider.cs
└── MarketDataProvider.cs

Models/Economy/
├── PriceHistory.cs
├── MarketTrend.cs
└── EconomicEvent.cs
```

#### Testing:
- [ ] Prices update daily correctly
- [ ] City modifiers apply properly
- [ ] Events affect prices as expected
- [ ] Supply/demand impacts pricing
- [ ] Player trades influence market
- [ ] Price history records accurately
- [ ] No negative or overflow prices

---

### Phase 3: Inventory & Trading System (Week 3-4)

**Objective:** Implement buying, selling, and inventory management

#### Tasks:

##### 3.1 Inventory Management

**Services/InventoryManager.cs**
- Add/remove items
- Check capacity (weight-based)
- Validate inventory space
- Calculate total weight

**ViewModels/InventoryViewModel.cs**
- Observable collection of inventory items
- Sorting/filtering options
- Item selection for sale/synthesis

##### 3.2 Trading System

**Services/TradingService.cs**
- Buy ingredient validation
- Sell ingredient with profit calculation
- Bulk trading operations
- Transaction history

**ViewModels/MarketViewModel.cs**
- Display available ingredients per city
- Show current prices with trends
- Buy/sell commands
- Profit/loss calculations

##### 3.3 Transaction Processing

**Services/TransactionProcessor.cs**
- Validate trades (sufficient funds, space)
- Update player coins
- Update inventory
- Record transaction
- Update market supply/demand

#### Deliverables:
- Complete inventory system with weight limits
- Buy/sell functionality
- Transaction validation and processing
- Transaction history

#### Files to Create:
```
Services/Trading/
├── InventoryManager.cs
├── TradingService.cs
└── TransactionProcessor.cs

ViewModels/
├── InventoryViewModel.cs
└── MarketViewModel.cs

Models/
└── Transaction.cs
```

#### Testing:
- [ ] Can buy ingredients with sufficient funds
- [ ] Cannot buy with insufficient funds
- [ ] Cannot buy if inventory full
- [ ] Selling adds correct amount of coins
- [ ] Inventory weight calculates correctly
- [ ] Prices update after large trades
- [ ] Transaction history records properly

---

### Phase 4: Dream Synthesis System (Week 4-5)

**Objective:** Implement alchemy-style dream crafting with AI integration

#### Tasks:

##### 4.1 Crafting Engine

**Services/SynthesisEngine.cs**
- Recipe matching (2-3 ingredients)
- Known recipe execution
- Experimental synthesis (discovery)
- Output generation with rarity calculation

**Services/RecipeDiscoveryService.cs**
- Generate new recipes procedurally
- Tag-based synergy calculation
- Recipe unlock system
- Recipe book management

##### 4.2 AI Integration for Dreams

**Services/AIService.cs**
- HttpClient wrapper for GPT API
- Generate dream descriptions
- Generate dream names
- Generate event narratives
- Rate limiting and caching

**Configuration:**
```csharp
public class AIConfiguration
{
    public string ApiKey { get; set; }
    public string Endpoint { get; set; }
    public string Model { get; set; } = "gpt-4-turbo-preview";
    public int MaxTokens { get; set; } = 150;
}
```

**Prompts:**
- Dream Description: "Generate a surreal dream description combining: {ingredients}. Style: dreamlike, poetic, 2-3 sentences."
- Event Narrative: "Create a short game event about: {context}. Style: mysterious, engaging, 1-2 sentences."

##### 4.3 Synthesis UI Logic

**ViewModels/SynthesisLabViewModel.cs**
- Ingredient selection (drag & drop state)
- Synthesis command
- Result display
- Recipe discovery notification

##### 4.4 Dream Effects

**Services/DreamEffectService.cs**
- Apply dream buffs to player
- Temporary modifiers (avoid raids, price bonuses)
- Story event triggers from special dreams

#### Deliverables:
- Complete synthesis system
- AI-generated dream content
- Recipe discovery mechanics
- Dream consumption effects

#### Files to Create:
```
Services/Synthesis/
├── SynthesisEngine.cs
├── RecipeDiscoveryService.cs
├── DreamEffectService.cs
└── AIService.cs

ViewModels/
└── SynthesisLabViewModel.cs

Models/
├── SynthesisResult.cs
└── DreamEffect.cs

Configuration/
└── AIConfiguration.cs
```

#### Testing:
- [ ] Known recipes produce correct results
- [ ] Experimental synthesis discovers new recipes
- [ ] AI generates appropriate descriptions
- [ ] Recipe book updates on discovery
- [ ] Dream effects apply correctly
- [ ] Synthesis consumes ingredients
- [ ] Time progresses on synthesis
- [ ] AI service handles rate limits gracefully

---

### Phase 5: World & Travel System (Week 5-6)

**Objective:** Implement city navigation and travel mechanics

#### Tasks:

##### 5.1 World Map System

**Services/WorldMapService.cs**
- City discovery system
- Travel route calculation
- Travel cost/time calculation
- Unlock new cities based on reputation

**ViewModels/WorldMapViewModel.cs**
- Display available cities
- Show economic indicators per city
- Travel command with confirmation
- Visual city connections

##### 5.2 Travel Mechanics

**Services/TravelService.cs**
- Initiate travel between cities
- Progress time during travel
- Trigger travel events
- Update player location
- Consume travel costs

##### 5.3 City Information

**ViewModels/CityDetailViewModel.cs**
- City description and lore
- Current market summary
- Active events in city
- Reputation requirements

#### Deliverables:
- Functional world map
- Travel between cities
- Time progression on travel
- City unlocking system

#### Files to Create:
```
Services/World/
├── WorldMapService.cs
└── TravelService.cs

ViewModels/
├── WorldMapViewModel.cs
└── CityDetailViewModel.cs

Models/
└── TravelRoute.cs
```

#### Testing:
- [ ] Can travel between unlocked cities
- [ ] Cannot travel to locked cities
- [ ] Travel consumes correct time/cost
- [ ] Time progression triggers market updates
- [ ] Player location updates correctly
- [ ] Travel events can trigger
- [ ] City indicators show accurate data

---

### Phase 6: Events System (Week 6-7)

**Objective:** Implement random and scripted game events

#### Tasks:

##### 6.1 Event Engine

**Services/EventEngine.cs**
- Event probability calculation
- Event triggering system
- Event pool management per city
- Global vs local events

**Services/EventResolver.cs**
- Apply event effects
- Modify market prices
- Modify player state
- Trigger story arcs

##### 6.2 Event Types Implementation

Events to implement (20+ total):

**Market Events:**
- Lucidity Surge (triple calming ingredients)
- Dream Storm (randomize all prices)
- Black Market Raid (lose illegal items)
- Supply Shortage (specific ingredient scarce)
- Demand Spike (specific ingredient valuable)

**Player Events:**
- Dream Police Raid (chance to lose inventory)
- Benefactor Offer (rare recipe at cost)
- Collector Request (high-value sale opportunity)
- Apprentice Encounter (unlock shop assistant)
- Memory Theft (lose random ingredients)

**Story Events:**
- Mysterious Stranger (story arc trigger)
- Dream Architect Challenge
- Forbidden Recipe Discovery
- Rival Dealer Encounter
- Underground Network Invitation

**Reputation Events:**
- Trust Gain (successful trade milestone)
- Infamy Gain (illegal synthesis)
- Lucidity Breakthrough (recipe mastery)

##### 6.3 Event UI

**ViewModels/EventViewModel.cs**
- Display event notification
- Present choices (if applicable)
- Show consequences
- Update game state

#### Deliverables:
- Event triggering system
- 20+ unique events implemented
- Event resolution logic
- Event notification UI logic

#### Files to Create:
```
Services/Events/
├── EventEngine.cs
├── EventResolver.cs
└── EventFactory.cs

Models/Events/
├── EventDefinition.cs
├── EventInstance.cs
└── EventChoice.cs

ViewModels/
└── EventViewModel.cs

Data/SeedData/
└── EventsData.json
```

#### Testing:
- [ ] Events trigger with correct probability
- [ ] Event effects apply correctly
- [ ] Market events modify prices
- [ ] Player events modify state
- [ ] Story events can chain
- [ ] Event choices have consequences
- [ ] Events respect city context
- [ ] No duplicate simultaneous events

---

### Phase 7: UI/UX Implementation (Week 7-10)

**Objective:** Create all game screens with procedural visuals and animations

#### Tasks:

##### 7.1 Core UI Pages

**Views/MainMenuPage.xaml**
- New Game / Continue
- Settings
- Credits
- Exit

**Views/WorldMapPage.xaml**
- City nodes with visual indicators
- Connection paths
- Current location highlight
- Travel button
- Economic summary overlay

**Views/MarketPage.xaml**
- Ingredient list (CollectionView)
- Price displays with trend arrows
- Buy/Sell buttons
- Inventory quick view
- Filter/sort controls

**Views/InventoryPage.xaml**
- Item cards with visuals
- Sorting options (rarity, weight, value)
- Total weight indicator
- Sell mode toggle
- Item details popup

**Views/SynthesisLabPage.xaml**
- 3 ingredient slots (drag targets)
- Ingredient picker
- Synthesize button
- Result display area
- Recipe book button
- Visual mixing animation

**Views/PlayerStatsPage.xaml**
- Current tier and progress
- Reputation bars (Trust, Infamy, Lucidity)
- Statistics (trades, syntheses, discoveries)
- Achievements
- Total wealth

**Views/RecipeBookPage.xaml**
- Discovered recipes list
- Recipe details view
- Required ingredients
- Expected output
- Discovery hints for undiscovered

**Views/SettingsPage.xaml**
- Audio settings
- Graphics quality
- Notifications toggle
- Cloud sync
- Account management

##### 7.2 SkiaSharp Procedural Graphics

**Controls/DreamVisualization.cs** (SKCanvasView)
- Procedural dream ingredient visuals
- Gradient backgrounds based on tags
- Particle effects for rarity
- Animated glow effects
- Color palettes per dream type

**Graphics/ProceduralGenerator.cs**
- Generate ingredient visuals from properties
- Color schemes based on tags:
  - Memory: Blue-purple gradients
  - Fear: Red-black gradients
  - Joy: Yellow-orange gradients
  - Melancholy: Gray-blue gradients
- Particle systems for effects

**Graphics/AnimationEngine.cs**
- Synthesis mixing animation
- Price change indicators
- Travel transitions
- Event popup effects
- Reputation bar animations

##### 7.3 Custom Controls

**Controls/PriceDisplayControl.xaml**
- Current price
- Trend arrow (up/down/stable)
- Percentage change
- History mini-chart

**Controls/IngredientCard.xaml**
- Procedural visual preview
- Name and rarity stars
- Tags display
- Weight indicator
- Quick action buttons

**Controls/ReputationBar.xaml**
- Animated progress bar
- Color gradient based on level
- Tooltip with details

**Controls/CityNode.xaml**
- City icon/visual
- Economic indicator colors
- Active event badge
- Locked/unlocked state

##### 7.4 Animations & Transitions

**Animations to implement:**
- Page transitions (slide, fade)
- Button press effects (scale, glow)
- Price update animations (number roll)
- Synthesis mixing (swirl, blend)
- Event popup (modal slide-in)
- Achievement unlock (celebration)
- Reputation gain/loss (bar fill)

**Animations/TransitionService.cs**
- Reusable animation definitions
- Timing functions
- Animation sequencing

##### 7.5 Responsive Design

- Support Android phones (4" to 7")
- Portrait-primary layout
- Landscape support for synthesis lab
- Touch-optimized hit targets (44dp minimum)
- Gesture recognizers for swipe/drag

#### Deliverables:
- All 8+ major UI pages
- Procedural graphics engine
- Custom controls library
- Animation system
- Responsive layouts

#### Files to Create:
```
Views/
├── MainMenuPage.xaml[.cs]
├── WorldMapPage.xaml[.cs]
├── MarketPage.xaml[.cs]
├── InventoryPage.xaml[.cs]
├── SynthesisLabPage.xaml[.cs]
├── PlayerStatsPage.xaml[.cs]
├── RecipeBookPage.xaml[.cs]
└── SettingsPage.xaml[.cs]

Controls/
├── DreamVisualization.cs
├── PriceDisplayControl.xaml[.cs]
├── IngredientCard.xaml[.cs]
├── ReputationBar.xaml[.cs]
└── CityNode.xaml[.cs]

Graphics/
├── ProceduralGenerator.cs
├── AnimationEngine.cs
├── ColorPalettes.cs
└── ParticleSystem.cs

Animations/
└── TransitionService.cs

Resources/Styles/
├── Styles.xaml
└── Colors.xaml
```

#### Testing:
- [ ] All pages render correctly
- [ ] Navigation works between pages
- [ ] Touch targets are appropriately sized
- [ ] Animations run smoothly (60 FPS)
- [ ] Procedural graphics generate correctly
- [ ] Custom controls display properly
- [ ] Responsive on different screen sizes
- [ ] Portrait and landscape work
- [ ] Gestures recognized correctly
- [ ] No UI freezing during operations

---

### Phase 8: Progression System (Week 10-11)

**Objective:** Implement player tiers, unlocks, and achievements

#### Tasks:

##### 8.1 Player Tier System

**Services/ProgressionService.cs**
- Track player tier (1-5)
- Calculate tier progress
- Unlock features per tier
- Tier advancement logic

**Tier Requirements:**
```
Tier 1: Novice Peddler (Start)
- 3 basic ingredients accessible
- Can trade in 2 cities

Tier 2: Dream Artisan (10k coins, 5 discoveries)
- Advanced recipes unlock
- Black market access
- 5 cities accessible

Tier 3: Dream Broker (50k coins, 20 discoveries, 1000 Trust)
- Can hire apprentice
- Open shopfront (passive income)
- All cities accessible

Tier 4: Dream Cartel Leader (200k coins, 50 discoveries, 2000 Infamy)
- Smuggling routes (reduced raid chance)
- Passive income systems
- Influence city events

Tier 5: Lucid Architect (1M coins, 100 discoveries, 3000 Lucidity)
- Shape markets directly
- Rewrite dream laws
- Access to ultimate recipes
```

##### 8.2 Unlock System

**Services/UnlockManager.cs**
- Track unlocked features
- Validate unlock conditions
- Notify player of new unlocks

##### 8.3 Achievement System

**Services/AchievementService.cs**
- Track achievement progress
- Unlock achievements
- Reward system (coins, recipes)

**Achievements (30+ total):**
- First Trade
- First Synthesis
- First Discovery
- Market Master (100 trades)
- Dream Artisan (50 syntheses)
- Recipe Hunter (all recipes discovered)
- Wealthy Dealer (100k coins)
- Reputation milestones
- City-specific achievements
- Event-based achievements

##### 8.4 Passive Income Systems (Tier 3+)

**Services/PassiveIncomeManager.cs**
- Shopfront income calculation
- Apprentice work simulation
- Daily passive earnings
- Upgrade system

#### Deliverables:
- Complete tier progression system
- Feature unlocking based on tiers
- Achievement tracking
- Passive income for high tiers

#### Files to Create:
```
Services/Progression/
├── ProgressionService.cs
├── UnlockManager.cs
├── AchievementService.cs
└── PassiveIncomeManager.cs

Models/
├── PlayerTier.cs
├── Achievement.cs
└── PassiveIncome.cs

ViewModels/
└── AchievementsViewModel.cs

Data/SeedData/
└── AchievementsData.json
```

#### Testing:
- [ ] Tier progression calculates correctly
- [ ] Unlocks trigger at right thresholds
- [ ] Features locked/unlocked properly
- [ ] Achievements track progress
- [ ] Achievement rewards granted
- [ ] Passive income accrues correctly
- [ ] Shopfront/apprentice systems work
- [ ] Tier benefits apply correctly

---

### Phase 9: Monetization Implementation (Week 11-12)

**Objective:** Integrate in-app purchases and advertisement systems

#### Tasks:

##### 9.1 Google Play Billing

**Services/BillingService.cs**
- Initialize billing client
- Query products
- Purchase flow
- Verify purchases
- Restore purchases

**In-App Purchases:**
1. **Remove Ads** - $2.99 (permanent)
2. **Premium Pack** - $4.99 (one-time)
   - Ad-free
   - Story mode unlocked
   - Exclusive ingredients (10)
   - Recipe hints
   - Cloud sync
3. **LucidCoin Packs:**
   - Small: 10k coins - $0.99
   - Medium: 50k coins - $3.99
   - Large: 150k coins - $9.99
   - Mega: 500k coins - $24.99
4. **Oracle Hints** - $0.99 (consumable)
   - Reveals 3 random recipes
5. **Cosmetic Lab Skins:**
   - Neon Dreams - $1.99
   - Vaporwave Aesthetic - $1.99
   - Dark Matter - $1.99

##### 9.2 AdMob Integration

**Platform/Android/AdMobService.cs**
- Initialize AdMob SDK
- Load interstitial ads
- Load rewarded video ads
- Show ads at appropriate times

**Ad Placement Strategy:**
- Interstitial: After 5 trades or syntheses
- Rewarded video: Market forecast (24hr bonus)
- Rewarded video: Double synthesis output
- Rewarded video: Bonus coins (5k)

##### 9.3 Purchase Validation

**Services/PurchaseValidator.cs**
- Verify Google Play receipt
- Prevent purchase fraud
- Track purchase history

##### 9.4 Premium Features

**Services/PremiumFeatureService.cs**
- Check premium status
- Enable/disable features
- Handle restoration

#### Deliverables:
- Google Play Billing integration
- AdMob ad serving
- Premium feature gating
- Purchase restoration

#### Files to Create:
```
Services/Monetization/
├── BillingService.cs
├── PurchaseValidator.cs
└── PremiumFeatureService.cs

Platforms/Android/
└── AdMobService.cs

Models/
└── Product.cs
```

#### Testing:
- [ ] Billing client initializes
- [ ] Products load correctly
- [ ] Purchase flow completes
- [ ] Purchases verified successfully
- [ ] Restore purchases works
- [ ] Premium features unlock
- [ ] Ads display appropriately
- [ ] Rewarded video grants rewards
- [ ] No ads shown if premium purchased
- [ ] Test with Google Play test users

---

### Phase 10: Polish & Optimization (Week 12-14)

**Objective:** Performance tuning, bug fixes, and quality improvements

#### Tasks:

##### 10.1 Performance Optimization

**Profiling:**
- Use .NET MAUI profiler
- Identify memory leaks
- Optimize database queries
- Reduce allocations

**Optimizations:**
- Implement object pooling for visuals
- Lazy load images and graphics
- Optimize CollectionView rendering
- Reduce JSON parsing overhead
- Cache expensive calculations
- Compress save files

**Android-Specific:**
- Enable AOT compilation
- Optimize APK size (ProGuard/R8)
- Reduce startup time
- Implement proper lifecycle handling
- Optimize battery usage

##### 10.2 Audio System

**Services/AudioService.cs**
- Background music player
- Sound effect player
- Volume controls
- Music crossfading

**Audio Assets:**
- 3-5 ambient music tracks (lo-fi)
- 20+ sound effects:
  - UI interactions (click, swipe)
  - Trade sounds (coins)
  - Synthesis sounds (mixing, success)
  - Event sounds (alert, notification)
  - Ambient city sounds

**Note:** Use royalty-free audio or generate with AI tools

##### 10.3 Haptic Feedback

**Services/HapticService.cs**
- Light feedback (button press)
- Medium feedback (success)
- Heavy feedback (important event)

**Haptic Events:**
- Trade completion
- Synthesis success
- Recipe discovery
- Event trigger
- Tier advancement
- Achievement unlock

##### 10.4 Notifications

**Services/NotificationService.cs**
- Schedule local notifications
- Market change alerts
- Time-based reminders
- Event notifications

**Notification Types:**
- "Market prices updated!" (daily)
- "Special event in [City]!" (on event)
- "Your shop earned [X] coins!" (tier 3+)
- "Come back tomorrow for new deals!"

##### 10.5 Error Handling & Logging

**Services/ErrorReportingService.cs**
- Catch and log exceptions
- User-friendly error messages
- Crash reporting (optional: AppCenter)

##### 10.6 Accessibility

- Screen reader support
- Font scaling
- High contrast mode
- Colorblind-friendly palettes
- Touch target sizing

##### 10.7 Localization Preparation

- Extract all strings to resources
- Prepare for future translations
- Date/number formatting

#### Deliverables:
- Optimized performance (60 FPS target)
- Complete audio system
- Haptic feedback
- Local notifications
- Error handling
- Accessibility features

#### Files to Create:
```
Services/
├── AudioService.cs
├── HapticService.cs
├── NotificationService.cs
└── ErrorReportingService.cs

Resources/
├── Audio/
│   ├── Music/
│   └── SFX/
└── Strings/
    └── AppResources.resx

Utilities/
├── Logger.cs
└── ErrorHandler.cs
```

#### Testing:
- [ ] App runs at 60 FPS on mid-range device
- [ ] No memory leaks detected
- [ ] Startup time < 3 seconds
- [ ] APK size reasonable (< 100 MB)
- [ ] Audio plays correctly
- [ ] Haptics trigger appropriately
- [ ] Notifications appear on schedule
- [ ] Errors don't crash app
- [ ] Accessibility features work
- [ ] Battery usage acceptable

---

### Phase 11: Testing & QA (Week 14-16)

**Objective:** Comprehensive testing across devices and scenarios

#### Tasks:

##### 11.1 Unit Testing

**Test Coverage:**
- MarketEngine price calculations
- InventoryManager weight calculations
- SynthesisEngine recipe matching
- EconomySimulator price fluctuations
- ReputationService calculations
- SaveGameService serialization

**Testing Framework:**
- xUnit or NUnit
- Moq for mocking
- FluentAssertions

##### 11.2 Integration Testing

**Test Scenarios:**
- Complete trade flow (buy → sell)
- Complete synthesis flow (gather → mix → sell)
- Travel and time progression
- Event triggering and resolution
- Tier progression
- Save and load game state
- Purchase flow (test environment)

##### 11.3 Device Testing

**Test Devices:**
- Android 5.0 (API 21) - minimum
- Android 8.0 (API 26) - common
- Android 12+ (API 31+) - modern
- Various screen sizes (4" to 7")
- Different manufacturers (Samsung, Pixel, etc.)
- Low-end and high-end devices

**Test Matrix:**
- Performance (FPS, loading times)
- UI rendering
- Touch responsiveness
- Battery consumption
- Storage usage
- Network connectivity
- Different locales

##### 11.4 User Acceptance Testing

**Beta Testing:**
- Internal testing (1-2 weeks)
- Closed beta via Google Play (2-4 weeks)
- Gather feedback
- Iterate on issues

**Feedback Collection:**
- In-app feedback form
- Beta tester surveys
- Analytics tracking (optional)

##### 11.5 Bug Fixing

**Bug Tracking:**
- Create issue tracker (GitHub Issues)
- Prioritize bugs (critical, major, minor)
- Fix and verify
- Regression testing

#### Deliverables:
- Unit test suite (80%+ coverage)
- Integration tests
- Device compatibility matrix
- Beta feedback report
- Bug-free stable build

#### Testing Checklist:
- [ ] All unit tests pass
- [ ] Integration tests pass
- [ ] No crashes on supported devices
- [ ] UI renders correctly on all screen sizes
- [ ] Performance meets targets
- [ ] Battery usage acceptable
- [ ] No data loss issues
- [ ] Save/load works reliably
- [ ] IAP works in production
- [ ] Ads display correctly
- [ ] Beta feedback addressed
- [ ] All critical bugs fixed

---

### Phase 12: Deployment (Week 16-17)

**Objective:** Prepare and launch on Google Play Store

#### Tasks:

##### 12.1 App Store Preparation

**Google Play Console Setup:**
- Create developer account
- Configure app listing
- Set up pricing and distribution
- Configure in-app products
- Set up AdMob account link

**App Store Assets:**
- App icon (512x512)
- Feature graphic (1024x500)
- Screenshots (4-8 per device type)
- Promotional video (optional)
- App description
- Short description
- What's new text

**Store Listing Content:**
```
Title: Dream Alchemist - Surreal Trading Game

Short Description:
Trade, craft, and master the economy of dreams in this surreal trading game.

Full Description:
Enter the Dream Black Market...

Become a dealer of dreams in this unique economic simulation game. Travel between surreal cities, buy and sell abstract dream ingredients, and synthesize powerful crafted dreams for profit and power.

Features:
• Dynamic market economy with daily price fluctuations
• Alchemy-style dream synthesis system
• AI-generated dream descriptions
• 5 surreal cities to explore
• 50+ unique dream ingredients
• Progression system with 5 player tiers
• Random events and story arcs
• Beautiful procedural graphics
• Lo-fi ambient soundtrack

Build your reputation, unlock new recipes, and rise from Novice Peddler to Lucid Architect!
```

##### 12.2 Release Configuration

**Build Settings:**
```xml
<PropertyGroup>
    <ApplicationId>com.dreamalchemist.game</ApplicationId>
    <ApplicationVersion>1</ApplicationVersion>
    <ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
    <AndroidPackageFormat>aab</AndroidPackageFormat>
    <AndroidSigningKeyStore>dreamalchemist.keystore</AndroidSigningKeyStore>
    <AndroidEnableProfiledAot>true</AndroidEnableProfiledAot>
    <RunAOTCompilation>true</RunAOTCompilation>
</PropertyGroup>
```

**Permissions:**
```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.VIBRATE" />
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
```

##### 12.3 App Signing

- Generate upload keystore
- Configure app signing in Play Console
- Enable Play App Signing

##### 12.4 Testing Track

**Internal Testing:**
- Upload to internal track
- Test with team (1-3 days)
- Verify all features work in production

**Closed Beta:**
- Create beta tester list
- Upload to closed track
- Gather feedback (1-2 weeks)
- Fix any issues

**Open Beta (Optional):**
- Release to open beta
- Monitor analytics and feedback
- Final polish

##### 12.5 Production Release

**Staged Rollout:**
- 10% rollout (day 1)
- Monitor for crashes/issues
- 25% rollout (day 3)
- 50% rollout (day 5)
- 100% rollout (day 7)

**Launch Monitoring:**
- Watch crash rates
- Monitor reviews
- Track user acquisition
- Check analytics data

##### 12.6 Post-Launch

**Maintenance Plan:**
- Monitor for critical bugs
- Prepare hotfix process
- Plan content updates
- Gather user feedback
- Track metrics (retention, monetization)

#### Deliverables:
- Signed production AAB
- Google Play listing complete
- Beta testing completed
- Production release live
- Monitoring dashboard set up

#### Deployment Checklist:
- [ ] App signed with production key
- [ ] Store listing complete with assets
- [ ] In-app products configured
- [ ] AdMob linked and tested
- [ ] Privacy policy published
- [ ] Internal testing passed
- [ ] Beta testing completed
- [ ] All feedback addressed
- [ ] Final QA passed
- [ ] Production build uploaded
- [ ] Staged rollout initiated
- [ ] Monitoring active
- [ ] Support channels ready

---

## File Structure

```
DreamAlchemist/
├── DreamAlchemist.csproj
├── MauiProgram.cs
├── App.xaml[.cs]
├── AppShell.xaml[.cs]
│
├── Models/
│   ├── Ingredient.cs
│   ├── Recipe.cs
│   ├── City.cs
│   ├── MarketPrice.cs
│   ├── GameEvent.cs
│   ├── PlayerState.cs
│   ├── InventoryItem.cs
│   ├── Transaction.cs
│   ├── Achievement.cs
│   ├── PlayerTier.cs
│   ├── SynthesisResult.cs
│   ├── DreamEffect.cs
│   └── Enums.cs
│
├── ViewModels/
│   ├── ViewModelBase.cs
│   ├── MainMenuViewModel.cs
│   ├── WorldMapViewModel.cs
│   ├── MarketViewModel.cs
│   ├── InventoryViewModel.cs
│   ├── SynthesisLabViewModel.cs
│   ├── PlayerStatsViewModel.cs
│   ├── RecipeBookViewModel.cs
│   ├── EventViewModel.cs
│   ├── AchievementsViewModel.cs
│   └── SettingsViewModel.cs
│
├── Views/
│   ├── MainMenuPage.xaml[.cs]
│   ├── WorldMapPage.xaml[.cs]
│   ├── MarketPage.xaml[.cs]
│   ├── InventoryPage.xaml[.cs]
│   ├── SynthesisLabPage.xaml[.cs]
│   ├── PlayerStatsPage.xaml[.cs]
│   ├── RecipeBookPage.xaml[.cs]
│   └── SettingsPage.xaml[.cs]
│
├── Controls/
│   ├── DreamVisualization.cs
│   ├── PriceDisplayControl.xaml[.cs]
│   ├── IngredientCard.xaml[.cs]
│   ├── ReputationBar.xaml[.cs]
│   └── CityNode.xaml[.cs]
│
├── Services/
│   ├── SaveGameService.cs
│   ├── GameStateManager.cs
│   ├── TimeManager.cs
│   ├── DataSeedService.cs
│   ├── AudioService.cs
│   ├── HapticService.cs
│   ├── NotificationService.cs
│   ├── ErrorReportingService.cs
│   │
│   ├── Economy/
│   │   ├── MarketEngine.cs
│   │   ├── EconomySimulator.cs
│   │   ├── CityEconomyManager.cs
│   │   └── MarketDataProvider.cs
│   │
│   ├── Trading/
│   │   ├── InventoryManager.cs
│   │   ├── TradingService.cs
│   │   └── TransactionProcessor.cs
│   │
│   ├── Synthesis/
│   │   ├── SynthesisEngine.cs
│   │   ├── RecipeDiscoveryService.cs
│   │   ├── DreamEffectService.cs
│   │   └── AIService.cs
│   │
│   ├── World/
│   │   ├── WorldMapService.cs
│   │   └── TravelService.cs
│   │
│   ├── Events/
│   │   ├── EventEngine.cs
│   │   ├── EventResolver.cs
│   │   └── EventFactory.cs
│   │
│   ├── Progression/
│   │   ├── ProgressionService.cs
│   │   ├── UnlockManager.cs
│   │   ├── AchievementService.cs
│   │   └── PassiveIncomeManager.cs
│   │
│   └── Monetization/
│       ├── BillingService.cs
│       ├── PurchaseValidator.cs
│       └── PremiumFeatureService.cs
│
├── Data/
│   ├── DreamAlchemistDatabase.cs
│   ├── Repositories/
│   │   ├── Interfaces/
│   │   │   ├── IIngredientRepository.cs
│   │   │   ├── IRecipeRepository.cs
│   │   │   ├── ICityRepository.cs
│   │   │   ├── IMarketPriceRepository.cs
│   │   │   ├── IPlayerStateRepository.cs
│   │   │   ├── IInventoryRepository.cs
│   │   │   └── IGameEventRepository.cs
│   │   │
│   │   └── Implementations/
│   │       ├── IngredientRepository.cs
│   │       ├── RecipeRepository.cs
│   │       ├── CityRepository.cs
│   │       ├── MarketPriceRepository.cs
│   │       ├── PlayerStateRepository.cs
│   │       ├── InventoryRepository.cs
│   │       └── GameEventRepository.cs
│   │
│   └── SeedData/
│       ├── IngredientsData.json
│       ├── CitiesData.json
│       ├── RecipesData.json
│       ├── EventsData.json
│       └── AchievementsData.json
│
├── Graphics/
│   ├── ProceduralGenerator.cs
│   ├── AnimationEngine.cs
│   ├── ColorPalettes.cs
│   └── ParticleSystem.cs
│
├── Animations/
│   └── TransitionService.cs
│
├── Configuration/
│   └── AIConfiguration.cs
│
├── Utilities/
│   ├── Logger.cs
│   └── ErrorHandler.cs
│
├── Platforms/
│   └── Android/
│       ├── AndroidManifest.xml
│       ├── MainActivity.cs
│       └── AdMobService.cs
│
└── Resources/
    ├── Styles/
    │   ├── Colors.xaml
    │   └── Styles.xaml
    ├── Fonts/
    ├── Images/
    ├── Audio/
    │   ├── Music/
    │   └── SFX/
    └── Strings/
        └── AppResources.resx
```

---

## Data Models

### Complete Model Definitions

#### Ingredient Model
```csharp
public class Ingredient
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string IngredientId { get; set; } // "echo_dust"
    public string Name { get; set; } // "Echo Dust"
    public int Rarity { get; set; } // 1-5
    public string TagsJson { get; set; } // JSON array
    public int BaseValue { get; set; } // Base price
    public bool IsVolatile { get; set; } // Affected by events more
    public float DreamWeight { get; set; } // Inventory weight
    public string Description { get; set; }
    public string VisualSeed { get; set; } // For procedural generation
    
    [Ignore]
    public List<string> Tags
    {
        get => JsonSerializer.Deserialize<List<string>>(TagsJson ?? "[]");
        set => TagsJson = JsonSerializer.Serialize(value);
    }
}
```

#### Recipe Model
```csharp
public class Recipe
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string RecipeId { get; set; }
    public string Name { get; set; }
    public string IngredientsJson { get; set; } // JSON array of ingredient IDs
    public int Rarity { get; set; }
    public string Alignment { get; set; } // melancholic, euphoric, etc.
    public float ValueMultiplier { get; set; }
    public string ResultDescription { get; set; }
    public bool IsDiscovered { get; set; }
    public string DiscoveryHint { get; set; }
    
    [Ignore]
    public List<string> IngredientIds
    {
        get => JsonSerializer.Deserialize<List<string>>(IngredientsJson ?? "[]");
        set => IngredientsJson = JsonSerializer.Serialize(value);
    }
}
```

#### City Model
```csharp
public class City
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string CityId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ModifiersJson { get; set; } // JSON dictionary
    public string EventPoolJson { get; set; } // JSON array
    public int TravelCost { get; set; }
    public int TravelDays { get; set; }
    public bool IsUnlocked { get; set; }
    public int UnlockTier { get; set; }
    
    [Ignore]
    public Dictionary<string, float> BaseModifiers
    {
        get => JsonSerializer.Deserialize<Dictionary<string, float>>(ModifiersJson ?? "{}");
        set => ModifiersJson = JsonSerializer.Serialize(value);
    }
    
    [Ignore]
    public List<string> EventPool
    {
        get => JsonSerializer.Deserialize<List<string>>(EventPoolJson ?? "[]");
        set => EventPoolJson = JsonSerializer.Serialize(value);
    }
}
```

#### MarketPrice Model
```csharp
public class MarketPrice
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string IngredientId { get; set; }
    public string CityId { get; set; }
    public int CurrentPrice { get; set; }
    public int Supply { get; set; } // 0-100
    public int Demand { get; set; } // 0-100
    public float TrendMultiplier { get; set; } // For trend arrows
    public DateTime LastUpdated { get; set; }
    
    [Ignore]
    public PriceTrend Trend => CalculateTrend();
    
    private PriceTrend CalculateTrend()
    {
        if (TrendMultiplier > 1.05f) return PriceTrend.Rising;
        if (TrendMultiplier < 0.95f) return PriceTrend.Falling;
        return PriceTrend.Stable;
    }
}

public enum PriceTrend
{
    Rising,
    Stable,
    Falling
}
```

#### PlayerState Model
```csharp
public class PlayerState
{
    [PrimaryKey]
    public string PlayerId { get; set; } = "player_1";
    
    public int LucidCoins { get; set; }
    public int CurrentTier { get; set; } = 1;
    public string CurrentCityId { get; set; }
    public int CurrentDay { get; set; } = 1;
    
    // Reputation (0-5000)
    public float TrustReputation { get; set; }
    public float InfamyReputation { get; set; }
    public float LucidityReputation { get; set; }
    
    public int InventoryCapacity { get; set; } = 100;
    public int TotalTrades { get; set; }
    public int TotalSyntheses { get; set; }
    public int TotalDiscoveries { get; set; }
    
    public DateTime LastPlayed { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public bool IsPremium { get; set; }
    public bool AdsRemoved { get; set; }
}
```

#### InventoryItem Model
```csharp
public class InventoryItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string PlayerId { get; set; }
    public string IngredientId { get; set; }
    public int Quantity { get; set; }
    public int PurchasePrice { get; set; }
    public string PurchaseCityId { get; set; }
    public DateTime PurchasedAt { get; set; }
    
    [Ignore]
    public Ingredient Ingredient { get; set; } // Loaded via join
}
```

#### GameEvent Model
```csharp
public class GameEvent
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public EventType Type { get; set; }
    public string EffectsJson { get; set; }
    public int Duration { get; set; } // Days
    public float Probability { get; set; } // 0.0-1.0
    public string TriggerConditionsJson { get; set; }
    public string ChoicesJson { get; set; } // For events with player choices
    
    [Ignore]
    public Dictionary<string, object> Effects
    {
        get => JsonSerializer.Deserialize<Dictionary<string, object>>(EffectsJson ?? "{}");
        set => EffectsJson = JsonSerializer.Serialize(value);
    }
}

public enum EventType
{
    Market,
    Player,
    Story,
    Reputation
}
```

---

## Implementation Roadmap

### Week-by-Week Breakdown

| Week | Phase | Focus Areas | Key Deliverables |
|------|-------|-------------|------------------|
| 1 | Phase 0-1 | Project setup, data models | Working project, database schema |
| 2 | Phase 1 | Core services, repositories | Save/load, seed data |
| 3 | Phase 2 | Economy system | Market pricing, simulation |
| 4 | Phase 3 | Trading system | Buy/sell, inventory |
| 5 | Phase 4 | Synthesis + AI | Crafting, GPT integration |
| 6 | Phase 5 | World & travel | City navigation, travel |
| 7 | Phase 6 | Events system | 20+ events, triggers |
| 8 | Phase 7 | UI Part 1 | Main menu, world map, market |
| 9 | Phase 7 | UI Part 2 | Inventory, synthesis lab |
| 10 | Phase 7 | UI Part 3 | SkiaSharp graphics, animations |
| 11 | Phase 8 | Progression | Tiers, achievements, unlocks |
| 12 | Phase 9 | Monetization | IAP, AdMob integration |
| 13 | Phase 10 | Polish Part 1 | Audio, haptics, notifications |
| 14 | Phase 10 | Polish Part 2 | Optimization, accessibility |
| 15 | Phase 11 | Testing Part 1 | Unit tests, integration tests |
| 16 | Phase 11 | Testing Part 2 | Device testing, beta |
| 17 | Phase 12 | Deployment | Google Play release |

### Critical Path Items

**Must Complete in Order:**
1. Project setup → Data models → Database
2. Core services → Economy → Trading
3. Trading → Synthesis
4. All backend systems → UI implementation
5. Core features → Monetization → Testing → Deployment

**Can Parallelize:**
- Events system while working on UI
- Progression system while polishing UI
- Audio/haptics during testing phase

---

## Testing Strategy

### Unit Testing Approach

**Framework:** xUnit + Moq + FluentAssertions

**Test Structure:**
```
DreamAlchemist.Tests/
├── Services/
│   ├── MarketEngineTests.cs
│   ├── SynthesisEngineTests.cs
│   ├── InventoryManagerTests.cs
│   └── ...
├── ViewModels/
│   ├── MarketViewModelTests.cs
│   └── ...
└── Utilities/
    └── ...
```

**Test Coverage Goals:**
- Services: 80%+
- Business logic: 90%+
- ViewModels: 70%+
- UI code: Not unit tested (integration tested)

### Integration Testing

**Key Scenarios:**
1. **Complete Trade Flow**
   - Start with seed data
   - Navigate to market
   - Buy ingredient
   - Travel to different city
   - Sell ingredient at profit
   - Verify coins increased
   - Verify inventory updated

2. **Complete Synthesis Flow**
   - Gather 3 ingredients
   - Open synthesis lab
   - Combine ingredients
   - Verify recipe discovered
   - Verify crafted dream created
   - Sell crafted dream
   - Verify high profit

3. **Event Flow**
   - Trigger event
   - Verify effect applied
   - Verify duration
   - Verify expiration

4. **Progression Flow**
   - Start at Tier 1
   - Accumulate requirements
   - Verify tier advancement
   - Verify unlocks

### Device Testing Matrix

| Device Type | Android Version | Screen Size | Priority |
|-------------|----------------|-------------|----------|
| Low-end phone | API 21 (5.0) | 4.5" | High |
| Mid-range phone | API 26 (8.0) | 5.5" | Critical |
| Modern phone | API 31 (12) | 6.1" | Critical |
| Large phone | API 34 (14) | 6.7" | Medium |
| Tablet | API 31 (12) | 10" | Low |

**Test Cases Per Device:**
- [ ] App installs successfully
- [ ] App launches without crash
- [ ] All UI screens render correctly
- [ ] Touch interactions responsive
- [ ] Performance acceptable (30+ FPS)
- [ ] No visual glitches
- [ ] Audio plays correctly
- [ ] Save/load works
- [ ] IAP flow works (production test)

### Performance Benchmarks

**Targets:**
- Startup time: < 3 seconds (cold start)
- Frame rate: 60 FPS (target), 30 FPS (minimum)
- Memory usage: < 200 MB
- APK size: < 100 MB
- Battery drain: < 5% per hour of active play

**Profiling Tools:**
- .NET MAUI built-in profiler
- Android Studio Profiler
- Visual Studio Performance Profiler

---

## Deployment Checklist

### Pre-Launch Checklist

#### Code Preparation
- [ ] All features implemented
- [ ] All critical bugs fixed
- [ ] Unit tests passing (80%+ coverage)
- [ ] Integration tests passing
- [ ] Performance targets met
- [ ] Memory leaks fixed
- [ ] Code reviewed and cleaned

#### Android Configuration
- [ ] Target SDK set to latest (API 34+)
- [ ] Minimum SDK verified (API 21)
- [ ] Permissions properly declared
- [ ] App signing configured
- [ ] ProGuard/R8 rules configured
- [ ] AOT compilation enabled
- [ ] Version code/name set

#### Assets & Content
- [ ] All placeholder graphics replaced/generated
- [ ] Audio files optimized and included
- [ ] 50+ ingredients with data
- [ ] 5+ cities with data
- [ ] 20+ events with data
- [ ] 30+ achievements defined
- [ ] All strings in resource files
- [ ] Privacy policy written and published

#### Monetization
- [ ] In-app products created in Play Console
- [ ] AdMob account created and linked
- [ ] Test purchases successful
- [ ] Test ads displaying
- [ ] Premium features tested
- [ ] Restore purchases working

#### Google Play Store
- [ ] Developer account created
- [ ] App listing created
- [ ] App icon uploaded (512x512)
- [ ] Feature graphic created (1024x500)
- [ ] Screenshots captured (4-8)
- [ ] App description written
- [ ] Privacy policy link added
- [ ] Content rating completed
- [ ] Pricing & distribution set

#### Testing
- [ ] Internal testing completed
- [ ] Closed beta completed (2+ weeks)
- [ ] Beta feedback addressed
- [ ] Device testing on 5+ devices
- [ ] Performance verified on low-end device
- [ ] No critical bugs remaining

#### Legal & Compliance
- [ ] Privacy policy published
- [ ] Terms of service created
- [ ] COPPA compliance verified (if applicable)
- [ ] GDPR considerations (if targeting EU)
- [ ] Google Play policies reviewed
- [ ] Age rating appropriate

### Launch Day Checklist

