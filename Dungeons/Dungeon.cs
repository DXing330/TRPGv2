using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the dungeon information for persistance.
[CreateAssetMenu(fileName = "Dungeon", menuName = "ScriptableObjects/Dungeons/Dungeon", order = 1)]
public class Dungeon : ScriptableObject
{
    public ActorPathfinder pathfinder;
    public DungeonGenerator dungeonGenerator;
    public CharacterList enemyList;
    public PartyData tempParty;
    public string escortName;
    public string GetEscortName(){return escortName;}
    public int dungeonSize;
    public int GetDungeonSize(){return dungeonSize;}
    public void MakeDungeon()
    {
        dungeonGenerator.SetTreasureCount(1+(currentFloor/2));
        List<string> newDungeon = dungeonGenerator.GenerateDungeon();
        dungeonSize = dungeonGenerator.GetSize();
        pathfinder.SetMapSize(dungeonSize);
        currentFloorTiles = newDungeon[0].Split("|").ToList();
        moveCosts.Clear();
        int bigInt = dungeonSize * dungeonSize;
        for (int i = 0; i < currentFloorTiles.Count; i++)
        {
            if (currentFloorTiles[i] == "1")
            {
                currentFloorTiles[i] = type;
                moveCosts.Add(bigInt);
            }
            else
            {
                currentFloorTiles[i] = passableTileType;
                moveCosts.Add(1);
            }
        }
        SetPartyLocation(int.Parse(newDungeon[1]));
        stairsDown = int.Parse(newDungeon[2]);
        List<string> tLoc = newDungeon[3].Split("|").ToList();
        treasureLocations.Clear();
        for (int i = 0; i < tLoc.Count; i++)
        {
            treasureLocations.Add(int.Parse(tLoc[i]));
        }
        goalTile = -1;
        if (currentFloor == goalFloor)
        {
            GenerateQuestGoal();
        }
        InitializeViewedTiles();
    }
    protected void GenerateQuestGoal()
    {
        switch (questGoal)
        {
            case "Search":
            goalTile = FindRandomEmptyTile();
            break;
            case "Defeat":
            goalTile = stairsDown;
            // Spawn a random boss tier enemy on the stairs.
            string[] bossGroups = dungeonBosses.ReturnValue(dungeonName).Split(",");
            // The enemy is based on the dungeon name.
            string bossParty = bossGroups[Random.Range(0, bossGroups.Length)];
            allEnemySprites.Add(bossParty.Split("|")[0]);
            allEnemyLocations.Add(goalTile);
            allEnemyParties.Add(bossParty);
            break;
            case "Rescue":
            goalTile = FindRandomEmptyTile();
            break;
        }
    }
    // List of dungeons and their stats.
    public StatDatabase dungeonData;
    public StatDatabase dungeonBosses;
    // Need to determine what floor the goal is on.
    public string questInfo;
    public string GetQuestInfo(){return questInfo;}
    public int goalFloor;
    public int goalTile;
    public bool GoalTile(int tileNumber)
    {
        if (goalFloor != currentFloor){return false;}
        return tileNumber == goalTile;
    }
    public string questGoal;
    public string GetQuestGoal(){return questGoal;}
    public int questGoalsCompleted;
    public void SetGoalsCompleted(int newAmount){questGoalsCompleted = newAmount;}
    public int GetGoalsCompleted(){return questGoalsCompleted;}
    public int questReward;
    public int GetQuestReward(){return questReward;}
    public void SetQuestInfo(string newQuest)
    {
        questInfo = newQuest;
        goalFloor = Random.Range(currentFloor, maxFloors+1);
        string[] questData = questInfo.Split("|");
        questGoal = questData[0];
        questGoalsCompleted = 0;
        questReward = int.Parse(questData[2]);
    }
    protected void ResetQuest()
    {
        questInfo = "";
        goalFloor = -1;
        questGoalsCompleted = 0;
        questReward = 0;
    }
    public string dungeonName;
    public void SetDungeonName(string newName)
    {
        dungeonName = newName;
        string[] dungeonInfo = dungeonData.ReturnValue(dungeonName).Split("|");
        currentFloor = 0;
        treasuresAcquired = 0;
        spawnCounter = 0;
        ResetAllEnemies();
        ResetQuest();
        maxFloors = int.Parse(dungeonInfo[0]);
        type = dungeonInfo[1];
        possibleEnemies = dungeonInfo[2].Split(",").ToList();
        minEnemies = int.Parse(dungeonInfo[3]);
        maxEnemies = int.Parse(dungeonInfo[4]);
        treasures = dungeonInfo[5].Split(",").ToList();
        maxPossibleTreasureQuantities = dungeonInfo[6].Split(",").ToList();
    }
    public List<string> treasures;
    public List<string> maxPossibleTreasureQuantities;
    public List<string> possibleEnemies;
    public bool fastSpawn = false;
    public int minEnemies;
    public int maxEnemies;
    public int enemyChaseRange = 6;
    public string type = "Forest";
    public string passableTileType = "Plains";
    public int maxFloors;
    public int currentFloor;
    public int treasuresAcquired;
    public int GetTreasuresAcquired(){return treasuresAcquired;}
    public int spawnCounter;
    // QUESTS and other stuff.
    // Store all the floors, incase you can go up or down floors?
    //public List<string> allFloorData;
    public List<bool> viewedTiles;
    protected void InitializeViewedTiles()
    {
        viewedTiles.Clear();
        for (int i = 0; i < GetDungeonSize() * GetDungeonSize(); i++)
        {
            viewedTiles.Add(false);
        }
    }
    public void UpdateViewedTiles(List<int> currentTiles)
    {
        for (int i = 0; i < currentTiles.Count; i++)
        {
            if (currentTiles[i] < 0 || currentTiles[i] >= viewedTiles.Count){continue;}
            viewedTiles[currentTiles[i]] = true;
        }
    }
    public List<string> currentFloorTiles;
    public List<int> moveCosts;
    public void SetFloorTiles(List<string> newTiles, int floor = 0)
    {
        currentFloorTiles = new List<string>(newTiles);
    }
    // Keep an empty list around for reseting.
    public List<string> allEmptyTiles;
    public void UpdateEmptyTiles(List<string> newEmptyTiles)
    {
        allEmptyTiles = new List<string>(newEmptyTiles);
        UpdatePartyLocations();
    }
    public List<string> partyLocations;
    public void UpdatePartyLocations()
    {
        // Make a new list.
        partyLocations = new List<string>(allEmptyTiles);
        for (int i = 0; i < treasureLocations.Count; i++)
        {
            partyLocations[treasureLocations[i]] = treasureSprite;
        }
        for (int i = 0; i < allEnemyLocations.Count; i++)
        {
            partyLocations[allEnemyLocations[i]] = allEnemySprites[i];
        }
        if (currentFloor == goalFloor && goalTile >= 0)
        {
            switch (questGoal)
            {
                case "Search":
                partyLocations[goalTile] = "Necklace";
                break;
            }
        }
        partyLocations[partyLocation] = partySprite;
        partyLocations[stairsDown] = stairsDownSprite;
    }
    protected void ResetAllEnemies()
    {
        allEnemySprites.Clear();
        allEnemyParties.Clear();
        allEnemyLocations.Clear();
    }
    protected void RemoveEnemyAtIndex(int indexOf)
    {
        allEnemySprites.RemoveAt(indexOf);
        allEnemyParties.RemoveAt(indexOf);
        allEnemyLocations.RemoveAt(indexOf);
    }
    public List<string> allEnemySprites;
    public List<string> allEnemyParties;
    public List<int> allEnemyLocations;
    public bool EnemyLocation(int nextTile)
    {
        return allEnemyLocations.Contains(nextTile);
    }
    public List<int> treasureLocations;
    public bool TreasureLocation(int nextTile)
    {
        return treasureLocations.Contains(nextTile);
    }
    public void ClaimTreasure()
    {
        for (int i = treasureLocations.Count- 1; i >= 0; i--)
        {
            if (treasureLocations[i] == partyLocation)
            {
                treasureLocations.RemoveAt(i);
                treasuresAcquired++;
            }
        }
    }
    public string treasureSprite;
    public int stairsDown;
    public bool StairsDownLocation(int nextTile)
    {
        return nextTile == stairsDown;
    }
    public bool FinalFloor()
    {
        return currentFloor == maxFloors;
    }
    public string stairsDownSprite = "LadderDown";
    public int stairsUp;
    public int partyLocation;
    public string partySprite = "Player";
    public int GetPartyLocation(){return partyLocation;}
    public void SetPartyLocation(int newLocation)
    {
        partyLocation = newLocation;
    }
    public void MovePartyLocation(int newLocation)
    {
        // Remove the old location.
        partyLocations[partyLocation] = "";
        partyLocation = newLocation;
        partyLocations[partyLocation] = partySprite;
        if (TreasureLocation(partyLocation))
        {
            ClaimTreasure();
        }
        if (GoalTile(partyLocation))
        {
            questGoalsCompleted++;
            goalTile = -1;
        }
        MoveEnemies();
        TryToSpawnEnemy();
        if (partyLocation == stairsDown){MoveFloors();}
    }
    public void PrepareBattle(int tileLocation)
    {
        // Determine which enemy party to use.
        int indexOf = allEnemyLocations.IndexOf(tileLocation);
        if (indexOf == -1){return;}
        // Use that enemy to determine the enemy party.
        enemyList.SetLists(allEnemyParties[indexOf].Split("|").ToList());
        // Remove the enemy on that location.
        RemoveEnemyAtIndex(indexOf);
    }
    public void EnemyBeginsBattle()
    {
        int enemyLocation = -1;
        enemyList.ResetLists();
        for (int i = allEnemySprites.Count - 1; i >= 0; i--)
        {
            enemyLocation = allEnemyLocations[i];
            if (enemyLocation == partyLocation)
            {
                enemyList.AddCharacters(allEnemyParties[i].Split("|").ToList());
                RemoveEnemyAtIndex(i);
            }
        }
    }
    public bool TilePassable(int tileNumber)
    {
        return currentFloorTiles[tileNumber] == passableTileType;
    }
    public bool TileEmpty(int tileNumber)
    {
        return (TilePassable(tileNumber) && partyLocations[tileNumber] == "");
    }
    public void TryToSpawnEnemy()
    {
        spawnCounter++;
        // If you're lucky enemies will never spawn.
        int spawnCheck = Random.Range(0, (dungeonSize * 6) + spawnCounter);
        if (fastSpawn){spawnCheck = spawnCheck/10;}
        if (spawnCheck < spawnCounter)
        {
            SpawnEnemy();
            spawnCounter = 0;
        }
    }
    protected void SpawnEnemy()
    {
        int spawnLocation = FindRandomEmptyTile();
        if (spawnLocation == -1){return;}
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        string enemyString = "";
        for (int i = 0; i < enemyCount; i++)
        {
            enemyString += possibleEnemies[Random.Range(0, possibleEnemies.Count)];
            // The first one is the sprite.
            if (i < enemyCount - 1){enemyString += "|";}
        }
        string[] allEnemies = enemyString.Split("|");
        allEnemySprites.Add(allEnemies[0]);
        allEnemyLocations.Add(spawnLocation);
        allEnemyParties.Add(enemyString);
        partyLocations[spawnLocation] = allEnemySprites[allEnemySprites.Count - 1];
    }
    protected void MoveEnemies()
    {
        if (allEnemySprites.Count <= 0){return;}
        pathfinder.FindPaths(partyLocation, moveCosts);
        List<int> enemyPath = new List<int>();
        int currentEnemyLocation = -1;
        for (int i = allEnemySprites.Count-1; i >= 0; i--)
        {
            // Move toward the player.
            currentEnemyLocation = allEnemyLocations[i];
            enemyPath = pathfinder.GetPrecomputedPath(partyLocation, currentEnemyLocation);
            if (enemyPath.Count >= enemyChaseRange)
            {
                // Maybe move in a random direction?
                continue;
            }
            if (enemyPath.Count < 2)
            {
                allEnemyLocations[i] = partyLocation;
                continue;
            }
            allEnemyLocations[i] = enemyPath[1];
        }
        CombineEnemies();
        UpdatePartyLocations();
    }
    protected void CombineEnemies()
    {
        int enemyCount = allEnemyLocations.Count;
        for (int i = enemyCount - 1; i > 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                if (j >= allEnemyLocations.Count || i >= allEnemyLocations.Count){break;}
                if (allEnemyLocations[j] == allEnemyLocations[i])
                {
                    allEnemyLocations.RemoveAt(i);
                    allEnemyParties[j] += "|"+allEnemyParties[i];
                    allEnemyParties.RemoveAt(i);
                    allEnemySprites.RemoveAt(i);
                    continue;
                }
            }
        }
    }
    protected int FindRandomEmptyTile()
    {
        int tile = -1;
        int tries = dungeonSize * dungeonSize;
        for (int i = 0; i < tries; i++)
        {
            tile = Random.Range(0, partyLocations.Count);
            if (TileEmpty(tile)){break;}
            else{tile = -1;}
        }
        return tile;
    }
    public void MoveFloors(bool increase = true)
    {
        if (increase)
        {
            currentFloor++;
            if (currentFloor > maxFloors)
            {
                CompleteDungeon();
                return;
            }
        }
        MakeDungeon();
        ResetAllEnemies();
        spawnCounter = 0;
        UpdatePartyLocations();
    }
    public void CompleteDungeon()
    {
        // move to dungeon reward scene.
    }
}
