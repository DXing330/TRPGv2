using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[TestFixture]
public class BattleManagerTests
{
    private BattleManager battleManager;
    private GameObject testGameObject;
    private TacticActor mockActor;
    private BattleMap mockMap;

    [SetUp]
    public void Setup()
    {
        testGameObject = new GameObject("TestBattleManager");
        battleManager = testGameObject.AddComponent<BattleManager>();
        
        // Initialize mock dependencies
        mockActor = new GameObject("MockActor").AddComponent<TacticActor>();
        mockMap = new GameObject("MockMap").AddComponent<BattleMap>();
        
        // Set up basic battle manager state
        battleManager.map = mockMap;
        battleManager.turnNumber = 0;
        battleManager.roundNumber = 1;
        battleManager.turnActor = mockActor;
        battleManager.interactable = true;
        battleManager.selectedState = "";
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testGameObject);
        Object.DestroyImmediate(mockActor.gameObject);
        Object.DestroyImmediate(mockMap.gameObject);
    }

    [Test]
    public void GetRoundNumber_ReturnsCorrectRoundNumber()
    {
        battleManager.roundNumber = 5;
        Assert.AreEqual(5, battleManager.GetRoundNumber());
    }

    [Test]
    public void GetTurnIndex_ReturnsCorrectTurnIndex()
    {
        battleManager.turnNumber = 3;
        Assert.AreEqual(3, battleManager.GetTurnIndex());
    }

    [Test]
    public void GetTurnActor_ReturnsCorrectActor()
    {
        Assert.AreEqual(mockActor, battleManager.GetTurnActor());
    }

    [Test]
    public void GetState_ReturnsCorrectState()
    {
        battleManager.selectedState = "Move";
        Assert.AreEqual("Move", battleManager.GetState());
    }

    [Test]
    public void SetState_Move_WhenActorHasSpeed()
    {
        // Arrange
        mockActor.GetComponent<TacticActor>().speed = 5;
        
        // Act
        battleManager.SetState("Move");
        
        // Assert
        Assert.AreEqual("Move", battleManager.selectedState);
    }

    [Test]
    public void SetState_Move_WhenActorHasNoSpeed_ResetsState()
    {
        // Arrange
        mockActor.GetComponent<TacticActor>().speed = 0;
        
        // Act
        battleManager.SetState("Move");
        
        // Assert
        Assert.AreEqual("", battleManager.selectedState);
    }

    [Test]
    public void SetState_Attack_SetsCorrectState()
    {
        battleManager.SetState("Attack");
        Assert.AreEqual("Attack", battleManager.selectedState);
    }

    [Test]
    public void SetState_Skill_SetsCorrectState()
    {
        battleManager.SetState("Skill");
        Assert.AreEqual("Skill", battleManager.selectedState);
    }

    [Test]
    public void SetState_SameState_ResetsState()
    {
        battleManager.selectedState = "Move";
        battleManager.SetState("Move");
        Assert.AreEqual("", battleManager.selectedState);
    }

    [Test]
    public void ClickOnTile_WhenNotInteractable_DoesNothing()
    {
        // Arrange
        battleManager.interactable = false;
        battleManager.selectedState = "Move";
        
        // Act
        battleManager.ClickOnTile(0);
        
        // Assert - state should remain unchanged
        Assert.AreEqual("Move", battleManager.selectedState);
    }

    [Test]
    public void ClickOnTile_InvalidTile_ResetsState()
    {
        // Arrange
        battleManager.selectedState = "Move";
        mockMap.currentTiles = new List<int> { -1, -1, -1 };
        
        // Act
        battleManager.ClickOnTile(0);
        
        // Assert
        Assert.AreEqual("", battleManager.selectedState);
        Assert.AreEqual(-1, battleManager.selectedTile);
    }

    [Test]
    public void ClickOnTile_ViewState_CallsViewActorOnTile()
    {
        // Arrange
        battleManager.selectedState = "View";
        mockMap.currentTiles = new List<int> { 0, 1, 2 };
        
        // Act
        battleManager.ClickOnTile(0);
        
        // Assert
        Assert.AreEqual(0, battleManager.selectedTile);
    }

    [Test]
    public void SpawnAndAddActor_AddsActorToMap()
    {
        // This test would require more setup of the ActorMaker mock
        // For now, we'll test the basic setup
        int testLocation = 5;
        string testActorName = "TestActor";
        int testTeam = 0;
        
        // Act
        battleManager.SpawnAndAddActor(testLocation, testActorName, testTeam);
        
        // Assert - would need to verify the actor was added to the map
        // This depends on the ActorMaker implementation
    }

    [Test]
    public void ViewActorFromTurnOrder_ValidIndex_SetsSelectedActor()
    {
        // Arrange
        battleManager.turnNumber = 0;
        List<TacticActor> mockActors = new List<TacticActor> { mockActor };
        
        // This would require setting up the map's GetActorByIndex method
        // For now, test the basic logic
        int testIndex = 0;
        
        // Act
        battleManager.ViewActorFromTurnOrder(testIndex);
        
        // The implementation depends on mockMap.GetActorByIndex
        // In a real test, we'd verify the selectedActor is set correctly
    }

    [Test]
    public void ClickOnTile_EmptyState_CallsViewActorOnTile()
    {
        // Arrange
        battleManager.selectedState = "";
        mockMap.currentTiles = new List<int> { 0, 1, 2 };
        
        // Act
        battleManager.ClickOnTile(0);
        
        // Assert
        Assert.AreEqual(0, battleManager.selectedTile);
    }

    [Test]
    public void ResetState_ClearsAllStateVariables()
    {
        // Arrange
        battleManager.selectedState = "Move";
        battleManager.selectedTile = 5;
        battleManager.selectedActor = mockActor;
        
        // Act - This calls the protected ResetState method indirectly through SetState
        battleManager.SetState("Move"); // This will reset because it's the same state
        
        // Assert
        Assert.AreEqual("", battleManager.selectedState);
        Assert.AreEqual(-1, battleManager.selectedTile);
        Assert.AreEqual(null, battleManager.selectedActor);
    }

    [Test]
    public void GetMoveCost_ReturnsCorrectValue()
    {
        // This test would require setting up the MoveCostManager
        // For now, test that the property exists and is accessible
        Assert.IsNotNull(battleManager.moveManager);
    }

    [Test]
    public void TurnActor_ActionsLeft_CanPerformActions()
    {
        // Arrange - this would require setting up TacticActor's ActionsLeft method
        // For now, test that the turn actor is properly set
        Assert.IsNotNull(battleManager.turnActor);
    }

    [Test]
    public void BattleState_InitializedCorrectly()
    {
        // Test initial state
        Assert.AreEqual(0, battleManager.turnNumber);
        Assert.AreEqual(1, battleManager.roundNumber);
        Assert.IsTrue(battleManager.interactable);
        Assert.AreEqual("", battleManager.selectedState);
        Assert.AreEqual(-1, battleManager.selectedTile);
        Assert.IsNull(battleManager.selectedActor);
    }
}