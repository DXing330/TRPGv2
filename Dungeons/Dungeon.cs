using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "ScriptableObjects/Dungeons/Dungeon", order = 1)]
public class Dungeon : ScriptableObject
{
    // List of dungeons and their stats.
    //public StatDatabase dungeonData;
    public int maxFloors;
    public int currentFloor;
    // QUESTS and other stuff.
    // Store all the floors, incase you can go up or down floors?
    //public List<string> allFloorData;
    public List<string> currentFloorTiles;
    public List<string> currentEnemies;
    public List<string> currentEnemyLocations;
    public int currentStairsDown;
    public int currentStairsUp;

    public void SpawnEnemy(){}

    public void MoveFloors(bool increase = true){}
}
