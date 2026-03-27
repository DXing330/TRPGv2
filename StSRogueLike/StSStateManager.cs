using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StSStateManager : MonoBehaviour
{
    // New Game/Load.
    void Start()
    {
        if (gameState.StartingNewGame())
        {
            NewGame();
        }
        else
        {
            Load();
        }
    }
    // SUBMANAGERS
    public SceneMover sceneMover;
    public PartyDataManager stsParty;
    public StSMap map;
    public StSState gameState;
    public StSEnemyTracker enemyTracker;
    public RNGUtility masterRNG;
    public BattleState battleState; // Needed To Save Enemies To A Battle.
    // STATE DATA
    public List<SavedData> stsSavedState;
    public List<RNGUtility> stsRNG;
    public void NewGame()
    {
        // First RNG, Then State, Then Enemy Tracker, Then Map
        masterRNG.NewGame();
        gameState.SetUpNewGame();
        enemyTracker.NewGame();
        map.NewGame();
    }
    public void Save()
    {
        for (int i = 0; i < stsSavedState.Count; i++)
        {
            stsSavedState[i].Save();
        }
        for (int i = 0; i < stsRNG.Count; i++)
        {
            stsRNG[i].Save();
        }
        stsParty.Save();
    }
    public void Load()
    {
        for (int i = 0; i < stsSavedState.Count; i++)
        {
            stsSavedState[i].Load();
        }
        for (int i = 0; i < stsRNG.Count; i++)
        {
            stsRNG[i].Load();
        }
        stsParty.Load();
        if (map != null)
        {
            map.Load();
        }
    }
    // STATE FUNCTIONS.
    public void MoveToTile(string tileType)
    {
        string newScene = "";
        switch (tileType)
        {
            case "Enemy":
            break;
            case "Event":
            break;
            case "Elite":
            break;
            case "Shop":
            break;
            case "Rest":
            break;
            case "Treasure":
            break;
        }
        if (newScene != "")
        {
            MoveScenes(newScene);
        }
    }
    public void CompleteBattle(string battleType)
    {
    }
    public void CompleteFloor(int floor)
    {
    }
    public void MoveScenes(string newScene)
    {
        Save();
        sceneMover.LoadScene(newScene);
    }
}
