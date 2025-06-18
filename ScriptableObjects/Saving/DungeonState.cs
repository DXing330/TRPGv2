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

    public override void NewGame()
    {
        return;
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        // Basic info, dungeon type/size/tiles.
        allData = previousScene + delimiter + dungeon.GetDungeonName() + delimiter + dungeon.GetDungeonSize() + delimiter;
        allData += String.Join(delimiterTwo, dungeon.GetCurrentFloorTiles()) + delimiter;
        // Locations of party/stairs/treasures.
        allData += dungeon.GetPartyLocation() + delimiter + dungeon.GetStairsDown() + delimiter;
        allData += String.Join(delimiterTwo, dungeon.treasureLocations) + delimiter;
        // Locations of enemies.
        allData += String.Join(delimiterTwo, dungeon.allEnemySprites) + delimiter;
        allData += String.Join(delimiterTwo, dungeon.allEnemyParties) + delimiter;
        allData += String.Join(delimiterTwo, dungeon.allEnemyLocations) + delimiter;
        // Current floor.
        allData += dungeon.GetCurrentFloor() + delimiter;
        // Quest/Goal Info // Not always needed.
        allData += dungeon.GetQuestGoal() + delimiter;
        allData += dungeon.GetGoalsCompleted() + delimiter;
        allData += dungeon.GetQuestReward() + delimiter;
        allData += dungeon.GetGoalFloor() + delimiter;
        allData += dungeon.GetGoalTile() + delimiter;
        // Collected treasure count.
        allData += dungeon.GetTreasuresAcquired() + delimiter;
        // Viewed tiles
        allData += String.Join(delimiterTwo, dungeon.GetViewedTiles()) + delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        previousScene = dataList[0];
        dungeon.SetDungeonName(dataList[1], false);
        dungeon.SetDungeonSize(int.Parse(dataList[2]));
        dungeon.LoadFloorTiles(dataList[3].Split(delimiterTwo).ToList());
        dungeon.SetPartyLocation(int.Parse(dataList[4]));
        dungeon.SetStairsDown(int.Parse(dataList[5]));
        dungeon.SetTreasureLocations(dataList[6].Split(delimiterTwo).ToList());
        dungeon.SetEnemies(dataList[7].Split(delimiterTwo).ToList(), dataList[8].Split(delimiterTwo).ToList(), dataList[9].Split(delimiterTwo).ToList());
        dungeon.SetCurrentFloor(int.Parse(dataList[10]));
        dungeon.SetQuestGoal(dataList[11]);
        dungeon.SetGoalsCompleted(int.Parse(dataList[12]));
        dungeon.SetQuestReward(int.Parse(dataList[13]));
        dungeon.SetGoalFloor(int.Parse(dataList[14]));
        dungeon.SetGoalTile(int.Parse(dataList[15]));
        dungeon.SetTreasuresAcquired(int.Parse(dataList[16]));
        dungeon.SetViewedTiles(dataList[17].Split(delimiterTwo).ToList());
    }
}
