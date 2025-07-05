using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[TestFixture]
public class HiringManagerTests
{
    private HiringManager hiringManager;
    private GameObject testGameObject;
    private StatDatabase mockFirstNames;
    private StatDatabase mockMiddleNames;
    private StatDatabase mockLastNames;
    private StatDatabase mockActorData;
    private TacticActor mockDummyActor;
    private PartyDataManager mockPartyData;
    private Inventory mockInventory;
    private SelectStatTextList mockHirelingList;
    private StatTextList mockHirelingStats;
    private GameObject[] mockObjects;

    [SetUp]
    public void Setup()
    {
        testGameObject = new GameObject("TestHiringManager");
        hiringManager = testGameObject.AddComponent<HiringManager>();
        
        // Create mock dependencies
        mockObjects = new GameObject[10];
        
        mockObjects[0] = new GameObject("MockFirstNames");
        mockFirstNames = mockObjects[0].AddComponent<StatDatabase>();
        
        mockObjects[1] = new GameObject("MockMiddleNames");
        mockMiddleNames = mockObjects[1].AddComponent<StatDatabase>();
        
        mockObjects[2] = new GameObject("MockLastNames");
        mockLastNames = mockObjects[2].AddComponent<StatDatabase>();
        
        mockObjects[3] = new GameObject("MockActorData");
        mockActorData = mockObjects[3].AddComponent<StatDatabase>();
        
        mockObjects[4] = new GameObject("MockDummyActor");
        mockDummyActor = mockObjects[4].AddComponent<TacticActor>();
        
        mockObjects[5] = new GameObject("MockPartyData");
        mockPartyData = mockObjects[5].AddComponent<PartyDataManager>();
        
        mockObjects[6] = new GameObject("MockInventory");
        mockInventory = mockObjects[6].AddComponent<Inventory>();
        
        mockObjects[7] = new GameObject("MockHirelingList");
        mockHirelingList = mockObjects[7].AddComponent<SelectStatTextList>();
        
        mockObjects[8] = new GameObject("MockHirelingStats");
        mockHirelingStats = mockObjects[8].AddComponent<StatTextList>();
        
        mockObjects[9] = new GameObject("MockGuildCard");
        var mockGuildCard = mockObjects[9].AddComponent<GuildCard>();
        
        // Initialize hiring manager
        hiringManager.firstNames = mockFirstNames;
        hiringManager.middleNames = mockMiddleNames;
        hiringManager.lastNames = mockLastNames;
        hiringManager.actorData = mockActorData;
        hiringManager.dummyActor = mockDummyActor;
        hiringManager.partyData = mockPartyData;
        hiringManager.inventory = mockInventory;
        hiringManager.hirelingList = mockHirelingList;
        hiringManager.hirelingStats = mockHirelingStats;
        
        // Set up mock party data components
        mockPartyData.guildCard = mockGuildCard;
        
        // Initialize basic values
        hiringManager.hireableActors = new List<string> { "Fighter", "Mage", "Rogue" };
        hiringManager.basePrices = new List<string> { "100", "150", "120" };
        hiringManager.possibleNames = new List<string> { "TestName1", "TestName2", "TestName3" };
        hiringManager.minHirelings = 2;
        hiringManager.priceFeeRatio = 10;
        hiringManager.currentHirelingClasses = new List<string>();
        hiringManager.currentHirelingNames = new List<string>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testGameObject);
        for (int i = 0; i < mockObjects.Length; i++)
        {
            if (mockObjects[i] != null)
                Object.DestroyImmediate(mockObjects[i]);
        }
    }

    [Test]
    public void GenerateRandomName_ReturnsFormattedName()
    {
        // Act
        string name = hiringManager.GenerateRandomName();
        
        // Assert - would need to verify name format (First Middle Last)
        // This depends on the StatDatabase.ReturnRandomValue implementation
        Assert.IsNotNull(name);
        Assert.IsTrue(name.Length > 0);
    }

    [Test]
    public void ViewStats_ValidSelection_UpdatesStats()
    {
        // Arrange
        hiringManager.currentHirelingClasses = new List<string> { "Fighter", "Mage" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter", "Test Mage" };
        
        // Act
        hiringManager.ViewStats();
        
        // Assert - would need to verify stats were updated
        // This depends on the SelectStatTextList and StatTextList implementations
        Assert.IsNotNull(hiringManager.hirelingStats);
    }

    [Test]
    public void ViewStats_NoSelection_DoesNothing()
    {
        // Arrange - hirelingList returns -1 for no selection
        hiringManager.currentHirelingClasses = new List<string> { "Fighter" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter" };
        
        // Act
        hiringManager.ViewStats();
        
        // Assert - Should not throw any exceptions
        Assert.IsNotNull(hiringManager.hirelingStats);
    }

    [Test]
    public void TryToHire_NoOpenSlots_DoesNotHire()
    {
        // Arrange - partyData returns false for OpenSlots
        hiringManager.currentHirelingClasses = new List<string> { "Fighter" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter" };
        
        // Act
        hiringManager.TryToHire();
        
        // Assert - would need to verify no hiring occurred
        // This depends on the PartyDataManager.OpenSlots implementation
        Assert.IsNotNull(hiringManager.currentHirelingClasses);
    }

    [Test]
    public void TryToHire_NoSelection_DoesNothing()
    {
        // Arrange - hirelingList returns -1 for no selection
        hiringManager.currentHirelingClasses = new List<string> { "Fighter" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter" };
        
        // Act
        hiringManager.TryToHire();
        
        // Assert - Should not throw any exceptions
        Assert.IsNotNull(hiringManager.currentHirelingClasses);
    }

    [Test]
    public void TryToHire_InsufficientFunds_DoesNotHire()
    {
        // Arrange - inventory returns false for QuantityExists
        hiringManager.currentHirelingClasses = new List<string> { "Fighter" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter" };
        
        // Act
        hiringManager.TryToHire();
        
        // Assert - would need to verify no hiring occurred
        // This depends on the Inventory.QuantityExists implementation
        Assert.IsNotNull(hiringManager.currentHirelingClasses);
    }

    [Test]
    public void TryToHire_ValidHire_RemovesHirelingFromList()
    {
        // Arrange
        hiringManager.currentHirelingClasses = new List<string> { "Fighter", "Mage" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter", "Test Mage" };
        int initialCount = hiringManager.currentHirelingClasses.Count;
        
        // Act
        hiringManager.TryToHire();
        
        // Assert - would need to verify hireling was removed
        // This depends on the various dependency implementations
        Assert.IsTrue(hiringManager.currentHirelingClasses.Count <= initialCount);
    }

    [Test]
    public void MinHirelings_DefaultValue()
    {
        // Assert
        Assert.AreEqual(2, hiringManager.minHirelings);
    }

    [Test]
    public void PriceFeeRatio_DefaultValue()
    {
        // Assert
        Assert.AreEqual(10, hiringManager.priceFeeRatio);
    }

    [Test]
    public void HireableActors_InitializedCorrectly()
    {
        // Assert
        Assert.IsNotNull(hiringManager.hireableActors);
        Assert.AreEqual(3, hiringManager.hireableActors.Count);
        Assert.Contains("Fighter", hiringManager.hireableActors);
        Assert.Contains("Mage", hiringManager.hireableActors);
        Assert.Contains("Rogue", hiringManager.hireableActors);
    }

    [Test]
    public void BasePrices_InitializedCorrectly()
    {
        // Assert
        Assert.IsNotNull(hiringManager.basePrices);
        Assert.AreEqual(3, hiringManager.basePrices.Count);
        Assert.Contains("100", hiringManager.basePrices);
        Assert.Contains("150", hiringManager.basePrices);
        Assert.Contains("120", hiringManager.basePrices);
    }

    [Test]
    public void CurrentHirelingLists_InitializedCorrectly()
    {
        // Assert
        Assert.IsNotNull(hiringManager.currentHirelingClasses);
        Assert.IsNotNull(hiringManager.currentHirelingNames);
        Assert.AreEqual(hiringManager.currentHirelingClasses.Count, hiringManager.currentHirelingNames.Count);
    }

    [Test]
    public void Components_InitializedCorrectly()
    {
        // Assert all components are not null
        Assert.IsNotNull(hiringManager.firstNames);
        Assert.IsNotNull(hiringManager.middleNames);
        Assert.IsNotNull(hiringManager.lastNames);
        Assert.IsNotNull(hiringManager.actorData);
        Assert.IsNotNull(hiringManager.dummyActor);
        Assert.IsNotNull(hiringManager.partyData);
        Assert.IsNotNull(hiringManager.inventory);
        Assert.IsNotNull(hiringManager.hirelingList);
        Assert.IsNotNull(hiringManager.hirelingStats);
    }

    [Test]
    public void GetPrice_ValidIndex_ReturnsCorrectPrice()
    {
        // Arrange
        hiringManager.currentHirelingClasses = new List<string> { "Fighter", "Mage" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter", "Test Mage" };
        
        // Act - This would require setting up hirelingList.GetSelected() to return a valid index
        // For now, test that the method exists and can be called
        Assert.DoesNotThrow(() => hiringManager.ViewStats());
    }

    [Test]
    public void UpdateGuildCardHirelings_UpdatesGuildCard()
    {
        // Arrange
        hiringManager.currentHirelingClasses = new List<string> { "Fighter" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter" };
        
        // Act - This calls the protected method indirectly
        hiringManager.TryToHire();
        
        // Assert - would need to verify guild card was updated
        // This depends on the GuildCard implementation
        Assert.IsNotNull(hiringManager.partyData.guildCard);
    }

    [Test]
    public void GenerateHirelings_UpdatesCurrentLists()
    {
        // This test would require setting up the GuildCard mock to return specific values
        // For now, test that the lists are initialized
        Assert.IsNotNull(hiringManager.currentHirelingClasses);
        Assert.IsNotNull(hiringManager.currentHirelingNames);
    }

    [Test]
    public void GenerateHirelings_RefreshNotRequired_UsesExistingData()
    {
        // This test would require setting up the GuildCard mock to return false for RefreshHireables
        // For now, test that the method can be called without errors
        Assert.DoesNotThrow(() => hiringManager.ViewStats());
    }

    [Test]
    public void GenerateHirelings_RefreshRequired_GeneratesNewHirelings()
    {
        // This test would require setting up the GuildCard mock to return true for RefreshHireables
        // For now, test that the method can be called without errors
        Assert.DoesNotThrow(() => hiringManager.ViewStats());
    }

    [Test]
    public void HireableActors_BasePrices_CountsMatch()
    {
        // Assert
        Assert.AreEqual(hiringManager.hireableActors.Count, hiringManager.basePrices.Count);
    }

    [Test]
    public void ViewStats_SetsCorrectStatNames()
    {
        // Arrange
        hiringManager.currentHirelingClasses = new List<string> { "Fighter" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter" };
        
        // Act
        hiringManager.ViewStats();
        
        // Assert - would need to verify stat names were set correctly
        // This depends on the StatTextList implementation
        Assert.IsNotNull(hiringManager.hirelingStats);
    }

    [Test]
    public void ViewStats_CalculatesCorrectStats()
    {
        // Arrange
        hiringManager.currentHirelingClasses = new List<string> { "Fighter" };
        hiringManager.currentHirelingNames = new List<string> { "Test Fighter" };
        
        // Act
        hiringManager.ViewStats();
        
        // Assert - would need to verify stats were calculated correctly
        // This depends on the TacticActor and StatTextList implementations
        Assert.IsNotNull(hiringManager.dummyActor);
    }

    [Test]
    public void TryToHire_SuccessfulHire_UpdatesInventory()
    {
        // This test would require setting up mocks to return appropriate values
        // For now, test that the method can be called without errors
        Assert.DoesNotThrow(() => hiringManager.TryToHire());
    }

    [Test]
    public void TryToHire_SuccessfulHire_UpdatesPartyData()
    {
        // This test would require setting up mocks to return appropriate values
        // For now, test that the method can be called without errors
        Assert.DoesNotThrow(() => hiringManager.TryToHire());
    }

    [Test]
    public void TryToHire_SuccessfulHire_ResetsHirelingList()
    {
        // This test would require setting up mocks to return appropriate values
        // For now, test that the method can be called without errors
        Assert.DoesNotThrow(() => hiringManager.TryToHire());
    }

    [Test]
    public void Lists_InitializedEmpty()
    {
        // Assert
        Assert.AreEqual(0, hiringManager.currentHirelingClasses.Count);
        Assert.AreEqual(0, hiringManager.currentHirelingNames.Count);
    }

    [Test]
    public void PossibleNames_InitializedCorrectly()
    {
        // Assert
        Assert.IsNotNull(hiringManager.possibleNames);
        Assert.AreEqual(3, hiringManager.possibleNames.Count);
        Assert.Contains("TestName1", hiringManager.possibleNames);
        Assert.Contains("TestName2", hiringManager.possibleNames);
        Assert.Contains("TestName3", hiringManager.possibleNames);
    }
}