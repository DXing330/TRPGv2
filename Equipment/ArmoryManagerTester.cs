using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoryManagerTester : MonoBehaviour
{
    public ArmoryManager armoryManager;
    public ArmoryUI armoryUI;
    public Equipment testEquipment;
    public EquipmentInventory testInventory;
    public StatDatabase equipmentDatabase;
    
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
    
    [ContextMenu("Run All Equipment Tests")]
    public void RunAllEquipmentTests()
    {
        testResults.Clear();
        Debug.Log("=== Running Equipment System Tests ===");
        
        TestEquipmentInitialization();
        TestEquipmentProperties();
        TestEquipmentInventory();
        TestEquipmentDatabase();
        TestEquipmentStats();
        TestEquipmentEquipping();
        TestEquipmentUI();
        TestEquipmentValidation();
        TestEquipmentSerialization();
        TestEquipmentEdgeCases();
        
        LogTestResults();
    }
    
    [ContextMenu("Test Equipment Initialization")]
    public void TestEquipmentInitialization()
    {
        Debug.Log("Testing Equipment Initialization...");
        
        // Test armory manager
        bool hasArmoryManager = armoryManager != null;
        testResults.Add(new TestResult("Armory Manager", hasArmoryManager, "Armory manager exists: " + hasArmoryManager));
        
        // Test armory UI
        bool hasArmoryUI = armoryUI != null;
        testResults.Add(new TestResult("Armory UI", hasArmoryUI, "Armory UI exists: " + hasArmoryUI));
        
        // Test equipment object
        bool hasEquipment = testEquipment != null;
        testResults.Add(new TestResult("Equipment Object", hasEquipment, "Equipment exists: " + hasEquipment));
        
        // Test equipment inventory
        bool hasInventory = testInventory != null;
        testResults.Add(new TestResult("Equipment Inventory", hasInventory, "Equipment inventory exists: " + hasInventory));
        
        // Test equipment database
        bool hasDatabase = equipmentDatabase != null;
        testResults.Add(new TestResult("Equipment Database", hasDatabase, "Equipment database exists: " + hasDatabase));
    }
    
    [ContextMenu("Test Equipment Properties")]
    public void TestEquipmentProperties()
    {
        Debug.Log("Testing Equipment Properties...");
        
        if (testEquipment == null)
        {
            testResults.Add(new TestResult("Equipment Properties Setup", false, "No test equipment available"));
            return;
        }
        
        try
        {
            // Test equipment name
            string equipName = testEquipment.GetName();
            bool hasName = !string.IsNullOrEmpty(equipName);
            testResults.Add(new TestResult("Equipment Name", hasName, "Equipment name: " + equipName));
            
            // Test equipment type
            string equipType = testEquipment.GetType().Name;
            bool hasType = !string.IsNullOrEmpty(equipType);
            testResults.Add(new TestResult("Equipment Type", hasType, "Equipment type: " + equipType));
            
            // Test equipment stats
            string equipStats = testEquipment.GetStats();
            bool hasStats = equipStats != null;
            testResults.Add(new TestResult("Equipment Stats", hasStats, "Equipment stats: " + equipStats));
            
            // Test equipment rarity
            string equipRarity = testEquipment.GetRarity();
            bool hasRarity = !string.IsNullOrEmpty(equipRarity);
            testResults.Add(new TestResult("Equipment Rarity", hasRarity, "Equipment rarity: " + equipRarity));
            
            // Test equipment slot
            string equipSlot = testEquipment.GetSlot();
            bool hasSlot = !string.IsNullOrEmpty(equipSlot);
            testResults.Add(new TestResult("Equipment Slot", hasSlot, "Equipment slot: " + equipSlot));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment Properties", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Equipment Inventory")]
    public void TestEquipmentInventory()
    {
        Debug.Log("Testing Equipment Inventory...");
        
        if (testInventory == null)
        {
            testResults.Add(new TestResult("Equipment Inventory Setup", false, "No test inventory available"));
            return;
        }
        
        try
        {
            // Test initial inventory state
            int initialCount = testInventory.GetEquipmentCount();
            testResults.Add(new TestResult("Initial Equipment Count", initialCount >= 0, "Initial count: " + initialCount));
            
            // Test adding equipment
            string testEquipName = "TestSword";
            if (equipmentDatabase != null)
            {
                testInventory.AddEquipmentByName(testEquipName);
                int newCount = testInventory.GetEquipmentCount();
                bool equipmentAdded = newCount > initialCount;
                testResults.Add(new TestResult("Add Equipment", equipmentAdded, $"Count: {initialCount} -> {newCount}"));
            }
            
            // Test equipment existence check
            bool equipmentExists = testInventory.EquipmentExists(testEquipName);
            testResults.Add(new TestResult("Equipment Exists Check", equipmentExists, "Equipment exists: " + equipmentExists));
            
            // Test equipment removal
            if (equipmentExists)
            {
                testInventory.RemoveEquipment(testEquipName);
                int finalCount = testInventory.GetEquipmentCount();
                bool equipmentRemoved = finalCount < newCount;
                testResults.Add(new TestResult("Remove Equipment", equipmentRemoved, $"Count after removal: {finalCount}"));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment Inventory", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Equipment Database")]
    public void TestEquipmentDatabase()
    {
        Debug.Log("Testing Equipment Database...");
        
        if (equipmentDatabase == null)
        {
            testResults.Add(new TestResult("Equipment Database Setup", false, "No equipment database available"));
            return;
        }
        
        try
        {
            // Test database has entries
            bool hasEntries = equipmentDatabase.GetEntryCount() > 0;
            testResults.Add(new TestResult("Database Has Entries", hasEntries, "Entry count: " + equipmentDatabase.GetEntryCount()));
            
            // Test retrieving equipment stats
            string testEquipName = "Sword";
            string equipStats = equipmentDatabase.ReturnValue(testEquipName);
            bool hasValidStats = !string.IsNullOrEmpty(equipStats);
            testResults.Add(new TestResult("Retrieve Equipment Stats", hasValidStats, "Stats for " + testEquipName + ": " + equipStats));
            
            // Test equipment type lookup
            string equipType = equipmentDatabase.ReturnType(testEquipName);
            bool hasValidType = !string.IsNullOrEmpty(equipType);
            testResults.Add(new TestResult("Retrieve Equipment Type", hasValidType, "Type for " + testEquipName + ": " + equipType));
            
            // Test equipment rarity lookup
            string equipRarity = equipmentDatabase.ReturnRarity(testEquipName);
            bool hasValidRarity = !string.IsNullOrEmpty(equipRarity);
            testResults.Add(new TestResult("Retrieve Equipment Rarity", hasValidRarity, "Rarity for " + testEquipName + ": " + equipRarity));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment Database", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Equipment Stats")]
    public void TestEquipmentStats()
    {
        Debug.Log("Testing Equipment Stats...");
        
        if (testEquipment == null)
        {
            testResults.Add(new TestResult("Equipment Stats Setup", false, "No test equipment available"));
            return;
        }
        
        try
        {
            // Test stat parsing
            string rawStats = testEquipment.GetStats();
            bool hasRawStats = !string.IsNullOrEmpty(rawStats);
            testResults.Add(new TestResult("Raw Stats", hasRawStats, "Raw stats: " + rawStats));
            
            // Test individual stat retrieval
            int attackBonus = testEquipment.GetAttackBonus();
            testResults.Add(new TestResult("Attack Bonus", attackBonus >= 0, "Attack bonus: " + attackBonus));
            
            int defenseBonus = testEquipment.GetDefenseBonus();
            testResults.Add(new TestResult("Defense Bonus", defenseBonus >= 0, "Defense bonus: " + defenseBonus));
            
            int speedBonus = testEquipment.GetSpeedBonus();
            testResults.Add(new TestResult("Speed Bonus", speedBonus >= 0, "Speed bonus: " + speedBonus));
            
            int healthBonus = testEquipment.GetHealthBonus();
            testResults.Add(new TestResult("Health Bonus", healthBonus >= 0, "Health bonus: " + healthBonus));
            
            // Test stat validation
            bool validStatRange = attackBonus <= 100 && defenseBonus <= 100 && speedBonus <= 100 && healthBonus <= 100;
            testResults.Add(new TestResult("Valid Stat Range", validStatRange, "All stats within reasonable range"));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment Stats", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Equipment Equipping")]
    public void TestEquipmentEquipping()
    {
        Debug.Log("Testing Equipment Equipping...");
        
        if (testEquipment == null)
        {
            testResults.Add(new TestResult("Equipment Equipping Setup", false, "No test equipment available"));
            return;
        }
        
        try
        {
            // Test equipment slot compatibility
            string equipSlot = testEquipment.GetSlot();
            List<string> validSlots = new List<string> { "Weapon", "Armor", "Accessory", "Shield" };
            bool validSlot = validSlots.Contains(equipSlot);
            testResults.Add(new TestResult("Valid Equipment Slot", validSlot, "Equipment slot: " + equipSlot));
            
            // Test equipment equipping process
            bool canEquip = testEquipment.CanEquip();
            testResults.Add(new TestResult("Can Equip", canEquip, "Equipment can be equipped: " + canEquip));
            
            // Test equipment requirements
            bool meetsRequirements = testEquipment.MeetsRequirements();
            testResults.Add(new TestResult("Meets Requirements", meetsRequirements, "Meets requirements: " + meetsRequirements));
            
            // Test equipment durability
            int durability = testEquipment.GetDurability();
            bool hasDurability = durability > 0;
            testResults.Add(new TestResult("Equipment Durability", hasDurability, "Durability: " + durability));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment Equipping", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Equipment UI")]
    public void TestEquipmentUI()
    {
        Debug.Log("Testing Equipment UI...");
        
        if (armoryUI == null)
        {
            testResults.Add(new TestResult("Equipment UI Setup", false, "No armory UI available"));
            return;
        }
        
        try
        {
            // Test UI initialization
            bool uiInitialized = armoryUI.IsInitialized();
            testResults.Add(new TestResult("UI Initialized", uiInitialized, "UI initialized: " + uiInitialized));
            
            // Test equipment display
            armoryUI.DisplayEquipment();
            testResults.Add(new TestResult("Display Equipment", true, "Equipment displayed successfully"));
            
            // Test equipment selection
            armoryUI.SelectEquipment(0);
            testResults.Add(new TestResult("Select Equipment", true, "Equipment selected successfully"));
            
            // Test equipment info display
            armoryUI.DisplayEquipmentInfo();
            testResults.Add(new TestResult("Display Equipment Info", true, "Equipment info displayed successfully"));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment UI", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Equipment Validation")]
    public void TestEquipmentValidation()
    {
        Debug.Log("Testing Equipment Validation...");
        
        if (testEquipment == null)
        {
            testResults.Add(new TestResult("Equipment Validation Setup", false, "No test equipment available"));
            return;
        }
        
        try
        {
            // Test equipment name validation
            string equipName = testEquipment.GetName();
            bool validName = !string.IsNullOrEmpty(equipName) && equipName.Length <= 50;
            testResults.Add(new TestResult("Valid Equipment Name", validName, "Equipment name valid: " + validName));
            
            // Test equipment type validation
            string equipType = testEquipment.GetType().Name;
            bool validType = !string.IsNullOrEmpty(equipType);
            testResults.Add(new TestResult("Valid Equipment Type", validType, "Equipment type valid: " + validType));
            
            // Test equipment rarity validation
            string equipRarity = testEquipment.GetRarity();
            List<string> validRarities = new List<string> { "Common", "Uncommon", "Rare", "Epic", "Legendary" };
            bool validRarity = validRarities.Contains(equipRarity);
            testResults.Add(new TestResult("Valid Equipment Rarity", validRarity, "Equipment rarity valid: " + validRarity));
            
            // Test equipment value validation
            int equipValue = testEquipment.GetValue();
            bool validValue = equipValue >= 0 && equipValue <= 10000;
            testResults.Add(new TestResult("Valid Equipment Value", validValue, "Equipment value: " + equipValue));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment Validation", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Equipment Serialization")]
    public void TestEquipmentSerialization()
    {
        Debug.Log("Testing Equipment Serialization...");
        
        if (testEquipment == null)
        {
            testResults.Add(new TestResult("Equipment Serialization Setup", false, "No test equipment available"));
            return;
        }
        
        try
        {
            // Test equipment to string conversion
            string equipmentString = testEquipment.ToString();
            bool hasStringRepresentation = !string.IsNullOrEmpty(equipmentString);
            testResults.Add(new TestResult("Equipment String Representation", hasStringRepresentation, "String length: " + equipmentString.Length));
            
            // Test equipment data export
            string equipmentData = testEquipment.ExportData();
            bool hasExportData = !string.IsNullOrEmpty(equipmentData);
            testResults.Add(new TestResult("Equipment Data Export", hasExportData, "Export data available: " + hasExportData));
            
            // Test equipment data import
            if (hasExportData)
            {
                Equipment importedEquipment = new Equipment();
                importedEquipment.ImportData(equipmentData);
                bool importSuccessful = importedEquipment.GetName() == testEquipment.GetName();
                testResults.Add(new TestResult("Equipment Data Import", importSuccessful, "Import successful: " + importSuccessful));
            }
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment Serialization", false, "Failed: " + e.Message));
        }
    }
    
    [ContextMenu("Test Equipment Edge Cases")]
    public void TestEquipmentEdgeCases()
    {
        Debug.Log("Testing Equipment Edge Cases...");
        
        try
        {
            // Test null equipment handling
            Equipment nullEquipment = null;
            bool nullHandled = true;
            try
            {
                string nullName = nullEquipment?.GetName();
                testResults.Add(new TestResult("Null Equipment Handling", nullHandled, "Null equipment handled gracefully"));
            }
            catch (System.NullReferenceException)
            {
                testResults.Add(new TestResult("Null Equipment Handling", true, "Null reference properly detected"));
            }
            
            // Test empty equipment name
            Equipment emptyEquipment = new Equipment();
            emptyEquipment.SetName("");
            string emptyName = emptyEquipment.GetName();
            bool emptyNameHandled = emptyName == "";
            testResults.Add(new TestResult("Empty Equipment Name", emptyNameHandled, "Empty name handled: " + emptyNameHandled));
            
            // Test negative stat values
            Equipment negativeEquipment = new Equipment();
            negativeEquipment.SetAttackBonus(-10);
            int negativeAttack = negativeEquipment.GetAttackBonus();
            bool negativeStatsHandled = negativeAttack <= 0;
            testResults.Add(new TestResult("Negative Stats Handling", negativeStatsHandled, "Negative attack: " + negativeAttack));
            
            // Test excessive stat values
            Equipment excessiveEquipment = new Equipment();
            excessiveEquipment.SetAttackBonus(1000);
            int excessiveAttack = excessiveEquipment.GetAttackBonus();
            bool excessiveStatsHandled = excessiveAttack >= 1000;
            testResults.Add(new TestResult("Excessive Stats Handling", excessiveStatsHandled, "Excessive attack: " + excessiveAttack));
            
        }
        catch (System.Exception e)
        {
            testResults.Add(new TestResult("Equipment Edge Cases", false, "Failed: " + e.Message));
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