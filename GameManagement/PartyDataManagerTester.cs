using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyDataManagerTester : MonoBehaviour
{
    public PartyDataManager partyDataManager;
    public TacticActor dummyActor;
    public CharacterList fullParty;
    public StatDatabase testActorStats;
    public Equipment testEquipment;

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

    [ContextMenu("Reset")]
    public void ResetTestResults()
    {
        testResults = new List<TestResult>();
    }

    public List<TestResult> testResults = new List<TestResult>();

    [ContextMenu("TestUpdatePartyAfterBattle")]
    public void TestUpdatePartyAfterBattle()
    {
        // Load the original party.
        partyDataManager.Load();
        // Set up a dummy party.
        List<string> dummySprites = new List<string> { "Knight", "Knight" };
        List<string> dummyStats = new List<string> { "60|5|10|1|4|2|Walking|1|10|Knight|1||||", "60|5|10|1|4|2|Walking|1|10|Knight|1||||" };
        List<string> dummyNames = new List<string> { "Knight1", "Knight2" };
        // Add the dummy party.
        for (int i = 0; i < dummySprites.Count; i++)
        {
            partyDataManager.HireMember(dummySprites[i], dummyStats[i], dummyNames[i]);
        }
        int originalPartyCount = partyDataManager.ReturnTotalPartyCount();
        // Set up some random after battle stats.
        List<int> currentHealths = new List<int>();
        List<string> currentNames = new List<string>();
        List<string> currentSpriteNames = new List<string>();
        List<string> currentStats = new List<string>();
        for (int i = 0; i < fullParty.characters.Count; i++)
        {
            dummyActor.SetStatsFromString(fullParty.stats[i]);
            int randomHealth = Random.Range(1, dummyActor.GetBaseHealth());
            dummyActor.SetCurrentHealth(randomHealth);
            currentHealths.Add(randomHealth);
            currentNames.Add(fullParty.characterNames[i]);
            currentSpriteNames.Add(fullParty.characters[i]);
            currentStats.Add(dummyActor.ReturnPersistentStats());
        }
        // Try to load them back in.
        partyDataManager.UpdatePartyAfterBattle(currentNames, currentSpriteNames, currentStats);
        // Check if all members loaded. (Completeness)
        testResults.Add(new TestResult("PartyUpdatedCompletelyAfterBattle", partyDataManager.ReturnTotalPartyCount() == originalPartyCount, "Original: "+originalPartyCount+"; Current: "+partyDataManager.ReturnTotalPartyCount()));
        // Check if all stats loaded. (Accuracy)
        for (int i = 0; i < partyDataManager.ReturnTotalPartyCount(); i++)
        {
            testResults.Add(new TestResult("Member At Index "+i+" Loaded Correctly", partyDataManager.ReturnPartyMemberCurrentHealthFromIndex(i) == currentHealths[i], "Original: "+currentHealths[i]+"; Current: "+partyDataManager.ReturnPartyMemberCurrentHealthFromIndex(i)));
        }
    }
    
    /*[ContextMenu("Run All Party Data Tests")]
    public void RunAllPartyDataTests()
    {
        testResults.Clear();
        Debug.Log("=== Running Party Data Manager Tests ===");
        
        TestPartyInitialization();
        TestPartyMemberManagement();
        TestEquipmentManagement();
        TestPartyStats();
        TestTempPartyMembers();
        TestPartyCapacity();
        TestPartyResting();
        TestPartyExhaustion();
        TestBattleUpdate();
        
        LogTestResults();
    }
    
    [ContextMenu("Test Party Initialization")]
    public void TestPartyInitialization()
    {
        Debug.Log("Testing Party Initialization...");
        
        // Test party data manager components
        bool hasActorStats = partyDataManager.actorStats != null;
        testResults.Add(new TestResult("Actor Stats Database", hasActorStats, "ActorStats exists: " + hasActorStats));
        
        bool hasFullParty = partyDataManager.fullParty != null;
        testResults.Add(new TestResult("Full Party List", hasFullParty, "FullParty exists: " + hasFullParty));
        
        bool hasAllParties = partyDataManager.allParties != null;
        testResults.Add(new TestResult("All Parties List", hasAllParties, "AllParties exists: " + hasAllParties));
        
        bool hasPermanentParty = partyDataManager.permanentPartyData != null;
        testResults.Add(new TestResult("Permanent Party Data", hasPermanentParty, "PermanentParty exists: " + hasPermanentParty));
        
        bool hasMainParty = partyDataManager.mainPartyData != null;
        testResults.Add(new TestResult("Main Party Data", hasMainParty, "MainParty exists: " + hasMainParty));
        
        bool hasTempParty = partyDataManager.tempPartyData != null;
        testResults.Add(new TestResult("Temp Party Data", hasTempParty, "TempParty exists: " + hasTempParty));
        
        bool hasInventory = partyDataManager.inventory != null;
        testResults.Add(new TestResult("Inventory System", hasInventory, "Inventory exists: " + hasInventory));
        
        bool hasEquipmentInventory = partyDataManager.equipmentInventory != null;
        testResults.Add(new TestResult("Equipment Inventory", hasEquipmentInventory, "EquipmentInventory exists: " + hasEquipmentInventory));
    }
    
    [ContextMenu("Test Party Member Management")]
    public void TestPartyMemberManagement()
    {
        Debug.Log("Testing Party Member Management...");
        
        if (partyDataManager.mainPartyData == null)
        {
            testResults.Add(new TestResult("Party Member Test Setup", false, "No main party data available"));
            return;
        }
        
        // Test initial party count
        int initialCount = partyDataManager.mainPartyData.PartyCount();
        testResults.Add(new TestResult("Initial Party Count", initialCount >= 0, "Initial count: " + initialCount));
        
        // Test adding party member
        if (testActorStats != null)
        {
            string testName = "TestHero";
            string testStats = "TestStats";
            string testPersonalName = "TestPersonal";
            string testFee = "100";
            
            partyDataManager.HireMember(testName, testStats, testPersonalName, testFee);
            int newCount = partyDataManager.mainPartyData.PartyCount();
            bool memberAdded = newCount > initialCount;
            testResults.Add(new TestResult("Hire Party Member", memberAdded, $"Count: {initialCount} -> {newCount}"));
        }
        
        // Test party capacity
        bool hasOpenSlots = partyDataManager.OpenSlots();
        testResults.Add(new TestResult("Open Slots Check", true, "Has open slots: " + hasOpenSlots));
    }
    
    [ContextMenu("Test Equipment Management")]
    public void TestEquipmentManagement()
    {
        Debug.Log("Testing Equipment Management...");
        
        if (partyDataManager.fullParty == null)
        {
            testResults.Add(new TestResult("Equipment Test Setup", false, "No full party available"));
            return;
        }
        
        int partyCount = partyDataManager.fullParty.characters.Count;
        testResults.Add(new TestResult("Party Count for Equipment", partyCount >= 0, "Party count: " + partyCount));
        
        if (partyCount > 0 && testEquipment != null)
        {
            // Test equipment assignment
            string testEquipName = "Sword|Weapon|Sword|Attack Up|1|1";
            string equipResult = partyDataManager.EquipToPartyMember(testEquipName, 0, testEquipment);
            bool equipSuccessful = !string.IsNullOrEmpty(equipResult);
            testResults.Add(new TestResult("Equip to Party Member", equipSuccessful, "Equip result: " + equipResult));
            
            // Test equipment retrieval
            string currentEquip = partyDataManager.ReturnPartyMemberEquipFromIndex(0);
            bool hasEquipment = !string.IsNullOrEmpty(currentEquip);
            testResults.Add(new TestResult("Get Party Member Equipment", hasEquipment, "Equipment: " + currentEquip));
            
            // Test unequipping
            string unequipResult = partyDataManager.UnequipFromPartyMember(0, "Weapon", testEquipment);
            bool unequipSuccessful = !string.IsNullOrEmpty(unequipResult);
            testResults.Add(new TestResult("Unequip from Party Member", unequipSuccessful, "Unequip result: " + unequipResult));
        }
    }
    
    [ContextMenu("Test Party Stats")]
    public void TestPartyStats()
    {
        Debug.Log("Testing Party Stats...");
        
        if (partyDataManager.fullParty == null)
        {
            testResults.Add(new TestResult("Party Stats Test Setup", false, "No full party available"));
            return;
        }
        
        // Test party stats lists
        bool hasCharacters = partyDataManager.fullParty.characters != null;
        testResults.Add(new TestResult("Characters List", hasCharacters, "Characters list exists: " + hasCharacters));
        
        bool hasStats = partyDataManager.fullParty.stats != null;
        testResults.Add(new TestResult("Stats List", hasStats, "Stats list exists: " + hasStats));
        
        bool hasNames = partyDataManager.fullParty.characterNames != null;
        testResults.Add(new TestResult("Character Names List", hasNames, "Names list exists: " + hasNames));
        
        bool hasEquipment = partyDataManager.fullParty.equipment != null;
        testResults.Add(new TestResult("Equipment List", hasEquipment, "Equipment list exists: " + hasEquipment));
        
    }
    
    [ContextMenu("Test Temp Party Members")]
    public void TestTempPartyMembers()
    {
        Debug.Log("Testing Temp Party Members...");
        
        if (partyDataManager.tempPartyData == null)
        {
            testResults.Add(new TestResult("Temp Party Test Setup", false, "No temp party data available"));
            return;
        }
        
        string testTempMember = "TempHero";
        int initialTempCount = partyDataManager.tempPartyData.PartyCount();
        
        // Test adding temp member
        partyDataManager.AddTempPartyMember(testTempMember);
        int newTempCount = partyDataManager.tempPartyData.PartyCount();
        bool tempMemberAdded = newTempCount > initialTempCount;
        testResults.Add(new TestResult("Add Temp Party Member", tempMemberAdded, $"Count: {initialTempCount} -> {newTempCount}"));
        
        // Test temp member existence check
        bool tempMemberExists = partyDataManager.TempPartyMemberExists(testTempMember);
        testResults.Add(new TestResult("Temp Member Exists Check", tempMemberExists, "Temp member exists: " + tempMemberExists));
        
        // Test removing temp member
        partyDataManager.RemoveTempPartyMember(testTempMember);
        int finalTempCount = partyDataManager.tempPartyData.PartyCount();
        bool tempMemberRemoved = finalTempCount == initialTempCount;
        testResults.Add(new TestResult("Remove Temp Party Member", tempMemberRemoved, $"Count: {newTempCount} -> {finalTempCount}"));
    }
    
    [ContextMenu("Test Party Capacity")]
    public void TestPartyCapacity()
    {
        Debug.Log("Testing Party Capacity...");
        
        if (partyDataManager.guildCard == null)
        {
            testResults.Add(new TestResult("Party Capacity Test Setup", false, "No guild card available"));
            return;
        }
        
        // Test open slots calculation
        bool hasOpenSlots = partyDataManager.OpenSlots();
        testResults.Add(new TestResult("Open Slots Available", true, "Open slots: " + hasOpenSlots));
        
        // Test guild rank influence on capacity
        int guildRank = partyDataManager.guildCard.GetGuildRank();
        testResults.Add(new TestResult("Guild Rank Retrieved", guildRank >= 0, "Guild rank: " + guildRank));
        
        int mainPartyCount = partyDataManager.mainPartyData.PartyCount();
        int expectedCapacity = guildRank + 2; // Base capacity formula
        bool capacityLogic = hasOpenSlots == (mainPartyCount < expectedCapacity);
        testResults.Add(new TestResult("Capacity Logic", capacityLogic, $"Main party: {mainPartyCount}, Expected capacity: {expectedCapacity}"));
    }
    
    [ContextMenu("Test Party Resting")]
    public void TestPartyResting()
    {
        Debug.Log("Testing Party Resting...");
        
        if (partyDataManager.caravan == null)
        {
            testResults.Add(new TestResult("Resting Test Setup", false, "No caravan available"));
            return;
        }
        
        try
        {
            partyDataManager.Rest();
            testResults.Add(new TestResult("Party Rest", true, "Party rested successfully"));
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Party Rest", false, "Rest failed: " + e.Message));
        }
        
        // Test exhaustion removal
        try
        {
            partyDataManager.RemoveExhaustion();
            testResults.Add(new TestResult("Remove Exhaustion", true, "Exhaustion removed successfully"));
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Remove Exhaustion", false, "Exhaustion removal failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Party Exhaustion")]
    public void TestPartyExhaustion()
    {
        Debug.Log("Testing Party Exhaustion...");
        
        try
        {
            partyDataManager.NewDay(1);
            testResults.Add(new TestResult("New Day Processing", true, "New day processed successfully"));
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("New Day Processing", false, "New day failed: " + e.Message));
        }
        
        try
        {
            partyDataManager.AddHours(4);
            testResults.Add(new TestResult("Add Hours", true, "Hours added successfully"));
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Add Hours", false, "Add hours failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Battle Update")]
    public void TestBattleUpdate()
    {
        Debug.Log("Testing Battle Update...");
        
        // Test battle result processing
        List<string> testNames = new List<string> { "Hero1", "Hero2" };
        List<string> testStats = new List<string> { "100/100/50", "80/80/40" };
        
        try
        {
            //partyDataManager.UpdatePartyAfterBattle(testNames, testStats);
            testResults.Add(new TestResult("Update Party After Battle", true, "Battle update completed"));
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Update Party After Battle", false, "Battle update failed: " + e.Message));
        }
        
        // Test party defeat handling
        try
        {
            partyDataManager.PartyDefeated();
            testResults.Add(new TestResult("Party Defeated", true, "Party defeat handled successfully"));
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Party Defeated", false, "Party defeat failed: " + e.Message));
        }
        
        // Test full party update
        try
        {
            partyDataManager.SetFullParty();
            testResults.Add(new TestResult("Set Full Party", true, "Full party set successfully"));
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Set Full Party", false, "Set full party failed: " + e.Message));
        }
    }

    [ContextMenu("Test Guild Card")]
    public void TestGuildCard()
    {
        Debug.Log("Testing Guild Card...");
        // Test if a quest can be added properly.
        //string dummyQuestString = "";
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
*/
}