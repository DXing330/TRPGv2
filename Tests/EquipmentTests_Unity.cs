using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

// Unity-compatible version for older Unity versions
// Use this if NUnit is not available
public class EquipmentTests_Unity : MonoBehaviour
{
    private Equipment equipment;
    private GameObject testGameObject;
    private TacticActor mockActor;
    private GameObject actorGameObject;

    void Start()
    {
        RunAllTests();
    }

    public void RunAllTests()
    {
        Debug.Log("=== Starting Equipment Tests ===");
        
        try
        {
            Setup();
            Test_SetAllStats_ValidData_ParsesCorrectly();
            TearDown();

            Setup();
            Test_SetAllStats_ShortData_SetsToNone();
            TearDown();

            Setup();
            Test_SetAllStats_EmptyData_SetsToNone();
            TearDown();

            Setup();
            Test_GetName_ReturnsCorrectName();
            TearDown();

            Setup();
            Test_GetSlot_ReturnsCorrectSlot();
            TearDown();

            Setup();
            Test_GetEquipType_ReturnsCorrectType();
            TearDown();

            Setup();
            Test_SetAllStats_ComplexPassives_ParsesCorrectly();
            TearDown();

            Setup();
            Test_SetAllStats_SinglePassive_ParsesCorrectly();
            TearDown();

            Setup();
            Test_Equipment_InitialState_PropertiesAreNull();
            TearDown();

            Debug.Log("=== All Equipment Tests Passed! ===");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Equipment test failed: {e.Message}");
        }
    }

    void Setup()
    {
        testGameObject = new GameObject("TestEquipment");
        equipment = testGameObject.AddComponent<Equipment>();
        
        actorGameObject = new GameObject("MockActor");
        mockActor = actorGameObject.AddComponent<TacticActor>();
    }

    void TearDown()
    {
        if (testGameObject != null)
            DestroyImmediate(testGameObject);
        if (actorGameObject != null)
            DestroyImmediate(actorGameObject);
    }

    void Test_SetAllStats_ValidData_ParsesCorrectly()
    {
        // Arrange
        string validStats = "Iron Sword|Weapon|Sword|Power,Sharpness|2,3|5";
        
        // Act
        equipment.SetAllStats(validStats);
        
        // Assert
        Assert.AreEqual("Iron Sword", equipment.GetName(), "Name should be 'Iron Sword'");
        Assert.AreEqual("Weapon", equipment.GetSlot(), "Slot should be 'Weapon'");
        Assert.AreEqual("Sword", equipment.GetEquipType(), "Type should be 'Sword'");
        Assert.AreEqual("5", equipment.maxLevel, "Max level should be '5'");
        Assert.AreEqual(2, equipment.passives.Count, "Should have 2 passives");
        Assert.AreEqual("Power", equipment.passives[0], "First passive should be 'Power'");
        Assert.AreEqual("Sharpness", equipment.passives[1], "Second passive should be 'Sharpness'");
        Assert.AreEqual("2", equipment.passiveLevels[0], "First passive level should be '2'");
        Assert.AreEqual("3", equipment.passiveLevels[1], "Second passive level should be '3'");
        Debug.Log("✓ SetAllStats valid data test passed");
    }

    void Test_SetAllStats_ShortData_SetsToNone()
    {
        // Arrange
        string shortStats = "Short";
        
        // Act
        equipment.SetAllStats(shortStats);
        
        // Assert
        Assert.AreEqual("None", equipment.GetName(), "Name should be 'None' for short data");
        Assert.AreEqual("-1", equipment.GetSlot(), "Slot should be '-1' for short data");
        Assert.AreEqual("-1", equipment.GetEquipType(), "Type should be '-1' for short data");
        Debug.Log("✓ SetAllStats short data test passed");
    }

    void Test_SetAllStats_EmptyData_SetsToNone()
    {
        // Arrange
        string emptyStats = "";
        
        // Act
        equipment.SetAllStats(emptyStats);
        
        // Assert
        Assert.AreEqual("None", equipment.GetName(), "Name should be 'None' for empty data");
        Assert.AreEqual("-1", equipment.GetSlot(), "Slot should be '-1' for empty data");
        Assert.AreEqual("-1", equipment.GetEquipType(), "Type should be '-1' for empty data");
        Debug.Log("✓ SetAllStats empty data test passed");
    }

    void Test_GetName_ReturnsCorrectName()
    {
        // Arrange
        equipment.equipName = "Test Weapon";
        
        // Act & Assert
        Assert.AreEqual("Test Weapon", equipment.GetName(), "Name getter should return correct value");
        Debug.Log("✓ GetName test passed");
    }

    void Test_GetSlot_ReturnsCorrectSlot()
    {
        // Arrange
        equipment.slot = "Armor";
        
        // Act & Assert
        Assert.AreEqual("Armor", equipment.GetSlot(), "Slot getter should return correct value");
        Debug.Log("✓ GetSlot test passed");
    }

    void Test_GetEquipType_ReturnsCorrectType()
    {
        // Arrange
        equipment.type = "Heavy";
        
        // Act & Assert
        Assert.AreEqual("Heavy", equipment.GetEquipType(), "Type getter should return correct value");
        Debug.Log("✓ GetEquipType test passed");
    }

    void Test_SetAllStats_ComplexPassives_ParsesCorrectly()
    {
        // Arrange
        string complexStats = "Magic Staff|Weapon|Staff|Fire,Ice,Lightning|1,2,3|10";
        
        // Act
        equipment.SetAllStats(complexStats);
        
        // Assert
        Assert.AreEqual("Magic Staff", equipment.GetName(), "Name should be 'Magic Staff'");
        Assert.AreEqual("Weapon", equipment.GetSlot(), "Slot should be 'Weapon'");
        Assert.AreEqual("Staff", equipment.GetEquipType(), "Type should be 'Staff'");
        Assert.AreEqual("10", equipment.maxLevel, "Max level should be '10'");
        Assert.AreEqual(3, equipment.passives.Count, "Should have 3 passives");
        Assert.AreEqual("Fire", equipment.passives[0], "First passive should be 'Fire'");
        Assert.AreEqual("Ice", equipment.passives[1], "Second passive should be 'Ice'");
        Assert.AreEqual("Lightning", equipment.passives[2], "Third passive should be 'Lightning'");
        Assert.AreEqual("1", equipment.passiveLevels[0], "First passive level should be '1'");
        Assert.AreEqual("2", equipment.passiveLevels[1], "Second passive level should be '2'");
        Assert.AreEqual("3", equipment.passiveLevels[2], "Third passive level should be '3'");
        Debug.Log("✓ SetAllStats complex passives test passed");
    }

    void Test_SetAllStats_SinglePassive_ParsesCorrectly()
    {
        // Arrange
        string singlePassiveStats = "Simple Ring|Accessory|Ring|Defense|1|3";
        
        // Act
        equipment.SetAllStats(singlePassiveStats);
        
        // Assert
        Assert.AreEqual("Simple Ring", equipment.GetName(), "Name should be 'Simple Ring'");
        Assert.AreEqual("Accessory", equipment.GetSlot(), "Slot should be 'Accessory'");
        Assert.AreEqual("Ring", equipment.GetEquipType(), "Type should be 'Ring'");
        Assert.AreEqual("3", equipment.maxLevel, "Max level should be '3'");
        Assert.AreEqual(1, equipment.passives.Count, "Should have 1 passive");
        Assert.AreEqual("Defense", equipment.passives[0], "Passive should be 'Defense'");
        Assert.AreEqual("1", equipment.passiveLevels[0], "Passive level should be '1'");
        Debug.Log("✓ SetAllStats single passive test passed");
    }

    void Test_Equipment_InitialState_PropertiesAreNull()
    {
        // Assert - Before SetAllStats is called
        Assert.IsNull(equipment.equipName, "Equipment name should be null initially");
        Assert.IsNull(equipment.slot, "Equipment slot should be null initially");
        Assert.IsNull(equipment.type, "Equipment type should be null initially");
        Assert.IsNull(equipment.passives, "Equipment passives should be null initially");
        Assert.IsNull(equipment.passiveLevels, "Equipment passive levels should be null initially");
        Assert.IsNull(equipment.maxLevel, "Equipment max level should be null initially");
        Debug.Log("✓ Equipment initial state test passed");
    }
}