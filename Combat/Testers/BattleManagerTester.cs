using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerTester : MonoBehaviour
{
    public BattleManager battleManager;
    public BattleMap testMap;
    public ActorMaker actorMaker;
    public CharacterList testPlayerParty;
    public CharacterList testEnemyParty;
    public MoveCostManager moveManager;
    public AttackManager attackManager;
    
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
    
    [ContextMenu("Run All Battle Tests")]
    public void RunAllBattleTests()
    {
        testResults.Clear();
        Debug.Log("=== Running Battle Manager Tests ===");
        
        TestBattleStateTransitions();
        TestTurnManagement();
        TestActorMovement();
        TestAttackSystem();
        TestSkillActivation();
        TestBattleEndConditions();
        TestMentalStateHandling();
        TestRoundProgression();
        
        LogTestResults();
    }
    
    [ContextMenu("Test Battle State Transitions")]
    public void TestBattleStateTransitions()
    {
        Debug.Log("Testing Battle State Transitions...");
        
        // Test state setting
        battleManager.SetState("Move");
        bool moveStateSet = battleManager.GetState() == "Move";
        testResults.Add(new TestResult("Set Move State", moveStateSet, "Current state: " + battleManager.GetState()));
        
        battleManager.SetState("Attack");
        bool attackStateSet = battleManager.GetState() == "Attack";
        testResults.Add(new TestResult("Set Attack State", attackStateSet, "Current state: " + battleManager.GetState()));
        
        // Test invalid state handling
        battleManager.SetState("InvalidState");
        bool invalidStateHandled = battleManager.GetState() == "";
        testResults.Add(new TestResult("Invalid State Handling", invalidStateHandled, "State after invalid: " + battleManager.GetState()));
        
        // Test state reset
        battleManager.SetState("Move");
        battleManager.SetState("Move"); // Should reset
        bool stateReset = battleManager.GetState() == "";
        testResults.Add(new TestResult("State Reset on Duplicate", stateReset, "State after duplicate: " + battleManager.GetState()));
    }
    
    [ContextMenu("Test Turn Management")]
    public void TestTurnManagement()
    {
        Debug.Log("Testing Turn Management...");
        
        int initialRound = battleManager.GetRoundNumber();
        int initialTurn = battleManager.GetTurnIndex();
        
        // Test round progression
        testResults.Add(new TestResult("Initial Round Number", initialRound >= 0, "Round: " + initialRound));
        testResults.Add(new TestResult("Initial Turn Index", initialTurn >= 0, "Turn: " + initialTurn));
        
        // Test turn actor assignment
        TacticActor currentActor = battleManager.GetTurnActor();
        bool hasCurrentActor = currentActor != null;
        testResults.Add(new TestResult("Current Actor Assignment", hasCurrentActor, "Actor exists: " + hasCurrentActor));
        
        if (hasCurrentActor)
        {
            string actorName = currentActor.GetPersonalName();
            bool hasValidName = !string.IsNullOrEmpty(actorName);
            testResults.Add(new TestResult("Actor Has Valid Name", hasValidName, "Actor name: " + actorName));
        }
    }
    
    [ContextMenu("Test Actor Movement")]
    public void TestActorMovement()
    {
        Debug.Log("Testing Actor Movement...");
        
        if (battleManager.GetTurnActor() == null)
        {
            testResults.Add(new TestResult("Movement Test Setup", false, "No turn actor available"));
            return;
        }
        
        TacticActor actor = battleManager.GetTurnActor();
        int initialLocation = actor.GetLocation();
        int initialSpeed = actor.GetSpeed();
        
        testResults.Add(new TestResult("Actor Initial Location", initialLocation >= 0, "Location: " + initialLocation));
        testResults.Add(new TestResult("Actor Has Speed", initialSpeed >= 0, "Speed: " + initialSpeed));
        
        // Test movement state activation
        battleManager.SetState("Move");
        bool moveStateActive = battleManager.GetState() == "Move";
        testResults.Add(new TestResult("Move State Activation", moveStateActive, "State: " + battleManager.GetState()));
        
        // Test movement with zero speed
        if (initialSpeed == 0)
        {
            battleManager.SetState("Move");
            bool noMoveWithZeroSpeed = battleManager.GetState() == "";
            testResults.Add(new TestResult("No Movement With Zero Speed", noMoveWithZeroSpeed, "State after zero speed move: " + battleManager.GetState()));
        }
    }
    
    [ContextMenu("Test Attack System")]
    public void TestAttackSystem()
    {
        Debug.Log("Testing Attack System...");
        
        if (battleManager.GetTurnActor() == null)
        {
            testResults.Add(new TestResult("Attack Test Setup", false, "No turn actor available"));
            return;
        }
        
        TacticActor actor = battleManager.GetTurnActor();
        
        // Test attack state activation
        battleManager.SetState("Attack");
        bool attackStateActive = battleManager.GetState() == "Attack";
        testResults.Add(new TestResult("Attack State Activation", attackStateActive, "State: " + battleManager.GetState()));
        
        // Test attack cost checking
        int initialActions = actor.GetActions();
        bool hasActions = initialActions > 0;
        testResults.Add(new TestResult("Actor Has Actions", hasActions, "Actions: " + initialActions));
        
        // Test attack range calculation
        if (moveManager != null)
        {
            List<int> attackableTiles = moveManager.GetAttackableTiles(actor, testMap.battlingActors);
            bool hasAttackableTiles = attackableTiles != null;
            testResults.Add(new TestResult("Attackable Tiles Calculation", hasAttackableTiles, "Tiles count: " + (attackableTiles?.Count ?? 0)));
        }
    }
    
    [ContextMenu("Test Skill Activation")]
    public void TestSkillActivation()
    {
        Debug.Log("Testing Skill Activation...");
        
        if (battleManager.GetTurnActor() == null)
        {
            testResults.Add(new TestResult("Skill Test Setup", false, "No turn actor available"));
            return;
        }
        
        // Test skill state activation
        battleManager.SetState("Skill");
        bool skillStateActive = battleManager.GetState() == "Skill";
        testResults.Add(new TestResult("Skill State Activation", skillStateActive, "State: " + battleManager.GetState()));
        
        // Test spell state activation
        battleManager.SetState("Spell");
        bool spellStateActive = battleManager.GetState() == "Spell";
        testResults.Add(new TestResult("Spell State Activation", spellStateActive, "State: " + battleManager.GetState()));
        
        // Test item state activation
        battleManager.SetState("Item");
        bool itemStateActive = battleManager.GetState() == "Item";
        testResults.Add(new TestResult("Item State Activation", itemStateActive, "State: " + battleManager.GetState()));
    }
    
    [ContextMenu("Test Battle End Conditions")]
    public void TestBattleEndConditions()
    {
        Debug.Log("Testing Battle End Conditions...");
        
        if (testMap == null || testMap.battlingActors == null)
        {
            testResults.Add(new TestResult("Battle End Test Setup", false, "No battle map or actors available"));
            return;
        }
        
        // Test actor list validity
        bool hasActors = testMap.battlingActors.Count > 0;
        testResults.Add(new TestResult("Battle Has Actors", hasActors, "Actor count: " + testMap.battlingActors.Count));
        
        // Test team distribution
        if (hasActors)
        {
            int team0Count = 0, team1Count = 0, otherTeamCount = 0;
            foreach (TacticActor actor in testMap.battlingActors)
            {
                if (actor.GetTeam() == 0) team0Count++;
                else if (actor.GetTeam() == 1) team1Count++;
                else otherTeamCount++;
            }
            
            testResults.Add(new TestResult("Team 0 Count", team0Count >= 0, "Team 0: " + team0Count));
            testResults.Add(new TestResult("Team 1 Count", team1Count >= 0, "Team 1: " + team1Count));
            testResults.Add(new TestResult("Other Teams Count", otherTeamCount >= 0, "Other teams: " + otherTeamCount));
            
            bool hasMultipleTeams = (team0Count > 0 && team1Count > 0) || (team0Count > 0 && otherTeamCount > 0) || (team1Count > 0 && otherTeamCount > 0);
            testResults.Add(new TestResult("Multiple Teams Present", hasMultipleTeams, "Battle has opposing teams"));
        }
    }
    
    [ContextMenu("Test Mental State Handling")]
    public void TestMentalStateHandling()
    {
        Debug.Log("Testing Mental State Handling...");
        
        if (battleManager.GetTurnActor() == null)
        {
            testResults.Add(new TestResult("Mental State Test Setup", false, "No turn actor available"));
            return;
        }
        
        TacticActor actor = battleManager.GetTurnActor();
        string mentalState = actor.GetMentalState();
        
        // Test mental state retrieval
        bool hasValidMentalState = mentalState != null;
        testResults.Add(new TestResult("Mental State Retrieval", hasValidMentalState, "Mental state: " + mentalState));
        
        // Test known mental states
        List<string> knownStates = new List<string> { "Normal", "Terrified", "Enraged", "Charmed", "Taunted" };
        bool isKnownState = knownStates.Contains(mentalState) || string.IsNullOrEmpty(mentalState);
        testResults.Add(new TestResult("Known Mental State", isKnownState, "State: " + mentalState));
    }
    
    [ContextMenu("Test Round Progression")]
    public void TestRoundProgression()
    {
        Debug.Log("Testing Round Progression...");
        
        int currentRound = battleManager.GetRoundNumber();
        int currentTurn = battleManager.GetTurnIndex();
        
        // Test round number validity
        bool validRound = currentRound >= 0;
        testResults.Add(new TestResult("Valid Round Number", validRound, "Round: " + currentRound));
        
        // Test turn index validity
        bool validTurn = currentTurn >= 0;
        testResults.Add(new TestResult("Valid Turn Index", validTurn, "Turn: " + currentTurn));
        
        // Test interactable state
        bool isInteractable = battleManager.interactable;
        testResults.Add(new TestResult("Battle Interactable", isInteractable, "Interactable: " + isInteractable));
    }
    
    [ContextMenu("Test Tile Interaction")]
    public void TestTileInteraction()
    {
        Debug.Log("Testing Tile Interaction...");
        
        // Test invalid tile handling
        battleManager.ClickOnTile(-1);
        bool handlesInvalidTile = battleManager.GetState() == "";
        testResults.Add(new TestResult("Invalid Tile Handling", handlesInvalidTile, "State after invalid tile: " + battleManager.GetState()));
        
        // Test valid tile interaction
        if (testMap != null && testMap.currentTiles != null && testMap.currentTiles.Count > 0)
        {
            int validTileIndex = 0;
            for (int i = 0; i < testMap.currentTiles.Count; i++)
            {
                if (testMap.currentTiles[i] >= 0)
                {
                    validTileIndex = i;
                    break;
                }
            }
            
            battleManager.ClickOnTile(validTileIndex);
            // Should handle the tile click without crashing
            testResults.Add(new TestResult("Valid Tile Interaction", true, "Tile index: " + validTileIndex));
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