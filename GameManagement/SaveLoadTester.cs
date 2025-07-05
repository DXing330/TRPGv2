using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadTester : MonoBehaviour
{
    public SavedDataManager savedDataManager;
    public SceneMover sceneMover;
    public SaveDataReset saveDataReset;
    public List<SavedData> testSavedDataComponents;
    
    [System.Serializable]
    public class TestResult
    {
        public string testName;
        public bool passed;
        public string details;
        
        public TestResult(string name, bool result, string info = "")
        {
            testName = name;
            passed = result;
            details = info;
        }
    }
    
    public List<TestResult> testResults = new List<TestResult>();
    
    [ContextMenu("Run All Save Load Tests")]
    public void RunAllSaveLoadTests()
    {
        testResults.Clear();
        Debug.Log("=== Running Save/Load System Tests ===");
        
        TestSaveSystemInitialization();
        TestBasicSaveLoad();
        TestDataPersistence();
        TestSaveFileHandling();
        TestSceneTransition();
        TestSaveDataReset();
        TestMultipleDataSources();
        TestSaveIntegrity();
        TestErrorHandling();
        TestEdgeCases();
        
        LogTestResults();
    }
    
    [ContextMenu("Test Save System Initialization")]
    public void TestSaveSystemInitialization()
    {
        Debug.Log("Testing Save System Initialization...");
        
        // Test save data manager
        bool hasSaveDataManager = savedDataManager != null;
        testResults.Add(new TestResult("Save Data Manager", hasSaveDataManager, "Save data manager exists: " + hasSaveDataManager));
        
        // Test scene mover
        bool hasSceneMover = sceneMover != null;
        testResults.Add(new TestResult("Scene Mover", hasSceneMover, "Scene mover exists: " + hasSceneMover));
        
        // Test save data reset
        bool hasSaveDataReset = saveDataReset != null;
        testResults.Add(new TestResult("Save Data Reset", hasSaveDataReset, "Save data reset exists: " + hasSaveDataReset));
        
        // Test saved data components
        bool hasSavedDataComponents = testSavedDataComponents != null && testSavedDataComponents.Count > 0;
        testResults.Add(new TestResult("Saved Data Components", hasSavedDataComponents, "Saved data components count: " + (testSavedDataComponents?.Count ?? 0)));
    }
    
    [ContextMenu("Test Basic Save Load")]
    public void TestBasicSaveLoad()
    {
        Debug.Log("Testing Basic Save/Load...");
        
        if (testSavedDataComponents == null || testSavedDataComponents.Count == 0)
        {
            testResults.Add(new TestResult("Basic Save Load Setup", false, "No saved data components available"));
            return;
        }
        
        try
        {
            // Test save operation
            foreach (SavedData savedData in testSavedDataComponents)
            {
                if (savedData != null)
                {
                    // Add test data
                    savedData.dataList.Clear();
                    savedData.dataList.Add("TestData_" + Random.Range(1000, 9999));
                    
                    // Save the data
                    savedData.Save();
                    testResults.Add(new TestResult("Save Operation", true, "Data saved for " + savedData.name));
                    
                    // Clear data and reload
                    string originalData = savedData.dataList[0];
                    savedData.dataList.Clear();
                    
                    // Load the data
                    savedData.Load();
                    
                    // Verify data was loaded
                    bool dataLoaded = savedData.dataList.Count > 0 && savedData.dataList[0] == originalData;
                    testResults.Add(new TestResult("Load Operation", dataLoaded, "Data loaded correctly: " + dataLoaded));
                    
                    break; // Test with first valid component
                }
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Basic Save Load", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Data Persistence")]
    public void TestDataPersistence()
    {
        Debug.Log("Testing Data Persistence...");
        
        if (testSavedDataComponents == null || testSavedDataComponents.Count == 0)
        {
            testResults.Add(new TestResult("Data Persistence Setup", false, "No saved data components available"));
            return;
        }
        
        try
        {
            foreach (SavedData savedData in testSavedDataComponents)
            {
                if (savedData != null)
                {
                    // Test data persistence across multiple operations
                    string testData1 = "PersistenceTest1_" + Random.Range(1000, 9999);
                    string testData2 = "PersistenceTest2_" + Random.Range(1000, 9999);
                    
                    // Save first data
                    savedData.dataList.Clear();
                    savedData.dataList.Add(testData1);
                    savedData.Save();
                    
                    // Save second data
                    savedData.dataList.Clear();
                    savedData.dataList.Add(testData2);
                    savedData.Save();
                    
                    // Load and verify latest data
                    savedData.dataList.Clear();
                    savedData.Load();
                    
                    bool latestDataPersisted = savedData.dataList.Count > 0 && savedData.dataList[0] == testData2;
                    testResults.Add(new TestResult("Data Persistence", latestDataPersisted, "Latest data persisted correctly"));
                    
                    break; // Test with first valid component
                }
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Data Persistence", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Save File Handling")]
    public void TestSaveFileHandling()
    {
        Debug.Log("Testing Save File Handling...");
        
        try
        {
            // Test save file directory
            string saveDirectory = Application.persistentDataPath;
            bool saveDirectoryExists = Directory.Exists(saveDirectory);
            testResults.Add(new TestResult("Save Directory Exists", saveDirectoryExists, "Save directory: " + saveDirectory));
            
            // Test save file creation
            string testFileName = "TestSaveFile.txt";
            string testFilePath = Path.Combine(saveDirectory, testFileName);
            
            // Create test file
            File.WriteAllText(testFilePath, "TestSaveContent");
            bool testFileCreated = File.Exists(testFilePath);
            testResults.Add(new TestResult("Save File Creation", testFileCreated, "Test file created: " + testFileCreated));
            
            // Test save file reading
            if (testFileCreated)
            {
                string fileContent = File.ReadAllText(testFilePath);
                bool contentMatches = fileContent == "TestSaveContent";
                testResults.Add(new TestResult("Save File Reading", contentMatches, "File content matches: " + contentMatches));
                
                // Clean up test file
                File.Delete(testFilePath);
                bool fileDeleted = !File.Exists(testFilePath);
                testResults.Add(new TestResult("Save File Cleanup", fileDeleted, "Test file cleaned up: " + fileDeleted));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Save File Handling", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Scene Transition")]
    public void TestSceneTransition()
    {
        Debug.Log("Testing Scene Transition...");
        
        if (sceneMover == null)
        {
            testResults.Add(new TestResult("Scene Transition Setup", false, "No scene mover available"));
            return;
        }
        
        try
        {
            // Test scene transition methods exist
            bool hasTransitionMethods = sceneMover.GetType().GetMethod("ChangeScene") != null;
            testResults.Add(new TestResult("Scene Transition Methods", hasTransitionMethods, "Scene transition methods available: " + hasTransitionMethods));
            
            // Test current scene information
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            bool hasCurrentScene = !string.IsNullOrEmpty(currentScene);
            testResults.Add(new TestResult("Current Scene Info", hasCurrentScene, "Current scene: " + currentScene));
            
            // Test scene count
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            bool hasMultipleScenes = sceneCount > 1;
            testResults.Add(new TestResult("Multiple Scenes Available", hasMultipleScenes, "Scene count: " + sceneCount));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Scene Transition", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Save Data Reset")]
    public void TestSaveDataReset()
    {
        Debug.Log("Testing Save Data Reset...");
        
        if (saveDataReset == null)
        {
            testResults.Add(new TestResult("Save Data Reset Setup", false, "No save data reset available"));
            return;
        }
        
        try
        {
            // Test reset functionality exists
            bool hasResetMethods = saveDataReset.GetType().GetMethod("ResetSaveData") != null;
            testResults.Add(new TestResult("Reset Methods Available", hasResetMethods, "Reset methods exist: " + hasResetMethods));
            
            // Test with saved data components
            if (testSavedDataComponents != null && testSavedDataComponents.Count > 0)
            {
                foreach (SavedData savedData in testSavedDataComponents)
                {
                    if (savedData != null)
                    {
                        // Add test data
                        savedData.dataList.Clear();
                        savedData.dataList.Add("DataToReset");
                        
                        // Test reset
                        savedData.NewGame(); // This should reset the data
                        
                        // Verify reset worked
                        bool dataReset = savedData.dataList.Count == 0 || savedData.dataList[0] != "DataToReset";
                        testResults.Add(new TestResult("Data Reset", dataReset, "Data reset successfully: " + dataReset));
                        
                        break; // Test with first valid component
                    }
                }
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Save Data Reset", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Multiple Data Sources")]
    public void TestMultipleDataSources()
    {
        Debug.Log("Testing Multiple Data Sources...");
        
        if (testSavedDataComponents == null || testSavedDataComponents.Count < 2)
        {
            testResults.Add(new TestResult("Multiple Data Sources Setup", false, "Need at least 2 saved data components"));
            return;
        }
        
        try
        {
            // Test multiple data sources can save/load independently
            List<string> testData = new List<string>();
            
            for (int i = 0; i < Mathf.Min(2, testSavedDataComponents.Count); i++)
            {
                SavedData savedData = testSavedDataComponents[i];
                if (savedData != null)
                {
                    string uniqueData = "MultiTest_" + i + "_" + Random.Range(1000, 9999);
                    testData.Add(uniqueData);
                    
                    // Save unique data
                    savedData.dataList.Clear();
                    savedData.dataList.Add(uniqueData);
                    savedData.Save();
                }
            }
            
            // Load and verify data independence
            bool dataIndependent = true;
            for (int i = 0; i < Mathf.Min(2, testSavedDataComponents.Count); i++)
            {
                SavedData savedData = testSavedDataComponents[i];
                if (savedData != null)
                {
                    savedData.dataList.Clear();
                    savedData.Load();
                    
                    if (savedData.dataList.Count == 0 || savedData.dataList[0] != testData[i])
                    {
                        dataIndependent = false;
                        break;
                    }
                }
            }
            
            testResults.Add(new TestResult("Data Source Independence", dataIndependent, "Multiple data sources work independently"));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Multiple Data Sources", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Save Integrity")]
    public void TestSaveIntegrity()
    {
        Debug.Log("Testing Save Integrity...");
        
        if (testSavedDataComponents == null || testSavedDataComponents.Count == 0)
        {
            testResults.Add(new TestResult("Save Integrity Setup", false, "No saved data components available"));
            return;
        }
        
        try
        {
            foreach (SavedData savedData in testSavedDataComponents)
            {
                if (savedData != null)
                {
                    // Test large data integrity
                    List<string> largeData = new List<string>();
                    for (int i = 0; i < 100; i++)
                    {
                        largeData.Add("LargeDataItem_" + i);
                    }
                    
                    // Save large data
                    savedData.dataList.Clear();
                    savedData.dataList.AddRange(largeData);
                    savedData.Save();
                    
                    // Load and verify
                    savedData.dataList.Clear();
                    savedData.Load();
                    
                    bool integrityMaintained = savedData.dataList.Count == 100 && 
                                             savedData.dataList[0] == "LargeDataItem_0" && 
                                             savedData.dataList[99] == "LargeDataItem_99";
                    testResults.Add(new TestResult("Large Data Integrity", integrityMaintained, "Large data integrity maintained"));
                    
                    // Test special characters
                    string specialData = "Special!@#$%^&*()Data_测试_🎮";
                    savedData.dataList.Clear();
                    savedData.dataList.Add(specialData);
                    savedData.Save();
                    
                    savedData.dataList.Clear();
                    savedData.Load();
                    
                    bool specialCharsHandled = savedData.dataList.Count > 0 && savedData.dataList[0] == specialData;
                    testResults.Add(new TestResult("Special Characters", specialCharsHandled, "Special characters handled correctly"));
                    
                    break; // Test with first valid component
                }
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Save Integrity", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Error Handling")]
    public void TestErrorHandling()
    {
        Debug.Log("Testing Error Handling...");
        
        try
        {
            // Test invalid file path handling
            string invalidPath = "Z:\\InvalidPath\\NonExistentFile.txt";
            bool errorHandled = true;
            try
            {
                File.ReadAllText(invalidPath);
                errorHandled = false;
            }
            catch (System.Exception)
            {
                errorHandled = true;
            }
            testResults.Add(new TestResult("Invalid Path Error Handling", errorHandled, "Invalid path errors handled gracefully"));
            
            // Test null data handling
            if (testSavedDataComponents != null && testSavedDataComponents.Count > 0)
            {
                foreach (SavedData savedData in testSavedDataComponents)
                {
                    if (savedData != null)
                    {
                        // Test with null data list
                        savedData.dataList = null;
                        try
                        {
                            savedData.Save();
                            testResults.Add(new TestResult("Null Data Handling", true, "Null data handled gracefully"));
                        }
                        catch (System.Exception e)
                        {
                            testResults.Add(new TestResult("Null Data Handling", false, "Null data caused error: " + e.Message));
                        }
                        
                        // Restore data list
                        savedData.dataList = new List<string>();
                        break;
                    }
                }
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Error Handling", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Edge Cases")]
    public void TestEdgeCases()
    {
        Debug.Log("Testing Edge Cases...");
        
        if (testSavedDataComponents == null || testSavedDataComponents.Count == 0)
        {
            testResults.Add(new TestResult("Edge Cases Setup", false, "No saved data components available"));
            return;
        }
        
        try
        {
            foreach (SavedData savedData in testSavedDataComponents)
            {
                if (savedData != null)
                {
                    // Test empty data
                    savedData.dataList.Clear();
                    savedData.Save();
                    savedData.Load();
                    bool emptyDataHandled = savedData.dataList.Count == 0;
                    testResults.Add(new TestResult("Empty Data Handling", emptyDataHandled, "Empty data handled correctly"));
                    
                    // Test very long string
                    string longString = new string('A', 10000);
                    savedData.dataList.Clear();
                    savedData.dataList.Add(longString);
                    savedData.Save();
                    savedData.dataList.Clear();
                    savedData.Load();
                    bool longStringHandled = savedData.dataList.Count > 0 && savedData.dataList[0] == longString;
                    testResults.Add(new TestResult("Long String Handling", longStringHandled, "Long string handled correctly"));
                    
                    // Test empty string
                    savedData.dataList.Clear();
                    savedData.dataList.Add("");
                    savedData.Save();
                    savedData.dataList.Clear();
                    savedData.Load();
                    bool emptyStringHandled = savedData.dataList.Count > 0 && savedData.dataList[0] == "";
                    testResults.Add(new TestResult("Empty String Handling", emptyStringHandled, "Empty string handled correctly"));
                    
                    break; // Test with first valid component
                }
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Edge Cases", false, "Failed: " + e.Message));
        }
    }
    
    private void LogTestResults()
    {
        Debug.Log("=== Test Results Summary ===");
        int passed = 0, failed = 0;
        
        foreach (TestResult result in testResults)
        {
            if (result.passed)
            {
                passed++;
                Debug.Log($"✓ {result.testName}: PASSED - {result.details}");
            }
            else
            {
                failed++;
                Debug.LogError($"✗ {result.testName}: FAILED - {result.details}");
            }
        }
        
        Debug.Log($"=== Total: {passed} passed, {failed} failed ===");
    }
}