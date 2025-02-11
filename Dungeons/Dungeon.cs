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
    public int dungeonSize;
    public int GetDungeonSize(){return dungeonSize;}
    public void MakeDungeon()
    {
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
    }
    // List of dungeons and their stats.
    public StatDatabase dungeonData;
    public string dungeonName;
    public void SetDungeonName(string newName)
    {
        dungeonName = newName;
        string[] dungeonInfo = dungeonData.ReturnValue(dungeonName).Split("|");
        currentFloor = 0;
        spawnCounter = 0;
        ResetAllEnemies();
        maxFloors = int.Parse(dungeonInfo[0]);
        type = dungeonInfo[1];
        possibleEnemies = dungeonInfo[2].Split(",").ToList();
        minEnemies = int.Parse(dungeonInfo[3]);
        maxEnemies = int.Parse(dungeonInfo[4]);
    }
    public List<string> treasures;
    public List<string> possibleEnemies;
    public bool fastSpawn = false;
    public int minEnemies;
    public int maxEnemies;
    public string type = "Forest";
    public string passableTileType = "Plains";
    public int maxFloors;
    public int currentFloor;
    public int spawnCounter;
    // QUESTS and other stuff.
    // Store all the floors, incase you can go up or down floors?
    //public List<string> allFloorData;
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
        for (int i = 0; i < allEnemyLocations.Count; i++)
        {
            partyLocations[allEnemyLocations[i]] = allEnemySprites[i];
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
    public List<DungeonTreasure> allTreasure;
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
    public void TryToSpawnEnemy()
    {
        spawnCounter++;
        // If you're lucky enemies will never spawn.
        int spawnCheck = Random.Range(0, (dungeonSize * 6)/(currentFloor + 1));
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
            if (enemyPath.Count < 2)
            {
                allEnemyLocations[i] = partyLocation;
                continue;
            }
            allEnemyLocations[i] = enemyPath[1];
        }
        UpdatePartyLocations();
    }
    protected int FindRandomEmptyTile()
    {
        int tile = -1;
        int tries = dungeonSize * dungeonSize;
        for (int i = 0; i < tries; i++)
        {
            tile = Random.Range(0, partyLocations.Count);
            if (currentFloorTiles[tile] == passableTileType && partyLocations[tile].Length < 1){break;}
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
