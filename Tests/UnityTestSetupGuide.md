# Unity Test Framework Setup Guide

This guide helps resolve NUnit and Test Framework issues across different Unity versions.

## Quick Fix Options

### Option 1: Install Test Framework Package (Unity 2018.1+)

1. **Open Package Manager**: `Window > Package Manager`
2. **Switch to Unity Registry**: Top-left dropdown
3. **Search for "Test Framework"**
4. **Click Install** on "Test Framework" package
5. **Restart Unity** after installation

### Option 2: Manual NUnit Installation (Older Unity)

If Package Manager doesn't have Test Framework:

1. **Download NUnit 3.5**: [NUnit.org Downloads](https://nunit.org/download/)
2. **Extract to Assets/Plugins/NUnit/**
3. **Add assembly references** in your test files

### Option 3: Convert to Unity's Legacy Testing (Unity 5.6-2017)

For very old Unity versions, use Unity's built-in testing:

```csharp
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

// Use [UnityTest] instead of [Test] for coroutine tests
// Use Assert from UnityEngine.Assertions
```

## Version-Specific Solutions

### Unity 2019.2+ (Recommended)
- Test Framework comes pre-installed
- Full NUnit 3.x support
- Use the tests as-is

### Unity 2018.1 - 2019.1
- Install Test Framework via Package Manager
- May need to enable "Show Preview Packages"
- Restart Unity after installation

### Unity 2017.1 - 2018.0
- Use Unity Test Tools from Asset Store
- Or manual NUnit installation
- Limited NUnit features

### Unity 5.6 - 2017.0
- Use Unity's built-in testing framework
- Convert tests to use UnityEngine.Assertions
- Limited testing capabilities

## Alternative: Use Unity-Compatible Test Files

If you can't get NUnit working, I've created Unity-compatible versions that use `UnityEngine.Assertions` instead:

### Available Unity-Compatible Test Files:
- `BattleManagerTests_Unity.cs` - Tests for the battle system
- `EquipmentTests_Unity.cs` - Tests for equipment parsing and handling

### How to Use Unity-Compatible Tests:

1. **Add to Scene**: 
   - Create an empty GameObject in your test scene
   - Attach the test script (e.g., `BattleManagerTests_Unity`)
   - Run the scene to execute tests

2. **View Results**:
   - Watch the Console window for test results
   - ✓ marks indicate passed tests
   - Errors indicate failed tests

3. **Alternative: Run via Script**:
```csharp
[MenuItem("Tests/Run Battle Manager Tests")]
static void RunBattleManagerTests()
{
    var testObject = new GameObject("BattleManagerTests");
    testObject.AddComponent<BattleManagerTests_Unity>();
}
```

### Converting Other Tests to Unity Format:

Here's the pattern to convert NUnit tests to Unity-compatible format:

**Before (NUnit):**
```csharp
using NUnit.Framework;

[TestFixture]
public class MyTests
{
    [SetUp]
    public void Setup() { /* setup code */ }
    
    [TearDown] 
    public void TearDown() { /* cleanup code */ }
    
    [Test]
    public void MyTest()
    {
        Assert.AreEqual(expected, actual);
    }
}
```

**After (Unity-Compatible):**
```csharp
using UnityEngine;
using UnityEngine.Assertions;

public class MyTests_Unity : MonoBehaviour
{
    void Start() { RunAllTests(); }
    
    public void RunAllTests()
    {
        try {
            Setup();
            Test_MyTest();
            TearDown();
            Debug.Log("All tests passed!");
        } catch (System.Exception e) {
            Debug.LogError($"Test failed: {e.Message}");
        }
    }
    
    void Setup() { /* setup code */ }
    void TearDown() { /* cleanup code */ }
    
    void Test_MyTest()
    {
        Assert.AreEqual(expected, actual, "Test description");
        Debug.Log("✓ MyTest passed");
    }
}
```

### Key Differences:
1. **Namespace**: Use `UnityEngine.Assertions` instead of `NUnit.Framework`
2. **Class**: Inherit from `MonoBehaviour` instead of using `[TestFixture]`
3. **Execution**: Call tests from `Start()` or manually trigger
4. **Assertions**: Use `Assert.AreEqual(expected, actual, message)` format
5. **Feedback**: Use `Debug.Log()` for test results

### Running Multiple Test Classes:

Create a test runner script:

```csharp
public class TestRunner : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RunAllTestSuites());
    }
    
    IEnumerator RunAllTestSuites()
    {
        // Run BattleManager tests
        var battleTests = gameObject.AddComponent<BattleManagerTests_Unity>();
        yield return new WaitForSeconds(1);
        
        // Run Equipment tests  
        var equipTests = gameObject.AddComponent<EquipmentTests_Unity>();
        yield return new WaitForSeconds(1);
        
        Debug.Log("=== All Test Suites Complete ===");
    }
}
```

### Manual NUnit Installation (Advanced)

If you want to use the original NUnit test files: