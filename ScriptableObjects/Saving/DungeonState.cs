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
        allData += dungeon.GetBossFought() + delimiter;
        allData += dungeon.GetMaxStomach() + delimiter;
        allData += dungeon.GetStomach() + delimiter;
        allData += String.Join(delimiterTwo, dungeon.itemLocations) + delimiter;
        allData += String.Join(delimiterTwo, dungeon.trapLocations) + delimiter;
        allData += dungeon.GetWeather() + delimiter;
        allData += String.Join(delimiterTwo, dungeon.partyModifiers) + delimiter;
        allData += String.Join(delimiterTwo, dungeon.partyModifierDurations) + delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            LoadStat(dataList[i], i);
        }
    }

    protected void LoadStat(string stat, int index)
    {
        switch (index)
        {
            case 0:
                previousScene = stat;
                break;
            case 1:
                dungeon.SetDungeonName(stat, false);
                break;
            case 2:
                dungeon.SetDungeonSize(int.Parse(stat));
                break;
            case 3:
                dungeon.LoadFloorTiles(stat.Split(delimiterTwo).ToList());
                break;
            case 4:
                dungeon.SetPartyLocation(int.Parse(stat));
                break;
            case 5:
                dungeon.SetStairsDown(int.Parse(stat));
                break;
            case 6:
                dungeon.SetTreasureLocations(stat.Split(delimiterTwo).ToList());
                break;
            case 7:
                dungeon.SetEnemySprites(stat.Split(delimiterTwo).ToList());
                break;
            case 8:
                dungeon.SetEnemyParties(stat.Split(delimiterTwo).ToList());
                break;
            case 9:
                dungeon.SetEnemyLocations(stat.Split(delimiterTwo).ToList());
                break;
            case 10:
                dungeon.SetCurrentFloor(int.Parse(stat));
                break;
            case 11:
                dungeon.SetQuestGoal(stat);
                break;
            case 12:
                dungeon.SetGoalsCompleted(int.Parse(stat));
                break;
            case 13:
                dungeon.SetQuestReward(int.Parse(stat));
                break;
            case 14:
                dungeon.SetGoalFloor(int.Parse(stat));
                break;
            case 15:
                dungeon.SetGoalTile(int.Parse(stat));
                break;
            case 16:
                dungeon.SetTreasuresAcquired(int.Parse(stat));
                break;
            case 17:
                dungeon.SetViewedTiles(stat.Split(delimiterTwo).ToList());
                break;
            case 18:
                dungeon.SetBossFought(int.Parse(stat));
                break;
            case 19:
                dungeon.SetMaxStomach(int.Parse(stat));
                break;
            case 20:
                dungeon.SetStomach(int.Parse(stat));
                break;
            case 21:
                dungeon.SetItemLocations(stat.Split(delimiterTwo).ToList());
                break;
            case 22:
                dungeon.SetTrapLocations(stat.Split(delimiterTwo).ToList());
                break;
            case 23:
                dungeon.SetWeather(stat);
                break;
            case 24:
                dungeon.SetPartyBattleModifiers(stat.Split(delimiterTwo).ToList());
                break;
            case 25:
                dungeon.SetPartyBattleModifierDurations(stat.Split(delimiterTwo).ToList());
                break;
            default:
                break;
        }
    }
}
