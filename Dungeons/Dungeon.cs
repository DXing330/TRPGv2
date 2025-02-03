using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "ScriptableObjects/Dungeons/Dungeon", order = 1)]
public class Dungeon : ScriptableObject
{
    // List of dungeons and their stats.
    //public StatDatabase dungeonData;
    public string type;
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
        // Hardcoded KEK.
        partyLocations[partyLocation] = partySprite;
    }
    public List<DungeonEnemy> allEnemies;
    public List<DungeonTreasure> allTreasure;
    public int currentStairsDown;
    public int currentStairsUp;
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
    }

    public void SpawnEnemy(){}

    public void MoveFloors(bool increase = true){}

    public void CompleteDungeon(){}
}
