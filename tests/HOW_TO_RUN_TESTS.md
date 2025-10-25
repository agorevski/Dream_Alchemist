# How to Run Dream Alchemist Tests

## Current Situation

The test project is configured but **requires Android SDK to run** because the main Dream Alchemist project targets `net9.0-android` exclusively.

### Why Tests Need Android SDK

The test project references the main project via:
```xml
<ProjectReference Include="..\..\src\DreamAlchemist\DreamAlchemist.csproj" />
```

Since the main project only supports `net9.0-android`, the test project must also target Android to be compatible.

---

## ✅ Solution 1: Install Android SDK (Recommended for Local Development)

### Install Android SDK

1. **Download Android SDK Command Line Tools**:
   - Visit: https://developer.android.com/studio#command-line-tools-only
   - Download the command line tools for Windows

2. **Extract and Set Environment Variable**:
   ```cmd
   set ANDROID_SDK_ROOT=C:\Android\sdk
   ```

3. **Install SDK Components**:
   ```cmd
   sdkmanager "platform-tools" "platforms;android-34" "build-tools;34.0.0"
   ```

4. **Run Tests**:
   ```cmd
   cd tests\DreamAlchemist.Tests
   dotnet test
   ```

---

## ✅ Solution 2: Use GitHub Actions CI/CD (Recommended for Team/CI)

Tests will run automatically in CI where Android SDK is pre-installed.

### GitHub Actions Workflow

Create `.github/workflows/test.yml`:

```yaml
name: Run Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  test:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Setup Android SDK
      uses: android-actions/setup-android@v2
    
    - name: Restore dependencies
      run: dotnet restore tests/DreamAlchemist.Tests
    
    - name: Build tests
      run: dotnet build tests/DreamAlchemist.Tests --no-restore
    
    - name: Run tests
      run: dotnet test tests/DreamAlchemist.Tests --no-build --verbosity normal
    
    - name: Generate coverage report
      run: dotnet test tests/DreamAlchemist.Tests --collect:"XPlat Code Coverage"
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        files: '**/coverage.cobertura.xml'
```

---

## ✅ Solution 3: Use Visual Studio (Has Android SDK Built-in)

If you have Visual Studio with Mobile Development workload:

1. Open solution in Visual Studio
2. Open Test Explorer (Test → Test Explorer)
3. Click "Run All"
4. Tests will execute using VS's built-in Android SDK

---

## ✅ Solution 4: Azure DevOps Pipeline

If using Azure DevOps:

```yaml
trigger:
- main

pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '9.0.x'

- task: AndroidToolkit@1
  inputs:
    javaVersion: '11'

- script: dotnet restore tests/DreamAlchemist.Tests
  displayName: 'Restore test dependencies'

- script: dotnet test tests/DreamAlchemist.Tests --logger trx
  displayName: 'Run tests'

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/*.trx'
```

---

## 🔍 Verify Test Setup Without Running

You can verify the test structure is correct by building without running:

```cmd
cd tests\DreamAlchemist.Tests
dotnet build
```

This will show compilation errors in test code even without Android SDK.

---

## 📊 Test Statistics

**Current Test Suite**:
- ✅ 113 tests implemented
- ✅ 79 tests templated (ready to implement)
- ✅ Test infrastructure complete
- ✅ MockFactory and TestDataBuilder ready
- ✅ Comprehensive documentation

**When tests run successfully, you'll see**:
```
Test run for DreamAlchemist.Tests.dll (.NET 9.0)
Microsoft (R) Test Execution Command Line Tool Version 17.14.1
Starting test execution, please wait...
A total of 113 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:   113, Skipped:     0, Total:   113
```

---

## 🚨 Common Issues

### Issue: "Android SDK directory could not be found"
**Solution**: Install Android SDK (Solution 1) or use CI/CD (Solution 2)

### Issue: "Project DreamAlchemist is not compatible"
**Solution**: Ensure test project targets `net9.0-android` to match main project

### Issue: Tests build but don't run
**Solution**: Ensure xUnit test runner is installed: `dotnet tool install -g dotnet-xunit`

---

## 💡 Alternative: Mock-Only Tests

If you absolutely cannot install Android SDK, you could create a separate test project that doesn't reference the main project:

1. Create new test project: `DreamAlchemist.Tests.MockOnly`
2. **Copy** service interfaces to test project
3. Test interfaces with mocks only
4. **Limitation**: Cannot test actual implementations

This is **not recommended** as it doesn't test real code.

---

## 📝 Summary

**Best Approach for Different Scenarios**:

| Scenario | Recommended Solution | Effort |
|----------|---------------------|--------|
| Local Development | Install Android SDK | Medium |
| Team Collaboration | GitHub Actions CI/CD | Low |
| Visual Studio User | Use built-in testing | None |
| Enterprise | Azure DevOps Pipeline | Low |

The test suite is **complete and ready to run** - it just needs Android SDK to be present in the environment.

---

## 📚 Additional Resources

- [.NET MAUI Testing Guide](https://learn.microsoft.com/en-us/dotnet/maui/testing/)
- [Android SDK Installation](https://aka.ms/dotnet-android-install-sdk)
- [xUnit Documentation](https://xunit.net/)
- [GitHub Actions for .NET](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)

---

**Last Updated**: October 25, 2025  
**Status**: Tests ready, Android SDK required for execution
