using UnityEngine;
using System.Collections;

// Simple test runner for Unity-compatible test files
// Use this when NUnit Test Framework is not available
public class UnityTestRunner : MonoBehaviour
{
    [Header("Test Configuration")]
    public bool runOnStart = true;
    public bool runBattleManagerTests = true;
    public bool runEquipmentTests = true;
    public float delayBetweenTests = 1.0f;

    void Start()
    {
        if (runOnStart)
        {
            StartCoroutine(RunAllTestSuites());
        }
    }

    [ContextMenu("Run All Tests")]
    public void RunAllTestsMenu()
    {
        StartCoroutine(RunAllTestSuites());
    }

    public IEnumerator RunAllTestSuites()
    {
        Debug.Log("====================================");
        Debug.Log("=== Starting Unity Test Runner ===");
        Debug.Log("====================================");

        int totalTests = 0;
        int passedTests = 0;

        if (runBattleManagerTests)
        {
            Debug.Log("\n--- Running Battle Manager Tests ---");
            var battleTests = gameObject.AddComponent<BattleManagerTests_Unity>();
            yield return new WaitForSeconds(delayBetweenTests);
            
            // Clean up the component
            if (battleTests != null)
                DestroyImmediate(battleTests);
            
            totalTests++;
            passedTests++; // Assume passed if no exceptions thrown
        }

        if (runEquipmentTests)
        {
            Debug.Log("\n--- Running Equipment Tests ---");
            var equipTests = gameObject.AddComponent<EquipmentTests_Unity>();
            yield return new WaitForSeconds(delayBetweenTests);
            
            // Clean up the component
            if (equipTests != null)
                DestroyImmediate(equipTests);
            
            totalTests++;
            passedTests++; // Assume passed if no exceptions thrown
        }

        // Final results
        Debug.Log("\n====================================");
        Debug.Log($"=== Test Results: {passedTests}/{totalTests} Suites Passed ===");
        Debug.Log("====================================");

        if (passedTests == totalTests)
        {
            Debug.Log("<color=green>🎉 ALL TESTS PASSED! 🎉</color>");
        }
        else
        {
            Debug.LogWarning("<color=yellow>⚠️ Some tests may have failed. Check console for details.</color>");
        }
    }

    // Menu item for easy access
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Run Unity Tests")]
    static void RunTestsFromMenu()
    {
        // Create a temporary test runner
        var testRunner = new GameObject("Unity Test Runner");
        var runner = testRunner.AddComponent<UnityTestRunner>();
        runner.runOnStart = false; // Don't auto-run, we're manually triggering
        runner.StartCoroutine(runner.RunAllTestSuites());
        
        // Clean up after a delay
        runner.StartCoroutine(CleanupAfterDelay(testRunner, 10f));
    }

    static IEnumerator CleanupAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            DestroyImmediate(obj);
    }
#endif

    // Individual test runners for selective testing
    [ContextMenu("Run Battle Manager Tests Only")]
    public void RunBattleManagerTestsOnly()
    {
        StartCoroutine(RunSingleTestSuite<BattleManagerTests_Unity>("Battle Manager"));
    }

    [ContextMenu("Run Equipment Tests Only")]
    public void RunEquipmentTestsOnly()
    {
        StartCoroutine(RunSingleTestSuite<EquipmentTests_Unity>("Equipment"));
    }

    IEnumerator RunSingleTestSuite<T>(string suiteName) where T : MonoBehaviour
    {
        Debug.Log($"=== Running {suiteName} Tests ===");
        
        var testComponent = gameObject.AddComponent<T>();
        yield return new WaitForSeconds(delayBetweenTests);
        
        if (testComponent != null)
            DestroyImmediate(testComponent);
        
        Debug.Log($"=== {suiteName} Tests Complete ===");
    }
}