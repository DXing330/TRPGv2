using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonState", menuName = "ScriptableObjects/DataContainers/SavedData/DungeonState", order = 1)]
public class DungeonState : SavedState
{
    public OverworldState overworldState;
    public Dungeon dungeon;
    public string dungeonName;

    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        // Basic info, dungeon type/size/tiles.
        allData = previousScene + delimiter + dungeon.GetDungeonName() + delimiter + dungeon.GetDungeonSize() + delimiter;
        allData += String.Join(delimiterTwo, dungeon.currentFloorTiles) + delimiter;
        // Locations of party/stairs/treasures.
        allData += dungeon.GetPartyLocation() + delimiter + dungeon.GetStairsDown() + delimiter;
        allData += String.Join(delimiterTwo, dungeon.treasureLocations) + delimiter;
        // Locations of enemies.
        allData += String.Join(delimiterTwo, dungeon.allEnemySprites) + delimiter;
        allData += String.Join(delimiterTwo, dungeon.allEnemyParties) + delimiter;
        allData += String.Join(delimiterTwo, dungeon.allEnemyLocations) + delimiter;
        // Quest/Goal Info // Not always needed.
        
        //allData += dungeon.GetGoalTile();
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        previousScene = dataList[0];
        dungeon.SetDungeonName(dataList[1], false);
        dungeon.SetDungeonSize(int.Parse(dataList[2]));
        dungeon.SetFloorTiles(dataList[3].Split(delimiterTwo).ToList());
        dungeon.SetPartyLocation(int.Parse(dataList[4]));
        dungeon.SetStairsDown(int.Parse(dataList[5]));
        dungeon.SetTreasureLocations(dataList[6].Split(delimiterTwo).ToList());
        dungeon.SetEnemies(dataList[7].Split(delimiterTwo).ToList(), dataList[8].Split(delimiterTwo).ToList(), dataList[9].Split(delimiterTwo).ToList());
        //dungeon.SetGoalTile(int.Parse(dataList[7]));
    }
}
