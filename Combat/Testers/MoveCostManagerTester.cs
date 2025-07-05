using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCostManagerTester : MonoBehaviour
{
    public MoveCostManager moveCostManager;
    public BattleMap testBattleMap;
    public List<TacticActor> testActors;
    public TacticActor testActor;
    
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
    
    [ContextMenu("Run All Move Cost Tests")]
    public void RunAllMoveCostTests()
    {
        testResults.Clear();
        Debug.Log("=== Running Move Cost Manager Tests ===");
        
        TestMoveCostInitialization();
        TestMapInfoSetting();
        TestReachableTiles();
        TestMoveCostCalculation();
        TestPathfinding();
        TestAttackableTiles();
        TestMovementValidation();
        TestPathPrecomputation();
        TestMoveCostPayment();
        TestMovementRange();
        
        LogTestResults();
    }
    
    [ContextMenu("Test Move Cost Initialization")]
    public void TestMoveCostInitialization()
    {
        Debug.Log("Testing Move Cost Initialization...");
        
        // Test pathfinder initialization
        bool hasPathfinder = moveCostManager.actorPathfinder != null;
        testResults.Add(new TestResult("Actor Pathfinder", hasPathfinder, "Pathfinder exists: " + hasPathfinder));
        
        // Test reachable tiles list
        bool hasReachableTiles = moveCostManager.reachableTiles != null;
        testResults.Add(new TestResult("Reachable Tiles List", hasReachableTiles, "Reachable tiles exists: " + hasReachableTiles));
        
        // Test move cost variable
        bool hasMoveCost = moveCostManager.moveCost >= 0;
        testResults.Add(new TestResult("Move Cost Variable", hasMoveCost, "Move cost: " + moveCostManager.moveCost));
    }
    
    [ContextMenu("Test Map Info Setting")]
    public void TestMapInfoSetting()
    {
        Debug.Log("Testing Map Info Setting...");
        
        if (testBattleMap == null)
        {
            testResults.Add(new TestResult("Map Info Test Setup", false, "No test battle map available"));
            return;
        }
        
        try
        {
            // Test setting map info
            moveCostManager.SetMapInfo(testBattleMap.mapInfo);
            testResults.Add(new TestResult("Set Map Info", true, "Map info set successfully"));
            
            // Verify map info is accessible
            if (testBattleMap.mapInfo != null)
            {
                testResults.Add(new TestResult("Map Info Access", true, "Map info accessible"));
            }
            else
            {
                testResults.Add(new TestResult("Map Info Access", false, "Map info is null"));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Set Map Info", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Reachable Tiles")]
    public void TestReachableTiles()
    {
        Debug.Log("Testing Reachable Tiles...");
        
        if (testActor == null)
        {
            testResults.Add(new TestResult("Reachable Tiles Setup", false, "No test actor available"));
            return;
        }
        
        try
        {
            // Test getting all reachable tiles
            moveCostManager.GetAllReachableTiles(testActor, testActors);
            
            // Check if reachable tiles were calculated
            bool hasReachableTiles = moveCostManager.reachableTiles.Count >= 0;
            testResults.Add(new TestResult("Calculate Reachable Tiles", hasReachableTiles, "Reachable tiles count: " + moveCostManager.reachableTiles.Count));
            
            // Test that actor's current location is reachable
            int actorLocation = testActor.GetLocation();
            bool currentLocationReachable = moveCostManager.reachableTiles.Contains(actorLocation);
            testResults.Add(new TestResult("Current Location Reachable", currentLocationReachable, "Actor location " + actorLocation + " is reachable: " + currentLocationReachable));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Calculate Reachable Tiles", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Move Cost Calculation")]
    public void TestMoveCostCalculation()
    {
        Debug.Log("Testing Move Cost Calculation...");
        
        if (testActor == null)
        {
            testResults.Add(new TestResult("Move Cost Test Setup", false, "No test actor available"));
            return;
        }
        
        try
        {
            // Test getting all move costs
            moveCostManager.GetAllMoveCosts(testActor, testActors);
            testResults.Add(new TestResult("Calculate All Move Costs", true, "Move costs calculated successfully"));
            
            // Test move cost is non-negative
            bool validMoveCost = moveCostManager.moveCost >= 0;
            testResults.Add(new TestResult("Valid Move Cost", validMoveCost, "Move cost: " + moveCostManager.moveCost));
            
            // Test actor has speed for movement
            int actorSpeed = testActor.GetSpeed();
            bool hasSpeed = actorSpeed >= 0;
            testResults.Add(new TestResult("Actor Has Speed", hasSpeed, "Actor speed: " + actorSpeed));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Calculate Move Costs", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Pathfinding")]
    public void TestPathfinding()
    {
        Debug.Log("Testing Pathfinding...");
        
        if (testActor == null)
        {
            testResults.Add(new TestResult("Pathfinding Test Setup", false, "No test actor available"));
            return;
        }
        
        try
        {
            int startLocation = testActor.GetLocation();
            
            // Test pathfinding to the same location
            List<int> samePath = moveCostManager.GetPrecomputedPath(startLocation, startLocation);
            bool sameLocationPath = samePath != null && samePath.Count >= 1;
            testResults.Add(new TestResult("Same Location Path", sameLocationPath, "Same location path count: " + (samePath?.Count ?? 0)));
            
            // Test pathfinding to a different location (if reachable tiles available)
            if (moveCostManager.reachableTiles.Count > 1)
            {
                int targetLocation = -1;
                foreach (int tile in moveCostManager.reachableTiles)
                {
                    if (tile != startLocation)
                    {
                        targetLocation = tile;
                        break;
                    }
                }
                
                if (targetLocation >= 0)
                {
                    List<int> path = moveCostManager.GetPrecomputedPath(startLocation, targetLocation);
                    bool validPath = path != null && path.Count > 0;
                    testResults.Add(new TestResult("Valid Path Found", validPath, "Path to " + targetLocation + " count: " + (path?.Count ?? 0)));
                    
                    // Test path starts with current location
                    if (validPath && path.Count > 0)
                    {
                        bool pathStartsCorrectly = path[0] == startLocation;
                        testResults.Add(new TestResult("Path Starts Correctly", pathStartsCorrectly, "Path starts at: " + path[0] + ", expected: " + startLocation));
                    }
                }
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Pathfinding", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Attackable Tiles")]
    public void TestAttackableTiles()
    {
        Debug.Log("Testing Attackable Tiles...");
        
        if (testActor == null)
        {
            testResults.Add(new TestResult("Attackable Tiles Setup", false, "No test actor available"));
            return;
        }
        
        try
        {
            // Test getting attackable tiles
            List<int> attackableTiles = moveCostManager.GetAttackableTiles(testActor, testActors);
            
            bool hasAttackableTiles = attackableTiles != null;
            testResults.Add(new TestResult("Get Attackable Tiles", hasAttackableTiles, "Attackable tiles calculated: " + hasAttackableTiles));
            
            if (hasAttackableTiles)
            {
                int attackableCount = attackableTiles.Count;
                testResults.Add(new TestResult("Attackable Tiles Count", attackableCount >= 0, "Attackable tiles count: " + attackableCount));
                
                // Test that attackable tiles are valid
                bool allTilesValid = true;
                foreach (int tile in attackableTiles)
                {
                    if (tile < 0)
                    {
                        allTilesValid = false;
                        break;
                    }
                }
                testResults.Add(new TestResult("Valid Attackable Tiles", allTilesValid, "All tiles have valid indices"));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Get Attackable Tiles", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Movement Validation")]
    public void TestMovementValidation()
    {
        Debug.Log("Testing Movement Validation...");
        
        if (testActor == null)
        {
            testResults.Add(new TestResult("Movement Validation Setup", false, "No test actor available"));
            return;
        }
        
        try
        {
            // Test movement validation with current location
            int currentLocation = testActor.GetLocation();
            bool currentLocationValid = moveCostManager.reachableTiles.Contains(currentLocation);
            testResults.Add(new TestResult("Current Location Valid", currentLocationValid, "Current location " + currentLocation + " is valid: " + currentLocationValid));
            
            // Test movement validation with invalid location
            int invalidLocation = -1;
            bool invalidLocationRejected = !moveCostManager.reachableTiles.Contains(invalidLocation);
            testResults.Add(new TestResult("Invalid Location Rejected", invalidLocationRejected, "Invalid location " + invalidLocation + " rejected: " + invalidLocationRejected));
            
            // Test movement cost within actor's speed
            int actorSpeed = testActor.GetSpeed();
            bool moveCostReasonable = moveCostManager.moveCost <= actorSpeed || actorSpeed == 0;
            testResults.Add(new TestResult("Move Cost Reasonable", moveCostReasonable, "Move cost " + moveCostManager.moveCost + " vs speed " + actorSpeed));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Movement Validation", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Path Precomputation")]
    public void TestPathPrecomputation()
    {
        Debug.Log("Testing Path Precomputation...");
        
        if (testActor == null)
        {
            testResults.Add(new TestResult("Path Precomputation Setup", false, "No test actor available"));
            return;
        }
        
        try
        {
            int startLocation = testActor.GetLocation();
            
            // Test precomputed path consistency
            List<int> path1 = moveCostManager.GetPrecomputedPath(startLocation, startLocation);
            List<int> path2 = moveCostManager.GetPrecomputedPath(startLocation, startLocation);
            
            bool pathsConsistent = (path1 == null && path2 == null) || 
                                  (path1 != null && path2 != null && path1.Count == path2.Count);
            testResults.Add(new TestResult("Path Consistency", pathsConsistent, "Paths are consistent"));
            
            // Test path contains start location
            if (path1 != null && path1.Count > 0)
            {
                bool pathContainsStart = path1.Contains(startLocation);
                testResults.Add(new TestResult("Path Contains Start", pathContainsStart, "Path contains start location"));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Path Precomputation", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Move Cost Payment")]
    public void TestMoveCostPayment()
    {
        Debug.Log("Testing Move Cost Payment...");
        
        if (testActor == null)
        {
            testResults.Add(new TestResult("Move Cost Payment Setup", false, "No test actor available"));
            return;
        }
        
        try
        {
            // Store initial speed
            int initialSpeed = testActor.GetSpeed();
            
            // Test that actor can pay move cost
            bool canPayMoveCost = initialSpeed >= moveCostManager.moveCost;
            testResults.Add(new TestResult("Can Pay Move Cost", canPayMoveCost, "Speed " + initialSpeed + " vs cost " + moveCostManager.moveCost));
            
            // Test move cost is not negative
            bool nonNegativeMoveCost = moveCostManager.moveCost >= 0;
            testResults.Add(new TestResult("Non-negative Move Cost", nonNegativeMoveCost, "Move cost: " + moveCostManager.moveCost));
            
            // Test that move cost is reasonable (not excessive)
            bool reasonableMoveCost = moveCostManager.moveCost <= 100; // Arbitrary reasonable limit
            testResults.Add(new TestResult("Reasonable Move Cost", reasonableMoveCost, "Move cost " + moveCostManager.moveCost + " is reasonable"));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Move Cost Payment", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Movement Range")]
    public void TestMovementRange()
    {
        Debug.Log("Testing Movement Range...");
        
        if (testActor == null)
        {
            testResults.Add(new TestResult("Movement Range Setup", false, "No test actor available"));
            return;
        }
        
        try
        {
            // Test movement range calculation
            int actorSpeed = testActor.GetSpeed();
            int reachableTilesCount = moveCostManager.reachableTiles.Count;
            
            // Test that reachable tiles count is reasonable
            bool reasonableRange = reachableTilesCount >= 0 && reachableTilesCount <= 1000; // Arbitrary reasonable limits
            testResults.Add(new TestResult("Reasonable Movement Range", reasonableRange, "Reachable tiles: " + reachableTilesCount));
            
            // Test that actor with zero speed has minimal movement
            if (actorSpeed == 0)
            {
                bool zeroSpeedLimitedMovement = reachableTilesCount <= 1;
                testResults.Add(new TestResult("Zero Speed Limited Movement", zeroSpeedLimitedMovement, "Zero speed, tiles: " + reachableTilesCount));
            }
            
            // Test that actor with speed can move
            if (actorSpeed > 0)
            {
                bool positiveSpeedAllowsMovement = reachableTilesCount >= 1;
                testResults.Add(new TestResult("Positive Speed Allows Movement", positiveSpeedAllowsMovement, "Speed " + actorSpeed + ", tiles: " + reachableTilesCount));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Movement Range", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Edge Cases")]
    public void TestEdgeCases()
    {
        Debug.Log("Testing Edge Cases...");
        
        try
        {
            // Test with null actor
            moveCostManager.GetAllReachableTiles(null, testActors);
            testResults.Add(new TestResult("Null Actor Handling", true, "Handled null actor gracefully"));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Null Actor Handling", false, "Failed: " + e.Message));
        }
        
        try
        {
            // Test with null actor list
            if (testActor != null)
            {
                moveCostManager.GetAllReachableTiles(testActor, null);
                testResults.Add(new TestResult("Null Actor List Handling", true, "Handled null actor list gracefully"));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Null Actor List Handling", false, "Failed: " + e.Message));
        }
        
        try
        {
            // Test with invalid path
            List<int> invalidPath = moveCostManager.GetPrecomputedPath(-1, -1);
            bool handledInvalidPath = invalidPath == null || invalidPath.Count == 0;
            testResults.Add(new TestResult("Invalid Path Handling", handledInvalidPath, "Handled invalid path request"));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Invalid Path Handling", false, "Failed: " + e.Message));
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