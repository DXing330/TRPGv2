# Quick Start Guide - Unity Testing

**Having NUnit errors?** This guide will get you testing immediately, regardless of your Unity version.

## ⚡ Option 1: Try Installing Test Framework First

1. **Open Package Manager**: `Window > Package Manager`
2. **Switch to Unity Registry** (top-left dropdown)
3. **Search for "Test Framework"**
4. **Click Install**
5. **Restart Unity**

If this works, use the original `*Tests.cs` files with Unity's Test Runner.

## ⚡ Option 2: Use Unity-Compatible Tests (Recommended for Older Unity)

If you're still getting NUnit errors, use these files instead:

### Files to Use:
- ✅ `BattleManagerTests_Unity.cs` (instead of `BattleManagerTests.cs`)
- ✅ `EquipmentTests_Unity.cs` (instead of `EquipmentTests.cs`)
- ✅ `UnityTestRunner.cs` (test execution manager)

### How to Run:

#### Method A: Menu Command (Easiest)
1. Go to `Tools > Run Unity Tests` in Unity's menu bar
2. Watch the Console for results
3. Done! ✓

#### Method B: Scene-Based Testing
1. Create a new scene
2. Create an empty GameObject
3. Attach the `UnityTestRunner` script
4. Press Play
5. Watch Console for test results

#### Method C: Individual Tests
1. Create empty GameObject in any scene
2. Attach `BattleManagerTests_Unity` or `EquipmentTests_Unity`
3. Press Play
4. Tests run automatically

## 📊 Understanding Results

### In the Console you'll see:
```
=== Starting Unity Test Runner ===
--- Running Battle Manager Tests ---
✓ GetRoundNumber test passed
✓ GetTurnIndex test passed
✓ GetState test passed
...
🎉 ALL TESTS PASSED! 🎉
```

### Success Indicators:
- ✅ Green checkmarks (✓) = Tests passed
- 🎉 "ALL TESTS PASSED!" = Everything working
- No red errors = Good to go!

### If Tests Fail:
- 🔴 Red error messages will show which test failed
- Error details help identify issues in your code
- Fix the underlying issue and re-run tests

## 🎯 What These Tests Check

### BattleManager Tests:
- Turn management and round progression
- Battle state handling (Move, Attack, Skill modes)
- Tile interaction and movement
- Combat system integration

### Equipment Tests:
- Equipment data parsing from strings
- Property extraction (name, slot, type, passives)
- Error handling for malformed data
- Actor equipment integration

## 🔧 Adding More Tests

To test other systems, follow this pattern:

```csharp
using UnityEngine;
using UnityEngine.Assertions;

public class MySystemTests_Unity : MonoBehaviour
{
    void Start() { RunAllTests(); }
    
    public void RunAllTests()
    {
        try {
            Setup();
            Test_MyFunction();
            TearDown();
            Debug.Log("✓ All tests passed!");
        } catch (System.Exception e) {
            Debug.LogError($"Test failed: {e.Message}");
        }
    }
    
    void Setup() { /* create test objects */ }
    void TearDown() { /* cleanup */ }
    
    void Test_MyFunction()
    {
        // Your test logic here
        Assert.AreEqual(expected, actual, "Test description");
        Debug.Log("✓ MyFunction test passed");
    }
}
```

## 🆘 Still Having Issues?

1. **Check Unity Version**: These work with Unity 2017.1+
2. **Check Console**: Look for specific error messages
3. **Missing Components**: Make sure your game scripts compile first
4. **Script Errors**: Fix any compilation errors before running tests

## 🎉 You're All Set!

The Unity-compatible tests provide the same validation as NUnit tests but work with any Unity version. They'll help you catch bugs, validate functionality, and maintain code quality as you develop your tactical RPG.

Happy testing! 🚀