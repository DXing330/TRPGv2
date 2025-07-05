using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestBoardTester : MonoBehaviour
{
    public RequestBoard requestBoard;
    public Request dummyRequest;
    public GuildCard testGuildCard;
    public SavedOverworld testOverworld;
    public PartyDataManager testPartyData;
    public Inventory testInventory;
    public SavedCaravan testCaravan;
    
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
    
    [ContextMenu("Run All Request Board Tests")]
    public void RunAllRequestBoardTests()
    {
        testResults.Clear();
        Debug.Log("=== Running Request Board Tests ===");
        
        TestRequestBoardInitialization();
        TestRequestGeneration();
        TestDeliveryRequests();
        TestDefeatRequests();
        TestEscortRequests();
        TestQuestAcceptance();
        TestQuestCompletion();
        TestQuestDisplay();
        TestLocationGeneration();
        TestRequestParameters();
        
        LogTestResults();
    }
    
    [ContextMenu("Test Request Board Initialization")]
    public void TestRequestBoardInitialization()
    {
        Debug.Log("Testing Request Board Initialization...");
        
        // Test component references
        bool hasUtility = requestBoard.utility != null;
        testResults.Add(new TestResult("General Utility", hasUtility, "Utility exists: " + hasUtility));
        
        bool hasOverworld = requestBoard.overworldTiles != null;
        testResults.Add(new TestResult("Overworld Tiles", hasOverworld, "Overworld exists: " + hasOverworld));
        
        bool hasOverworldState = requestBoard.overworldState != null;
        testResults.Add(new TestResult("Overworld State", hasOverworldState, "Overworld state exists: " + hasOverworldState));
        
        bool hasPartyData = requestBoard.partyData != null;
        testResults.Add(new TestResult("Party Data Manager", hasPartyData, "Party data exists: " + hasPartyData));
        
        bool hasInventory = requestBoard.inventory != null;
        testResults.Add(new TestResult("Inventory System", hasInventory, "Inventory exists: " + hasInventory));
        
        bool hasGuildCard = requestBoard.guildCard != null;
        testResults.Add(new TestResult("Guild Card", hasGuildCard, "Guild card exists: " + hasGuildCard));
        
        bool hasDummyRequest = requestBoard.dummyRequest != null;
        testResults.Add(new TestResult("Dummy Request", hasDummyRequest, "Dummy request exists: " + hasDummyRequest));
        
        bool hasRequestGoals = requestBoard.requestGoals != null && requestBoard.requestGoals.Count > 0;
        testResults.Add(new TestResult("Request Goals", hasRequestGoals, "Request goals count: " + (requestBoard.requestGoals?.Count ?? 0)));
        
        bool hasAvailableRequests = requestBoard.availableRequests != null;
        testResults.Add(new TestResult("Available Requests", hasAvailableRequests, "Available requests exists: " + hasAvailableRequests));
    }
    
    [ContextMenu("Test Request Generation")]
    public void TestRequestGeneration()
    {
        Debug.Log("Testing Request Generation...");
        
        if (requestBoard.availableRequests == null)
        {
            testResults.Add(new TestResult("Request Generation Setup", false, "No available requests list"));
            return;
        }
        
        int initialCount = requestBoard.availableRequests.Count;
        
        try
        {
            requestBoard.GenerateRequests();
            int newCount = requestBoard.availableRequests.Count;
            bool requestsGenerated = newCount > 0;
            testResults.Add(new TestResult("Generate Requests", requestsGenerated, $"Generated {newCount} requests (was {initialCount})"));
            
            // Test base request count
            bool hasBaseAmount = newCount >= requestBoard.baseRequestCount;
            testResults.Add(new TestResult("Base Request Count", hasBaseAmount, $"Expected: {requestBoard.baseRequestCount}, Got: {newCount}"));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Generate Requests", false, "Generation failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Delivery Requests")]
    public void TestDeliveryRequests()
    {
        Debug.Log("Testing Delivery Requests...");
        
        if (requestBoard.dummyRequest == null)
        {
            testResults.Add(new TestResult("Delivery Test Setup", false, "No dummy request available"));
            return;
        }
        
        try
        {
            // Reset dummy request
            requestBoard.dummyRequest.Reset();
            
            // Test delivery request generation
            requestBoard.dummyRequest.SetGoal("Deliver");
            bool deliveryGoalSet = requestBoard.dummyRequest.GetGoal() == "Deliver";
            testResults.Add(new TestResult("Set Delivery Goal", deliveryGoalSet, "Goal: " + requestBoard.dummyRequest.GetGoal()));
            
            // Test delivery specifics
            string testLuxury = "Silk";
            requestBoard.dummyRequest.SetGoalSpecifics(testLuxury);
            bool luxurySet = requestBoard.dummyRequest.GetGoalSpecifics() == testLuxury;
            testResults.Add(new TestResult("Set Delivery Luxury", luxurySet, "Luxury: " + requestBoard.dummyRequest.GetGoalSpecifics()));
            
            // Test delivery amount
            int testAmount = 5;
            requestBoard.dummyRequest.SetGoalAmount(testAmount);
            bool amountSet = requestBoard.dummyRequest.GetGoalAmount() == testAmount;
            testResults.Add(new TestResult("Set Delivery Amount", amountSet, "Amount: " + requestBoard.dummyRequest.GetGoalAmount()));
            
            // Test reward calculation
            int testReward = 100;
            requestBoard.dummyRequest.SetReward(testReward);
            bool rewardSet = requestBoard.dummyRequest.GetReward() == testReward;
            testResults.Add(new TestResult("Set Delivery Reward", rewardSet, "Reward: " + requestBoard.dummyRequest.GetReward()));
            
            // Test fail penalty
            int testPenalty = 50;
            requestBoard.dummyRequest.SetFailPenalty(testPenalty);
            bool penaltySet = requestBoard.dummyRequest.GetFailPenalty() == testPenalty;
            testResults.Add(new TestResult("Set Delivery Penalty", penaltySet, "Penalty: " + requestBoard.dummyRequest.GetFailPenalty()));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Delivery Request Creation", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Defeat Requests")]
    public void TestDefeatRequests()
    {
        Debug.Log("Testing Defeat Requests...");
        
        if (requestBoard.dummyRequest == null)
        {
            testResults.Add(new TestResult("Defeat Test Setup", false, "No dummy request available"));
            return;
        }
        
        try
        {
            // Reset dummy request
            requestBoard.dummyRequest.Reset();
            
            // Test defeat request setup
            requestBoard.dummyRequest.SetGoal("Defeat");
            bool defeatGoalSet = requestBoard.dummyRequest.GetGoal() == "Defeat";
            testResults.Add(new TestResult("Set Defeat Goal", defeatGoalSet, "Goal: " + requestBoard.dummyRequest.GetGoal()));
            
            // Test defeat location
            requestBoard.dummyRequest.SetLocationSpecifics(requestBoard.defeatFeatureString);
            bool locationSet = requestBoard.dummyRequest.GetLocationSpecifics() == requestBoard.defeatFeatureString;
            testResults.Add(new TestResult("Set Defeat Location", locationSet, "Location: " + requestBoard.dummyRequest.GetLocationSpecifics()));
            
            // Test defeat location tile
            int testLocation = 100;
            requestBoard.dummyRequest.SetLocation(testLocation);
            bool locationTileSet = requestBoard.dummyRequest.GetLocation() == testLocation;
            testResults.Add(new TestResult("Set Defeat Location Tile", locationTileSet, "Tile: " + requestBoard.dummyRequest.GetLocation()));
            
            // Test defeat deadline
            int testDeadline = 7;
            requestBoard.dummyRequest.SetDeadline(testDeadline);
            bool deadlineSet = requestBoard.dummyRequest.GetDeadline() == testDeadline;
            testResults.Add(new TestResult("Set Defeat Deadline", deadlineSet, "Deadline: " + requestBoard.dummyRequest.GetDeadline()));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Defeat Request Creation", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Escort Requests")]
    public void TestEscortRequests()
    {
        Debug.Log("Testing Escort Requests...");
        
        if (requestBoard.dummyRequest == null)
        {
            testResults.Add(new TestResult("Escort Test Setup", false, "No dummy request available"));
            return;
        }
        
        try
        {
            // Reset dummy request
            requestBoard.dummyRequest.Reset();
            
            // Test escort request setup
            requestBoard.dummyRequest.SetGoal("Escort");
            bool escortGoalSet = requestBoard.dummyRequest.GetGoal() == "Escort";
            testResults.Add(new TestResult("Set Escort Goal", escortGoalSet, "Goal: " + requestBoard.dummyRequest.GetGoal()));
            
            // Test escort location
            requestBoard.dummyRequest.SetLocationSpecifics(requestBoard.escortFeatureString);
            bool locationSet = requestBoard.dummyRequest.GetLocationSpecifics() == requestBoard.escortFeatureString;
            testResults.Add(new TestResult("Set Escort Location", locationSet, "Location: " + requestBoard.dummyRequest.GetLocationSpecifics()));
            
            // Test escort target
            requestBoard.dummyRequest.SetGoalSpecifics(requestBoard.escortTempMember);
            bool targetSet = requestBoard.dummyRequest.GetGoalSpecifics() == requestBoard.escortTempMember;
            testResults.Add(new TestResult("Set Escort Target", targetSet, "Target: " + requestBoard.dummyRequest.GetGoalSpecifics()));
            
            // Test escort penalty
            requestBoard.dummyRequest.SetFailPenalty(requestBoard.escortFailPenalty);
            bool penaltySet = requestBoard.dummyRequest.GetFailPenalty() == requestBoard.escortFailPenalty;
            testResults.Add(new TestResult("Set Escort Penalty", penaltySet, "Penalty: " + requestBoard.dummyRequest.GetFailPenalty()));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Escort Request Creation", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Quest Acceptance")]
    public void TestQuestAcceptance()
    {
        Debug.Log("Testing Quest Acceptance...");
        
        if (requestBoard.guildCard == null)
        {
            testResults.Add(new TestResult("Quest Acceptance Setup", false, "No guild card available"));
            return;
        }
        
        // First ensure we have quests available
        if (requestBoard.availableRequests == null || requestBoard.availableRequests.Count == 0)
        {
            requestBoard.GenerateRequests();
        }
        
        int initialAcceptedCount = requestBoard.guildCard.acceptedQuests.Count;
        int initialAvailableCount = requestBoard.availableRequests.Count;
        
        try
        {
            // Select the first quest if available
            if (requestBoard.questSelect != null && initialAvailableCount > 0)
            {
                requestBoard.questSelect.SetSelected(0);
                requestBoard.AcceptQuest();
                
                int newAcceptedCount = requestBoard.guildCard.acceptedQuests.Count;
                int newAvailableCount = requestBoard.availableRequests.Count;
                
                bool questAccepted = newAcceptedCount > initialAcceptedCount;
                testResults.Add(new TestResult("Accept Quest", questAccepted, $"Accepted: {initialAcceptedCount} -> {newAcceptedCount}"));
                
                bool availableReduced = newAvailableCount < initialAvailableCount;
                testResults.Add(new TestResult("Available Quests Reduced", availableReduced, $"Available: {initialAvailableCount} -> {newAvailableCount}"));
            }
            else
            {
                testResults.Add(new TestResult("Quest Selection", false, "No quest selector or available quests"));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Quest Acceptance", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Quest Completion")]
    public void TestQuestCompletion()
    {
        Debug.Log("Testing Quest Completion...");
        
        if (requestBoard.guildCard == null)
        {
            testResults.Add(new TestResult("Quest Completion Setup", false, "No guild card available"));
            return;
        }
        
        try
        {
            // Test quest completion check
            requestBoard.TryToCompleteQuest();
            testResults.Add(new TestResult("Try Complete Quest", true, "Quest completion check executed"));
            
            // Test with no accepted quests
            if (requestBoard.guildCard.acceptedQuests.Count == 0)
            {
                testResults.Add(new TestResult("No Accepted Quests", true, "Handled case with no accepted quests"));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Quest Completion", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Quest Display")]
    public void TestQuestDisplay()
    {
        Debug.Log("Testing Quest Display...");
        
        if (requestBoard.questSelect == null)
        {
            testResults.Add(new TestResult("Quest Display Setup", false, "No quest selector available"));
            return;
        }
        
        try
        {
            // Test quest list update
            requestBoard.UpdateSelectableQuests();
            testResults.Add(new TestResult("Update Selectable Quests", true, "Quest list updated"));
            
            // Test quest description display
            if (requestBoard.availableRequests != null && requestBoard.availableRequests.Count > 0)
            {
                requestBoard.questSelect.SetSelected(0);
                requestBoard.DisplayRequestDescription();
                testResults.Add(new TestResult("Display Quest Description", true, "Quest description displayed"));
            }
            
            // Test quest details reset
            requestBoard.ResetQuestDetails();
            testResults.Add(new TestResult("Reset Quest Details", true, "Quest details reset"));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Quest Display", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Location Generation")]
    public void TestLocationGeneration()
    {
        Debug.Log("Testing Location Generation...");
        
        if (requestBoard.overworldTiles == null)
        {
            testResults.Add(new TestResult("Location Generation Setup", false, "No overworld tiles available"));
            return;
        }
        
        try
        {
            // Test distance variation
            bool hasDistanceVariation = requestBoard.distanceVariation > 0;
            testResults.Add(new TestResult("Distance Variation", hasDistanceVariation, "Distance variation: " + requestBoard.distanceVariation));
            
            // Test amount variation
            bool hasAmountVariation = requestBoard.amountVariation > 0;
            testResults.Add(new TestResult("Amount Variation", hasAmountVariation, "Amount variation: " + requestBoard.amountVariation));
            
            // Test feature strings
            bool hasDeliverFeature = !string.IsNullOrEmpty(requestBoard.deliverFeatureString);
            testResults.Add(new TestResult("Deliver Feature String", hasDeliverFeature, "Deliver feature: " + requestBoard.deliverFeatureString));
            
            bool hasDefeatFeature = !string.IsNullOrEmpty(requestBoard.defeatFeatureString);
            testResults.Add(new TestResult("Defeat Feature String", hasDefeatFeature, "Defeat feature: " + requestBoard.defeatFeatureString));
            
            bool hasEscortFeature = !string.IsNullOrEmpty(requestBoard.escortFeatureString);
            testResults.Add(new TestResult("Escort Feature String", hasEscortFeature, "Escort feature: " + requestBoard.escortFeatureString));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Location Generation", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Request Parameters")]
    public void TestRequestParameters()
    {
        Debug.Log("Testing Request Parameters...");
        
        // Test base request count
        bool hasValidBaseCount = requestBoard.baseRequestCount > 0;
        testResults.Add(new TestResult("Base Request Count", hasValidBaseCount, "Base count: " + requestBoard.baseRequestCount));
        
        // Test amount variation
        bool hasValidAmountVariation = requestBoard.amountVariation >= 0;
        testResults.Add(new TestResult("Amount Variation Valid", hasValidAmountVariation, "Amount variation: " + requestBoard.amountVariation));
        
        // Test distance variation
        bool hasValidDistanceVariation = requestBoard.distanceVariation >= 0;
        testResults.Add(new TestResult("Distance Variation Valid", hasValidDistanceVariation, "Distance variation: " + requestBoard.distanceVariation));
        
        // Test escort fail penalty
        bool hasValidEscortPenalty = requestBoard.escortFailPenalty > 0;
        testResults.Add(new TestResult("Escort Fail Penalty", hasValidEscortPenalty, "Escort penalty: " + requestBoard.escortFailPenalty));
        
        // Test escort temp member
        bool hasValidEscortMember = !string.IsNullOrEmpty(requestBoard.escortTempMember);
        testResults.Add(new TestResult("Escort Temp Member", hasValidEscortMember, "Escort member: " + requestBoard.escortTempMember));
        
        // Test request goals
        if (requestBoard.requestGoals != null)
        {
            bool hasDeliverGoal = requestBoard.requestGoals.Contains("Deliver");
            testResults.Add(new TestResult("Has Deliver Goal", hasDeliverGoal, "Deliver goal available: " + hasDeliverGoal));
            
            bool hasDefeatGoal = requestBoard.requestGoals.Contains("Defeat");
            testResults.Add(new TestResult("Has Defeat Goal", hasDefeatGoal, "Defeat goal available: " + hasDefeatGoal));
            
            bool hasEscortGoal = requestBoard.requestGoals.Contains("Escort");
            testResults.Add(new TestResult("Has Escort Goal", hasEscortGoal, "Escort goal available: " + hasEscortGoal));
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