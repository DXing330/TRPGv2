using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSState", menuName = "ScriptableObjects/StS/StSState", order = 1)]
public class StSState : SavedState
{
    public StSSettings settings;
    public List<SavedData> stsData;
    public void SaveAllData()
    {
        for (int i = 0; i < stsData.Count; i++)
        {
            stsData[i].Save();
        }
    }
    public void LoadAllData()
    {
        for (int i = 0; i < stsData.Count; i++)
        {
            stsData[i].Load();
        }
    }
    // Enemy tracker is just an extension of the state, sectioned off for convenience.
    public StSEnemyTracker enemyTracker;
    // Save the map, the floor, the party location, the store items, the random event details.
    public override void NewGame()
    {
        newGame = 1;
        currentFloor = 1;
        currentState = "Moving";
        currentStateSpecifics = "";
        battlesFought = 0;
        bossBattled = 0;
        for (int i = 0; i < stsData.Count; i++)
        {
            stsData[i].NewGame();
        }
        NewRandomSeed();
    }
    // SEEDS
    public RNGUtility masterSeed;
    public RNGUtility mapGenSeed;
    public RNGUtility enemySeed;
    public RNGUtility rewardSeed;
    public void NewRandomSeed()
    {
        masterSeed.RandomSeed();
        mapGenSeed.SetSeed(masterSeed.GetSeed());
        enemySeed.SetSeed(masterSeed.GetSeed());
        rewardSeed.SetSeed(masterSeed.GetSeed());
        Save();
    }
    public int newGame = 1;
    public int currentFloor = 1;
    // TODO track and implement states, all will change the map behavior.
    // Moving/Event/Rewards/Shopping/Resting?
    // Shopping and resting are stored in different scenes probably.
    public string currentState;
    // Determines events/rewards/etc.
    public string currentStateSpecifics;
    public void ReturnFromWinningBattle()
    {
        currentState = "Reward";
        // The Specifics (Event/Battle/Elite/Boss) should already be stored.
        Save();
    }
    public StatDatabase bossFightsPerFloorData;
    public int GetBossFightsPerFloor()
    {
        int difficulty = settings.GetDifficulty();
        for (int i = difficulty; i > 0; i--)
        {
            if (bossFightsPerFloorData.KeyExists(GetCurrentFloor() + "-" + i))
            {
                return int.Parse(bossFightsPerFloorData.ReturnValue(GetCurrentFloor() + "-" + i));
            }
        }
        // Based on floor and difficulty;
        return 1;
    }
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
    }
    public string floorBoss;
    public int bossBattled = 0;
    public void BattleBoss()
    {
        bossBattled++;
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
        if (bossBattled >= GetBossFightsPerFloor())
        {
            return "Boss";
        }
        else if (bossBattled > 0)
        {
            return "";
        }
        else if (partyPathing.Count <= 0)
        {
            return "";
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
        currentState = map.state;
        currentStateSpecifics = map.stateSpecifics;
        Save();
    }
    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = "NG=" + newGame + delimiter;
        allData += "Map=" + String.Join(delimiterTwo, mapInfo) + delimiter;
        allData += "Path=" + String.Join(delimiterTwo, partyPathing) + delimiter;
        allData += "Floor=" + currentFloor + delimiter;
        allData += "Battles=" + battlesFought + delimiter;
        allData += "Boss=" + floorBoss + delimiter;
        allData += "BossBattled=" + bossBattled + delimiter;
        allData += "State=" + currentState + delimiter;
        allData += "StateSpecifics=" + currentStateSpecifics + delimiter;
        File.WriteAllText(dataPath, allData);
        SaveAllData();
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            LoadStat(dataList[i]);
        }
        LoadAllData();
    }
    protected void LoadStat(string stat)
    {
        string[] blocks = stat.Split('=');
        if (blocks.Length < 2) { return; }
        string key = blocks[0];
        string value = blocks[1];
        switch (key)
        {
            case "NG":
                newGame = int.Parse(value);
                break;
            case "Map":
                mapInfo = new List<string>(value.Split(delimiterTwo));
                break;
            case "Path":
                if (value.Length <= 0)
                {
                    partyPathing = new List<int>();
                }
                else
                {
                    partyPathing = utility.ConvertStringListToIntList(value.Split(delimiterTwo).ToList());
                }
                break;
            case "Floor":
                currentFloor = int.Parse(value);
                break;
            case "Battles":
                battlesFought = int.Parse(value);
                break;
            case "Boss":
                floorBoss = value;
                break;
            case "BossBattled":
                bossBattled = int.Parse(value);
                break;
            case "State":
                currentState = value;
                break;
            case "StateSpecifics":
                currentStateSpecifics = value;
                break;
        }
    }
}
