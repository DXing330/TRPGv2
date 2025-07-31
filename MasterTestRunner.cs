using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterTestRunner : MonoBehaviour
{
    /*[Header("Test Components")]
    public BattleManagerTester battleManagerTester;
    public PartyDataManagerTester partyDataManagerTester;
    public RequestBoardTester requestBoardTester;
    public MoveCostManagerTester moveCostManagerTester;
    public SaveLoadTester saveLoadTester;
    
    [Header("Test Results")]
    public List<TestSuite> testSuites = new List<TestSuite>();
    
    [System.Serializable]
    public class TestSuite
    {
        public string suiteName;
        public int totalTests;
        public int passedTests;
        public int failedTests;
        public float executionTime;
        public bool completed;
        
        public TestSuite(string name)
        {
            suiteName = name;
            totalTests = 0;
            passedTests = 0;
            failedTests = 0;
            executionTime = 0f;
            completed = false;
        }
    }
    
    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        Debug.Log("=== MASTER TEST RUNNER - Starting All Tests ===");
        StartCoroutine(RunAllTestsCoroutine());
    }
    
    [ContextMenu("Run Battle System Tests")]
    public void RunBattleSystemTests()
    {
        Debug.Log("=== Running Battle System Tests ===");
        StartCoroutine(RunBattleSystemTestsCoroutine());
    }
    
    [ContextMenu("Run Data Management Tests")]
    public void RunDataManagementTests()
    {
        Debug.Log("=== Running Data Management Tests ===");
        StartCoroutine(RunDataManagementTestsCoroutine());
    }
    
    [ContextMenu("Run UI System Tests")]
    public void RunUISystemTests()
    {
        Debug.Log("=== Running UI System Tests ===");
        StartCoroutine(RunUISystemTestsCoroutine());
    }
    
    [ContextMenu("Generate Test Report")]
    public void GenerateTestReport()
    {
        Debug.Log("=== COMPREHENSIVE TEST REPORT ===");
        GenerateDetailedReport();
    }
    
    private IEnumerator RunAllTestsCoroutine()
    {
        testSuites.Clear();
        float totalStartTime = Time.realtimeSinceStartup;
        
        // Run Battle Manager Tests
        if (battleManagerTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Battle Manager", () => battleManagerTester.RunAllBattleTests()));
        }
        
        // Run Party Data Manager Tests
        if (partyDataManagerTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Party Data Manager", () => partyDataManagerTester.RunAllPartyDataTests()));
        }
        
        // Run Request Board Tests
        if (requestBoardTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Request Board", () => requestBoardTester.RunAllRequestBoardTests()));
        }
        
        // Run Move Cost Manager Tests
        if (moveCostManagerTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Move Cost Manager", () => moveCostManagerTester.RunAllMoveCostTests()));
        }
        
        // Run Save/Load Tests
        if (saveLoadTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Save/Load System", () => saveLoadTester.RunAllSaveLoadTests()));
        }
        
        float totalTime = Time.realtimeSinceStartup - totalStartTime;
        Debug.Log($"=== ALL TESTS COMPLETED IN {totalTime:F2} SECONDS ===");
        
        GenerateDetailedReport();
    }
    
    private IEnumerator RunBattleSystemTestsCoroutine()
    {
        testSuites.Clear();
        
        if (battleManagerTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Battle Manager", () => battleManagerTester.RunAllBattleTests()));
        }
        
        if (moveCostManagerTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Move Cost Manager", () => moveCostManagerTester.RunAllMoveCostTests()));
        }
        
        GenerateDetailedReport();
    }
    
    private IEnumerator RunDataManagementTestsCoroutine()
    {
        testSuites.Clear();
        
        if (partyDataManagerTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Party Data Manager", () => partyDataManagerTester.RunAllPartyDataTests()));
        }
        
        if (saveLoadTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Save/Load System", () => saveLoadTester.RunAllSaveLoadTests()));
        }
        
        GenerateDetailedReport();
    }
    
    private IEnumerator RunUISystemTestsCoroutine()
    {
        testSuites.Clear();
        
        if (requestBoardTester != null)
        {
            yield return StartCoroutine(RunTestSuite("Request Board", () => requestBoardTester.RunAllRequestBoardTests()));
        }
        
        GenerateDetailedReport();
    }
    
    private IEnumerator RunTestSuite(string suiteName, System.Action testAction)
    {
        Debug.Log($"--- Starting {suiteName} Tests ---");
        
        TestSuite suite = new TestSuite(suiteName);
        testSuites.Add(suite);
        
        float startTime = Time.realtimeSinceStartup;
        
        try
        {
            testAction.Invoke();
            suite.completed = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Test suite {suiteName} failed with exception: {e.Message}");
            suite.completed = false;
        }
        
        suite.executionTime = Time.realtimeSinceStartup - startTime;
        
        // Count results from the individual tester
        CountTestResults(suite, suiteName);
        
        Debug.Log($"--- {suiteName} Tests Completed in {suite.executionTime:F2}s ---");
        
        yield return new WaitForSeconds(0.1f); // Small delay between test suites
    }
    
    private void CountTestResults(TestSuite suite, string suiteName)
    {
        // This is a simplified approach - in a real implementation,
        // you'd want to get the actual test results from each tester
        try
        {
            switch (suiteName)
            {
                case "Battle Manager":
                    if (battleManagerTester != null && battleManagerTester.testResults != null)
                    {
                        suite.totalTests = battleManagerTester.testResults.Count;
                        foreach (var result in battleManagerTester.testResults)
                        {
                            if (result.passed) suite.passedTests++;
                            else suite.failedTests++;
                        }
                    }
                    break;
                    
                case "Party Data Manager":
                    if (partyDataManagerTester != null && partyDataManagerTester.testResults != null)
                    {
                        suite.totalTests = partyDataManagerTester.testResults.Count;
                        foreach (var result in partyDataManagerTester.testResults)
                        {
                            if (result.passed) suite.passedTests++;
                            else suite.failedTests++;
                        }
                    }
                    break;
                    
                case "Request Board":
                    if (requestBoardTester != null && requestBoardTester.testResults != null)
                    {
                        suite.totalTests = requestBoardTester.testResults.Count;
                        foreach (var result in requestBoardTester.testResults)
                        {
                            if (result.passed) suite.passedTests++;
                            else suite.failedTests++;
                        }
                    }
                    break;
                    
                case "Move Cost Manager":
                    if (moveCostManagerTester != null && moveCostManagerTester.testResults != null)
                    {
                        suite.totalTests = moveCostManagerTester.testResults.Count;
                        foreach (var result in moveCostManagerTester.testResults)
                        {
                            if (result.passed) suite.passedTests++;
                            else suite.failedTests++;
                        }
                    }
                    break;
                    
                case "Save/Load System":
                    if (saveLoadTester != null && saveLoadTester.testResults != null)
                    {
                        suite.totalTests = saveLoadTester.testResults.Count;
                        foreach (var result in saveLoadTester.testResults)
                        {
                            if (result.passed) suite.passedTests++;
                            else suite.failedTests++;
                        }
                    }
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not count test results for {suiteName}: {e.Message}");
        }
    }
    
    private void GenerateDetailedReport()
    {
        Debug.Log("=== DETAILED TEST REPORT ===");
        
        int totalTests = 0;
        int totalPassed = 0;
        int totalFailed = 0;
        float totalTime = 0f;
        
        foreach (TestSuite suite in testSuites)
        {
            totalTests += suite.totalTests;
            totalPassed += suite.passedTests;
            totalFailed += suite.failedTests;
            totalTime += suite.executionTime;
            
            float passRate = suite.totalTests > 0 ? (suite.passedTests / (float)suite.totalTests) * 100f : 0f;
            
            Debug.Log($"📋 {suite.suiteName}:");
            Debug.Log($"   Tests: {suite.totalTests} | Passed: {suite.passedTests} | Failed: {suite.failedTests}");
            Debug.Log($"   Pass Rate: {passRate:F1}% | Time: {suite.executionTime:F2}s | Status: {(suite.completed ? "✓" : "✗")}");
        }
        
        Debug.Log("=== SUMMARY ===");
        float overallPassRate = totalTests > 0 ? (totalPassed / (float)totalTests) * 100f : 0f;
        Debug.Log($"🎯 Overall Results:");
        Debug.Log($"   Total Tests: {totalTests}");
        Debug.Log($"   Passed: {totalPassed}");
        Debug.Log($"   Failed: {totalFailed}");
        Debug.Log($"   Pass Rate: {overallPassRate:F1}%");
        Debug.Log($"   Total Time: {totalTime:F2}s");
        
        if (overallPassRate >= 90f)
        {
            Debug.Log("🎉 EXCELLENT! Test suite is in great shape!");
        }
        else if (overallPassRate >= 75f)
        {
            Debug.Log("👍 GOOD! Most tests are passing, but some issues need attention.");
        }
        else if (overallPassRate >= 50f)
        {
            Debug.Log("⚠️ NEEDS WORK! Significant issues detected.");
        }
        else
        {
            Debug.Log("🚨 CRITICAL! Major problems detected. Immediate attention required.");
        }
        
        Debug.Log("=== END REPORT ===");
    }
    
    [ContextMenu("Quick System Check")]
    public void QuickSystemCheck()
    {
        Debug.Log("=== QUICK SYSTEM CHECK ===");
        
        // Check if all tester components are assigned
        List<string> missingComponents = new List<string>();
        
        if (battleManagerTester == null) missingComponents.Add("Battle Manager Tester");
        if (partyDataManagerTester == null) missingComponents.Add("Party Data Manager Tester");
        if (requestBoardTester == null) missingComponents.Add("Request Board Tester");
        if (moveCostManagerTester == null) missingComponents.Add("Move Cost Manager Tester");
        if (saveLoadTester == null) missingComponents.Add("Save Load Tester");
        
        if (missingComponents.Count == 0)
        {
            Debug.Log("✅ All test components are properly assigned!");
        }
        else
        {
            Debug.LogWarning("⚠️ Missing test components:");
            foreach (string component in missingComponents)
            {
                Debug.LogWarning($"   - {component}");
            }
        }
        
        // Check Unity version compatibility
        string unityVersion = Application.unityVersion;
        Debug.Log($"🔧 Unity Version: {unityVersion}");
        
        // Check platform
        Debug.Log($"🖥️ Platform: {Application.platform}");
        
        Debug.Log("=== END QUICK CHECK ===");
    }*/
}