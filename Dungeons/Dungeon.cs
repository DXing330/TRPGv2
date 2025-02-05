using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the dungeon information for persistance.
[CreateAssetMenu(fileName = "Dungeon", menuName = "ScriptableObjects/Dungeons/Dungeon", order = 1)]
public class Dungeon : ScriptableObject
{
    public DungeonGenerator dungeonGenerator;
    public DungeonEnemy enemyPrefab;
    public int dungeonSize;
    public int GetDungeonSize(){return dungeonSize;}
    public void MakeDungeon()
    {
        List<string> newDungeon = dungeonGenerator.GenerateDungeon();
        dungeonSize = dungeonGenerator.GetSize();
        currentFloorTiles = newDungeon[0].Split("|").ToList();
        for (int i = 0; i < currentFloorTiles.Count; i++)
        {
            if (currentFloorTiles[i] == "1")
            {
                currentFloorTiles[i] = type;
            }
            else
            {
                currentFloorTiles[i] = passableTileType;
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
        maxFloors = int.Parse(dungeonInfo[0]);
        type = dungeonInfo[1];
        possibleEnemies = dungeonInfo[2].Split(",").ToList();
        minEnemies = int.Parse(dungeonInfo[3]);
        maxEnemies = int.Parse(dungeonInfo[4]);
    }
    public List<string> treasures;
    public List<string> possibleEnemies;
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
        for (int i = 0; i < allEnemies.Count; i++)
        {
            partyLocations[allEnemies[i].GetLocation()] = allEnemies[i].GetSpriteName();
        }
        partyLocations[partyLocation] = partySprite;
        partyLocations[stairsDown] = stairsDownSprite;
    }
    public List<DungeonEnemy> allEnemies;
    public List<string> allEnemyStrings;
    public List<DungeonTreasure> allTreasure;
    public int stairsDown;
    public bool stairsDownLocation(int nextTile)
    {
        return nextTile == stairsDown;
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
        partyLocations[newLocation] = partySprite;
        partyLocation = newLocation;
        TryToSpawnEnemy();
        if (newLocation == stairsDown){MoveFloors();}
    }
    public bool TilePassable(int tileNumber)
    {
        return currentFloorTiles[tileNumber] == passableTileType;
    }
    public void TryToSpawnEnemy()
    {
        spawnCounter++;
        int spawnCheck = Random.Range(0, dungeonSize);
        if (spawnCheck < spawnCounter)
        {
            SpawnEnemy();
            spawnCounter = 0;
        }
    }
    protected void SpawnEnemy()
    {
        int spawnLocation = FindEmptyTile();
        if (spawnLocation == -1){return;}
        DungeonEnemy newEnemy = Instantiate(enemyPrefab, new Vector3(0,0,0), new Quaternion(0, 0, 0, 0));
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        for (int i = 0; i < enemyCount; i++)
        {
            newEnemy.AddEnemy(possibleEnemies[Random.Range(0, possibleEnemies.Count)]);
        }
        newEnemy.SetLocation(spawnLocation);
        partyLocations[spawnLocation] = newEnemy.GetSpriteName();
        allEnemies.Add(newEnemy);
    }
    protected int FindEmptyTile()
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
        allEnemies.Clear();
        spawnCounter = 0;
        UpdatePartyLocations();
    }
    public void CompleteDungeon()
    {
        // move to dungeon reward scene.
    }
}
