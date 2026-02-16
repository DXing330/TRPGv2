using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SavedBattles", menuName = "ScriptableObjects/DataContainers/SavedData/SavedBattles", order = 1)]
public class BattleMapEditorSaver : MapEditorSaver
{
    public string mapEnemyDelim;
    public void SaveBattle(BattleMapEditor bMap, string battleName)
    {
        // Later can search and filter by key name.
        if (AddKey(battleName))
        {
            SaveKeys();
        }
        dataPath = Application.persistentDataPath + "/" + filename + battleName;
        allData = "";
        allData += ReturnSaveMapDataString(bMap.mapEditor);
        // Keep track of the different between the saved map and the saved enemies. 
        allData += mapEnemyDelim;
        allData += "Enemies=" + String.Join(delimiterTwo, bMap.enemies) + delimiter;
        allData += "EnemyLocations=" + String.Join(delimiterTwo, bMap.enemyLocations);
        File.WriteAllText(dataPath, allData);
    }
    public void LoadBattle(BattleMapEditor bMap, string battleName)
    {
        dataPath = Application.persistentDataPath + "/" + filename + battleName;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else
        {
            bMap.InitializeNewMap();
            return;
        }
        string[] mapEnemyData = allData.Split(mapEnemyDelim);
        string[] mapData = mapEnemyData[0].Split(delimiter);
        for (int i = 0; i < mapData.Length; i++)
        {
            if (!LoadMapStat(mapData[i], bMap.mapEditor))
            {
                bMap.InitializeNewMap();
                return;
            }
        }
        string[] enemyData = mapEnemyData[1].Split(delimiter);
        for (int i = 0; i < enemyData.Length; i++)
        {
            if (!LoadBattleStat(enemyData[i], bMap))
            {
                bMap.InitializeNewMap();
                return;
            }
        }
        bMap.mapEditor.UndoEdits();
        bMap.UpdateMap();
    }
    protected virtual bool LoadBattleStat(string data, BattleMapEditor map)
    {
        string[] blocks = data.Split("=");
        if (blocks.Length < 2){return false;}
        string value = blocks[1];
        switch (blocks[0])
        {
            default:
                return false;
            case "Enemies":
                map.enemies = value.Split(delimiterTwo).ToList();
                utility.RemoveEmptyListItems(map.enemies);
                return true;
            case "EnemyLocations":
                map.enemyLocations = value.Split(delimiterTwo).ToList();
                utility.RemoveEmptyListItems(map.enemyLocations);
                return true;
        }
    }
}
