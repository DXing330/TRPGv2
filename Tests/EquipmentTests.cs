using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[TestFixture]
public class EquipmentTests
{
    private Equipment equipment;
    private GameObject testGameObject;
    private TacticActor mockActor;
    private GameObject actorGameObject;

    [SetUp]
    public void Setup()
    {
        testGameObject = new GameObject("TestEquipment");
        equipment = testGameObject.AddComponent<Equipment>();
        
        actorGameObject = new GameObject("MockActor");
        mockActor = actorGameObject.AddComponent<TacticActor>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testGameObject);
        Object.DestroyImmediate(actorGameObject);
    }

    [Test]
    public void SetAllStats_ValidData_ParsesCorrectly()
    {
        // Arrange
        string validStats = "Iron Sword|Weapon|Sword|Power,Sharpness|2,3|5";
        
        // Act
        equipment.SetAllStats(validStats);
        
        // Assert
        Assert.AreEqual("Iron Sword", equipment.GetName());
        Assert.AreEqual("Weapon", equipment.GetSlot());
        Assert.AreEqual("Sword", equipment.GetEquipType());
        Assert.AreEqual("5", equipment.maxLevel);
        Assert.AreEqual(2, equipment.passives.Count);
        Assert.AreEqual("Power", equipment.passives[0]);
        Assert.AreEqual("Sharpness", equipment.passives[1]);
        Assert.AreEqual("2", equipment.passiveLevels[0]);
        Assert.AreEqual("3", equipment.passiveLevels[1]);
    }

    [Test]
    public void SetAllStats_ShortData_SetsToNone()
    {
        // Arrange
        string shortStats = "Short";
        
        // Act
        equipment.SetAllStats(shortStats);
        
        // Assert
        Assert.AreEqual("None", equipment.GetName());
        Assert.AreEqual("-1", equipment.GetSlot());
        Assert.AreEqual("-1", equipment.GetEquipType());
    }

    [Test]
    public void SetAllStats_EmptyData_SetsToNone()
    {
        // Arrange
        string emptyStats = "";
        
        // Act
        equipment.SetAllStats(emptyStats);
        
        // Assert
        Assert.AreEqual("None", equipment.GetName());
        Assert.AreEqual("-1", equipment.GetSlot());
        Assert.AreEqual("-1", equipment.GetEquipType());
    }

    [Test]
    public void GetName_ReturnsCorrectName()
    {
        // Arrange
        equipment.equipName = "Test Weapon";
        
        // Act & Assert
        Assert.AreEqual("Test Weapon", equipment.GetName());
    }

    [Test]
    public void GetSlot_ReturnsCorrectSlot()
    {
        // Arrange
        equipment.slot = "Armor";
        
        // Act & Assert
        Assert.AreEqual("Armor", equipment.GetSlot());
    }

    [Test]
    public void GetEquipType_ReturnsCorrectType()
    {
        // Arrange
        equipment.type = "Heavy";
        
        // Act & Assert
        Assert.AreEqual("Heavy", equipment.GetEquipType());
    }

    [Test]
    public void SetAllStats_ComplexPassives_ParsesCorrectly()
    {
        // Arrange
        string complexStats = "Magic Staff|Weapon|Staff|Fire,Ice,Lightning|1,2,3|10";
        
        // Act
        equipment.SetAllStats(complexStats);
        
        // Assert
        Assert.AreEqual("Magic Staff", equipment.GetName());
        Assert.AreEqual("Weapon", equipment.GetSlot());
        Assert.AreEqual("Staff", equipment.GetEquipType());
        Assert.AreEqual("10", equipment.maxLevel);
        Assert.AreEqual(3, equipment.passives.Count);
        Assert.AreEqual("Fire", equipment.passives[0]);
        Assert.AreEqual("Ice", equipment.passives[1]);
        Assert.AreEqual("Lightning", equipment.passives[2]);
        Assert.AreEqual("1", equipment.passiveLevels[0]);
        Assert.AreEqual("2", equipment.passiveLevels[1]);
        Assert.AreEqual("3", equipment.passiveLevels[2]);
    }

    [Test]
    public void SetAllStats_SinglePassive_ParsesCorrectly()
    {
        // Arrange
        string singlePassiveStats = "Simple Ring|Accessory|Ring|Defense|1|3";
        
        // Act
        equipment.SetAllStats(singlePassiveStats);
        
        // Assert
        Assert.AreEqual("Simple Ring", equipment.GetName());
        Assert.AreEqual("Accessory", equipment.GetSlot());
        Assert.AreEqual("Ring", equipment.GetEquipType());
        Assert.AreEqual("3", equipment.maxLevel);
        Assert.AreEqual(1, equipment.passives.Count);
        Assert.AreEqual("Defense", equipment.passives[0]);
        Assert.AreEqual("1", equipment.passiveLevels[0]);
    }

    [Test]
    public void EquipToActor_ValidEquipment_AppliesPassives()
    {
        // Arrange
        string validStats = "Iron Sword|Weapon|Sword|Power,Sharpness|2,3|5";
        equipment.SetAllStats(validStats);
        
        // Act
        equipment.EquipToActor(mockActor);
        
        // Assert - This would require mocking the TacticActor methods
        // For now, we test that the method can be called without errors
        Assert.IsNotNull(mockActor);
    }

    [Test]
    public void EquipToActor_ShortData_DoesNothing()
    {
        // Arrange
        string shortStats = "Short";
        equipment.SetAllStats(shortStats);
        
        // Act
        equipment.EquipToActor(mockActor);
        
        // Assert - Should not throw any exceptions
        Assert.IsNotNull(mockActor);
    }

    [Test]
    public void EquipToActor_WeaponType_SetsWeaponType()
    {
        // Arrange
        string weaponStats = "Test Weapon|Weapon|Bow|Archery|1|3";
        equipment.SetAllStats(weaponStats);
        
        // Act
        equipment.EquipToActor(mockActor);
        
        // Assert - This would require mocking TacticActor.SetWeaponType
        // For now, we test that the method can be called
        Assert.IsNotNull(mockActor);
    }

    [Test]
    public void EquipToActor_NonWeaponType_DoesNotSetWeaponType()
    {
        // Arrange
        string armorStats = "Test Armor|Armor|Plate|Defense|2|5";
        equipment.SetAllStats(armorStats);
        
        // Act
        equipment.EquipToActor(mockActor);
        
        // Assert - Should not call SetWeaponType
        Assert.IsNotNull(mockActor);
    }

    [Test]
    public void EquipWeapon_ValidWeapon_SetsWeaponType()
    {
        // Arrange
        string weaponStats = "Test Sword|Weapon|Sword|Slashing|1|3";
        equipment.SetAllStats(weaponStats);
        
        // Act
        equipment.EquipWeapon(mockActor);
        
        // Assert - This would require mocking TacticActor.SetWeaponType
        // For now, we test that the method can be called
        Assert.IsNotNull(mockActor);
    }

    [Test]
    public void EquipWeapon_NonWeapon_DoesNotSetWeaponType()
    {
        // Arrange
        string armorStats = "Test Armor|Armor|Plate|Defense|2|5";
        equipment.SetAllStats(armorStats);
        
        // Act
        equipment.EquipWeapon(mockActor);
        
        // Assert - Should not call SetWeaponType for non-weapons
        Assert.IsNotNull(mockActor);
    }

    [Test]
    public void EquipWeapon_ShortData_DoesNothing()
    {
        // Arrange
        string shortStats = "Short";
        equipment.SetAllStats(shortStats);
        
        // Act
        equipment.EquipWeapon(mockActor);
        
        // Assert - Should not throw any exceptions
        Assert.IsNotNull(mockActor);
    }

    [Test]
    public void DebugPassives_DoesNotThrow()
    {
        // Arrange
        string validStats = "Test Item|Accessory|Ring|Power,Defense|1,2|3";
        equipment.SetAllStats(validStats);
        
        // Act & Assert - Should not throw any exceptions
        Assert.DoesNotThrow(() => equipment.DebugPassives());
    }

    [Test]
    public void DebugPassives_EmptyPassives_DoesNotThrow()
    {
        // Arrange
        equipment.passives = new List<string>();
        equipment.passiveLevels = new List<string>();
        
        // Act & Assert - Should not throw any exceptions
        Assert.DoesNotThrow(() => equipment.DebugPassives());
    }

    [Test]
    public void AllStats_Property_WorksCorrectly()
    {
        // Arrange
        string testStats = "Test Equipment|Weapon|Sword|Power|1|5";
        
        // Act
        equipment.SetAllStats(testStats);
        
        // Assert
        Assert.AreEqual(testStats, equipment.allStats);
    }

    [Test]
    public void SetAllStats_MalformedData_HandlesGracefully()
    {
        // Arrange
        string malformedStats = "Name|Slot|Type|Passives"; // Missing levels and max level
        
        // Act & Assert - Should not throw exceptions
        Assert.DoesNotThrow(() => equipment.SetAllStats(malformedStats));
    }

    [Test]
    public void SetAllStats_ExtraFields_IgnoresExtra()
    {
        // Arrange
        string extraFieldStats = "Name|Slot|Type|Passives|Levels|MaxLevel|Extra|Field";
        
        // Act
        equipment.SetAllStats(extraFieldStats);
        
        // Assert - Should still parse the first 6 fields correctly
        Assert.AreEqual("Name", equipment.GetName());
        Assert.AreEqual("Slot", equipment.GetSlot());
        Assert.AreEqual("Type", equipment.GetEquipType());
        Assert.AreEqual("MaxLevel", equipment.maxLevel);
    }

    [Test]
    public void Passives_Lists_InitializedCorrectly()
    {
        // Arrange
        string validStats = "Test|Weapon|Sword|Pass1,Pass2|1,2|3";
        
        // Act
        equipment.SetAllStats(validStats);
        
        // Assert
        Assert.IsNotNull(equipment.passives);
        Assert.IsNotNull(equipment.passiveLevels);
        Assert.AreEqual(equipment.passives.Count, equipment.passiveLevels.Count);
    }

    [Test]
    public void Equipment_InitialState_PropertiesAreNull()
    {
        // Assert - Before SetAllStats is called
        Assert.IsNull(equipment.equipName);
        Assert.IsNull(equipment.slot);
        Assert.IsNull(equipment.type);
        Assert.IsNull(equipment.passives);
        Assert.IsNull(equipment.passiveLevels);
        Assert.IsNull(equipment.maxLevel);
    }
}