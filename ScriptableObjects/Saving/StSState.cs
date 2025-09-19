using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSState", menuName = "ScriptableObjects/StS/StSState", order = 1)]
public class StSState : SavedState
{
    public List<SavedData> stsData;
    // Enemy tracker is just an extension of the state, sectioned off for convenience.
    public StSEnemyTracker enemyTracker;
    // Save the map, the floor, the party location, the store items, the random event details.
    public override void NewGame()
    {
        newGame = 1;
        currentFloor = 1;
        battlesFought = 0;
        bossBattled = 0;
        Save();
        for (int i = 0; i < stsData.Count; i++)
        {
            stsData[i].NewGame();
            stsData[i].Save();
        }
    }
    public int newGame = 1;
    public int currentFloor = 1;
    public int GetCurrentFloor()
    {
        return currentFloor;
    }
    public void CompleteFloor()
    {
        currentFloor++;
        battlesFought = 0;
        bossBattled = 0;
        Save();
    }
    public int battlesFought = 0;
    public void CompleteBattle()
    {
        battlesFought++;
        Save();
    }
    public string floorBoss;
    public int bossBattled = 0;
    public void BattleBoss()
    {
        bossBattled = 1;
    }
    public void SetFloorBoss(string newInfo)
    {
        floorBoss = newInfo;
    }
    public string GetFloorBoss()
    {
        return floorBoss;
    }
    public int ReturnCurrentDifficulty()
    {
        return battlesFought;
    }
    public bool StartingNewGame()
    {
        return newGame == 1;
    }
    public List<string> mapInfo;
    public string ReturnCurrentTile()
    {
        if (partyPathing.Count <= 0)
        {
            return "";
        }
        else if (bossBattled == 1)
        {
            return "Boss";
        }
        return mapInfo[partyPathing[partyPathing.Count - 1]];
    }
    public List<int> partyPathing;
    public int eventIndex;
    public void SetDataFromMap(StSLikeMap map)
    {
        newGame = 0;
        mapInfo = new List<string>(map.mapInfo);
        partyPathing = new List<int>(map.partyPathing);
        floorBoss = map.floorBoss;
        Save();
    }
    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = newGame + delimiter + String.Join(delimiterTwo, mapInfo) + delimiter + String.Join(delimiterTwo, partyPathing) + delimiter + currentFloor + delimiter + battlesFought + delimiter + floorBoss + delimiter + bossBattled;
        File.WriteAllText(dataPath, allData);
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        newGame = int.Parse(dataList[0]);
        mapInfo = dataList[1].Split(delimiterTwo).ToList();
        if (dataList[2].Length <= 0)
        {
            partyPathing = new List<int>();
        }
        else
        {
            partyPathing = utility.ConvertStringListToIntList(dataList[2].Split(delimiterTwo).ToList());
        }
        currentFloor = int.Parse(dataList[3]);
        battlesFought = int.Parse(dataList[4]);
        floorBoss = dataList[5];
        bossBattled = int.Parse(dataList[6]);
    }
}
