using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "ScriptableObjects/Dungeons/Dungeon", order = 1)]
public class Dungeon : ScriptableObject
{
    public DungeonGenerator dungeonGenerator;
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
        partyLocation = int.Parse(newDungeon[1]);
        stairsDown = int.Parse(newDungeon[2]);
        UpdatePartyLocations();
    }
    // List of dungeons and their stats.
    //public StatDatabase dungeonData;
    public string type = "Forest";
    public string passableTileType = "Plains";
    public int maxFloors;
    public int currentFloor;
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
        if (newLocation == stairsDown){MoveFloors();}
    }

    public bool TilePassable(int tileNumber)
    {
        return currentFloorTiles[tileNumber] == passableTileType;
    }

    public void SpawnEnemy(){}

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
    }

    public void CompleteDungeon()
    {
        // move to dungeon reward scene.
    }
}
