# Dream Alchemist - Deployment Guide

## 📋 Pre-Deployment Checklist

### Build Verification ✅
- [x] Project builds without errors
- [x] All warnings documented and understood
- [x] Dependencies resolved correctly
- [x] Seed data populated and valid
- [ ] Tested on Android emulator
- [ ] Tested on physical device (API 21-34)

### Code Quality ✅
- [x] MVVM architecture implemented
- [x] Dependency injection configured
- [x] Error handling in place
- [x] Value converters registered
- [x] Navigation routes configured

### Assets 📝
- [ ] App icon created (512x512 PNG)
- [ ] Splash screen configured
- [ ] Screenshots prepared (phone + tablet)
- [ ] Feature graphic (1024x500)
- [ ] Promotional images

## 🔧 Testing Instructions

### Local Testing (Development Build)

#### 1. Test on Android Emulator

**Setup Emulator:**
```bash
# List available emulators
emulator -list-avds

# Start emulator
emulator -avd Pixel_5_API_34 -netdelay none -netspeed full
```

**Run App:**
```bash
cd src/DreamAlchemist
dotnet build -t:Run -f net9.0-android
```

**Test Scenarios:**

**A. App Launch & Initialization**
- [ ] App launches without crashes
- [ ] Splash screen displays
- [ ] Database initializes
- [ ] Seed data loads correctly
- [ ] MainPage appears with default player

**B. Navigation Flow**
- [ ] Navigate to Market → loads prices
- [ ] Navigate to Lab → shows ingredients
- [ ] Navigate to Inventory → displays items
- [ ] Navigate to Travel → shows cities
- [ ] Back navigation works correctly

**C. Market System**
- [ ] Market prices display correctly
- [ ] Buy ingredient → coins decrease
- [ ] Sell ingredient → coins increase
- [ ] Inventory updates after purchase
- [ ] Weight limits enforced
- [ ] Insufficient funds handled

**D. Crafting System**
- [ ] Select 2-3 ingredients
- [ ] Craft button enables/disables
- [ ] Crafting consumes ingredients
- [ ] Crafted dream appears
- [ ] Recipe discovery works

**E. Inventory Management**
- [ ] Inventory items display
- [ ] Sort buttons work (Name, Rarity, etc.)
- [ ] Weight capacity shows correctly
- [ ] Progress bar updates

**F. Travel System**
- [ ] Cities list displays
- [ ] Current city marked
- [ ] Locked cities show correctly
- [ ] Travel consumes coins and time
- [ ] City change reflected everywhere

**G. Time Progression**
- [ ] Sleep button progresses day
- [ ] Market prices update
- [ ] Events can trigger
- [ ] Day counter increments

**H. Save/Load**
- [ ] Game state saves automatically
- [ ] App restart loads saved state
- [ ] Player progress persists
- [ ] Inventory persists
- [ ] Market state persists

### Performance Testing

**Metrics to Check:**
- [ ] App launch time < 3 seconds
- [ ] Navigation transitions smooth
- [ ] No UI freezing
- [ ] Scrolling is smooth (60fps)
- [ ] Memory usage reasonable
- [ ] Battery drain acceptable

### Device Compatibility

**Test on Multiple API Levels:**
- [ ] API 21 (Android 5.0 - Minimum)
- [ ] API 26 (Android 8.0)
- [ ] API 30 (Android 11)
- [ ] API 34 (Android 14 - Target)

**Test on Different Screen Sizes:**
- [ ] Small phone (4.7")
- [ ] Standard phone (6.0")
- [ ] Large phone (6.7")
- [ ] Tablet (10")

## 🏗️ Release Build Configuration

### 1. Update Version Numbers

Edit `DreamAlchemist.csproj`:
```xml
<ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>
```

### 2. Configure Release Build

Edit `DreamAlchemist.csproj`:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidPackageFormat>aab</AndroidPackageFormat>
  <AndroidKeyStore>True</AndroidKeyStore>
  <AndroidSigningKeyStore>dreamalchemist.keystore</AndroidSigningKeyStore>
  <AndroidSigningKeyAlias>dreamalchemist</AndroidSigningKeyAlias>
  <AndroidSigningKeyPass></AndroidSigningKeyPass>
  <AndroidSigningStorePass></AndroidSigningStorePass>
  <AndroidLinkMode>SdkOnly</AndroidLinkMode>
  <RunAOTCompilation>True</RunAOTCompilation>
  <AndroidEnableProfiledAot>True</AndroidEnableProfiledAot>
</PropertyGroup>
```

### 3. Create Signing Key

```bash
# Generate keystore
keytool -genkey -v -keystore dreamalchemist.keystore -alias dreamalchemist -keyalg RSA -keysize 2048 -validity 10000

# Store passwords securely (use environment variables or secure vault)
```

### 4. Build Release AAB

```bash
cd src/DreamAlchemist
dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=aab
```

Output: `bin/Release/net9.0-android/publish/com.dreamalchemist.game-Signed.aab`

### 5. Build Release APK (for testing)

```bash
dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=apk
```

## 📱 Google Play Store Submission

### Store Listing Information

**App Details:**
```
Title: Dream Alchemist
Short Description: Trade dream ingredients in a surreal economic simulation

Full Description:
Welcome to Dream Alchemist, where you trade in the economy of dreams.

🌙 EXPLORE THE DREAM MARKET
Buy and sell rare dream ingredients in a dynamic market where prices shift based on supply, demand, and mysterious events.

✨ CRAFT UNIQUE DREAMS
Combine ingredients to synthesize dreams. Discover rare recipes and create legendary dreams worth fortunes.

🗺️ TRAVEL BETWEEN SURREAL CITIES
Each city has its own culture and market dynamics. Build your reputation and unlock new locations.

📈 BUILD YOUR EMPIRE
Start as a Novice Peddler and rise to become a Lucid Architect through clever trading and crafting.

FEATURES:
• Dynamic market simulation with real-time price changes
• 15+ unique dream ingredients to collect
• Crafting system with recipe discovery
• 5 distinct cities to explore
• Reputation system with multiple paths
• Random events that affect the economy
• Deep progression system with 5 tiers

Category: Games > Simulation
Content Rating: Everyone 10+
```

**Privacy Policy:**
```
Required URL: https://yourdomain.com/privacy-policy

Privacy Policy must cover:
- Data collected (save games, analytics if any)
- How data is used
- Data storage and security
- User rights
```

### Assets Required

**App Icon:**
- 512x512 PNG (32-bit, transparency allowed)
- High-resolution icon for store

**Screenshots (Required: 2 minimum):**
- Phone: 1080x1920 or 1080x2340 (PNG/JPEG)
- Tablet: 1536x2048 (PNG/JPEG)
- At least 2 screenshots required

**Feature Graphic:**
- 1024x500 (PNG/JPEG, no transparency)
- Displayed at top of store listing

**Promotional Assets (Optional):**
- Promo video (YouTube link)
- TV banner: 1280x720
- Wear screenshot: 384x384

### Testing Track

**Recommended: Internal Testing First**
```
1. Internal Testing (10-100 testers)
   - Quick deployment
   - No review required
   - Test core functionality

2. Closed Alpha (up to 100 testers)
   - Invite-only
   - Gather feedback
   - Fix critical issues

3. Open Beta (unlimited)
   - Public testing
   - Wider feedback
   - Final polish

4. Production
   - Full release
   - Monitored rollout (10% → 50% → 100%)
```

## 🐛 Common Issues & Solutions

### Build Issues

**Issue: "AndroidSigningKeyStore not found"**
```
Solution: Ensure keystore path is correct and file exists
```

**Issue: "AOT compilation failed"**
```
Solution: Disable AOT temporarily or fix incompatible code
<RunAOTCompilation>False</RunAOTCompilation>
```

**Issue: "Duplicate class found"**
```
Solution: Check for conflicting NuGet packages
```

### Runtime Issues

**Issue: "App crashes on startup"**
```
Check:
1. Database initialization
2. Seed data JSON format
3. Dependency injection setup
4. Permissions in AndroidManifest
```

**Issue: "Database not found"**
```
Solution: Ensure Resources/Raw/Data files are set as MauiAsset
<MauiAsset Include="Resources\Raw\**" />
```

**Issue: "Navigation not working"**
```
Solution: Verify routes registered in AppShell.xaml.cs
```

## 📊 Post-Launch Monitoring

### Metrics to Track

**Performance:**
- Crash rate (target: <1%)
- ANR rate (target: <0.5%)
- App start time
- Memory usage

**Engagement:**
- Daily active users
- Session length
- Retention rate (D1, D7, D30)
- Feature usage

**Technical:**
- Device distribution
- Android version distribution
- Install/uninstall rate
- Update adoption rate

### Analytics Integration (Future)

```csharp
// Firebase Analytics or similar
public interface IAnalyticsService
{
    void LogEvent(string eventName, Dictionary<string, string> parameters);
    void SetUserProperty(string name, string value);
}
```

## 🔄 Update Process

### Versioning Strategy

```
Semantic Versioning: MAJOR.MINOR.PATCH

1.0.0 - Initial release
1.0.1 - Bug fixes
1.1.0 - New features
2.0.0 - Major changes
```

### Update Checklist

- [ ] Increment version numbers
- [ ] Test on all supported API levels
- [ ] Create release notes
- [ ] Build signed AAB
- [ ] Upload to appropriate track
- [ ] Monitor rollout
- [ ] Gather user feedback

## 📞 Support

### User Support Channels

- Email: support@dreamalchemist.com
- Twitter: @DreamAlchemistGame
- Discord: discord.gg/dreamalchemist

### Issue Tracking

Use GitHub Issues or similar for:
- Bug reports
- Feature requests
- Documentation updates

## 🎉 Launch Day Checklist

**T-7 Days:**
- [ ] Final testing complete
- [ ] Release build created and signed
- [ ] Store listing finalized
- [ ] Screenshots and assets prepared
- [ ] Privacy policy published

**T-3 Days:**
- [ ] Upload to closed testing
- [ ] Verify all features work
- [ ] Check analytics integration
- [ ] Prepare marketing materials

**T-1 Day:**
- [ ] Promote to production
- [ ] Set rollout percentage
- [ ] Monitor crash reports
- [ ] Prepare for user feedback

**Launch Day:**
- [ ] Monitor Play Console
- [ ] Respond to early reviews
- [ ] Track metrics
- [ ] Address critical issues immediately

**Post-Launch:**
- [ ] Gather user feedback
- [ ] Plan first update
- [ ] Continue monitoring
- [ ] Build community

---

**Good luck with your launch! 🚀**
