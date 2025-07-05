using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

// Unity-compatible version for older Unity versions
// Use this if NUnit is not available
public class BattleManagerTests_Unity : MonoBehaviour
{
    private BattleManager battleManager;
    private GameObject testGameObject;
    private TacticActor mockActor;
    private BattleMap mockMap;

    void Start()
    {
        RunAllTests();
    }

    public void RunAllTests()
    {
        Debug.Log("=== Starting BattleManager Tests ===");
        
        try
        {
            Setup();
            Test_GetRoundNumber_ReturnsCorrectRoundNumber();
            TearDown();

            Setup();
            Test_GetTurnIndex_ReturnsCorrectTurnIndex();
            TearDown();

            Setup();
            Test_GetTurnActor_ReturnsCorrectActor();
            TearDown();

            Setup();
            Test_GetState_ReturnsCorrectState();
            TearDown();

            Setup();
            Test_SetState_Attack_SetsCorrectState();
            TearDown();

            Setup();
            Test_SetState_Skill_SetsCorrectState();
            TearDown();

            Setup();
            Test_SetState_SameState_ResetsState();
            TearDown();

            Setup();
            Test_ClickOnTile_InvalidTile_ResetsState();
            TearDown();

            Setup();
            Test_BattleState_InitializedCorrectly();
            TearDown();

            Debug.Log("=== All BattleManager Tests Passed! ===");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Test failed: {e.Message}");
        }
    }

    void Setup()
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

    void TearDown()
    {
        if (testGameObject != null)
            DestroyImmediate(testGameObject);
        if (mockActor != null)
            DestroyImmediate(mockActor.gameObject);
        if (mockMap != null)
            DestroyImmediate(mockMap.gameObject);
    }

    void Test_GetRoundNumber_ReturnsCorrectRoundNumber()
    {
        // Arrange
        battleManager.roundNumber = 5;
        
        // Act & Assert
        Assert.AreEqual(5, battleManager.GetRoundNumber(), "Round number should be 5");
        Debug.Log("✓ GetRoundNumber test passed");
    }

    void Test_GetTurnIndex_ReturnsCorrectTurnIndex()
    {
        // Arrange
        battleManager.turnNumber = 3;
        
        // Act & Assert
        Assert.AreEqual(3, battleManager.GetTurnIndex(), "Turn index should be 3");
        Debug.Log("✓ GetTurnIndex test passed");
    }

    void Test_GetTurnActor_ReturnsCorrectActor()
    {
        // Act & Assert
        Assert.AreEqual(mockActor, battleManager.GetTurnActor(), "Turn actor should match mock actor");
        Debug.Log("✓ GetTurnActor test passed");
    }

    void Test_GetState_ReturnsCorrectState()
    {
        // Arrange
        battleManager.selectedState = "Move";
        
        // Act & Assert
        Assert.AreEqual("Move", battleManager.GetState(), "State should be 'Move'");
        Debug.Log("✓ GetState test passed");
    }

    void Test_SetState_Attack_SetsCorrectState()
    {
        // Act
        battleManager.SetState("Attack");
        
        // Assert
        Assert.AreEqual("Attack", battleManager.selectedState, "State should be 'Attack'");
        Debug.Log("✓ SetState Attack test passed");
    }

    void Test_SetState_Skill_SetsCorrectState()
    {
        // Act
        battleManager.SetState("Skill");
        
        // Assert
        Assert.AreEqual("Skill", battleManager.selectedState, "State should be 'Skill'");
        Debug.Log("✓ SetState Skill test passed");
    }

    void Test_SetState_SameState_ResetsState()
    {
        // Arrange
        battleManager.selectedState = "Move";
        
        // Act
        battleManager.SetState("Move");
        
        // Assert
        Assert.AreEqual("", battleManager.selectedState, "State should be reset to empty");
        Debug.Log("✓ SetState same state test passed");
    }

    void Test_ClickOnTile_InvalidTile_ResetsState()
    {
        // Arrange
        battleManager.selectedState = "Move";
        mockMap.currentTiles = new List<int> { -1, -1, -1 };
        
        // Act
        battleManager.ClickOnTile(0);
        
        // Assert
        Assert.AreEqual("", battleManager.selectedState, "State should be reset");
        Assert.AreEqual(-1, battleManager.selectedTile, "Selected tile should be -1");
        Debug.Log("✓ ClickOnTile invalid tile test passed");
    }

    void Test_BattleState_InitializedCorrectly()
    {
        // Assert initial state
        Assert.AreEqual(0, battleManager.turnNumber, "Turn number should start at 0");
        Assert.AreEqual(1, battleManager.roundNumber, "Round number should start at 1");
        Assert.IsTrue(battleManager.interactable, "Battle should be interactable");
        Assert.AreEqual("", battleManager.selectedState, "Selected state should be empty");
        Assert.AreEqual(-1, battleManager.selectedTile, "Selected tile should be -1");
        Assert.IsNull(battleManager.selectedActor, "Selected actor should be null");
        Debug.Log("✓ BattleState initialization test passed");
    }
}