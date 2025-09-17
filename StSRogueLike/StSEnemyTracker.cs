using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSEnemyTracker", menuName = "ScriptableObjects/StS/StSEnemyTracker", order = 1)]
public class StSEnemyTracker : SavedData
{
    public string delimiterTwo;
    public StSState stsState;
    public List<StatDatabase> floorEnemies;
    public List<StatDatabase> floorElites;
    public List<StatDatabase> floorBosses;
    public List<string> enemyPool;
    public List<string> elitePool;
    public string previousElite;
    public string floorBoss;

    public override void NewGame()
    {
        enemyPool.Clear();
        elitePool.Clear();
        floorBoss = "";
        previousElite = "";
        Save();
    }

    public void NewFloor()
    {
        int floor = stsState.GetCurrentFloor();
        previousElite = "";
        enemyPool = floorEnemies[floor - 1].ReturnAllKeys();
        elitePool = floorElites[floor - 1].ReturnAllKeys();
        floorBoss = floorBosses[floor - 1].ReturnRandomKey();
        Save();
    }

    public List<string> GetBossData()
    {
        int floor = stsState.GetCurrentFloor();
        return floorBosses[floor - 1].ReturnValue(floorBoss).Split("-").ToList();
    }

    public string GetEliteData()
    {
        string eliteData = elitePool[UnityEngine.Random.Range(0, elitePool.Count)];
        if (eliteData == previousElite)
        {
            return GetEliteData();
        }
        previousElite = eliteData;
        Save();
        return eliteData;
    }

    public string GetEnemyData(int difficulty)
    {
        int floor = stsState.GetCurrentFloor();
        List<string> possibleEnemies = new List<string>();
        for (int i = 0; i < enemyPool.Count; i++)
        {
            if (int.Parse(floorEnemies[floor - 1].ReturnValue(enemyPool[i])) == difficulty)
            {
                possibleEnemies.Add(enemyPool[i]);
            }
        }
        if (possibleEnemies.Count <= 0 && difficulty > 0)
        {
            return GetEnemyData(difficulty - 1);
        }
        else if (possibleEnemies.Count <= 0 && difficulty <= 0)
        {
            return enemyPool[UnityEngine.Random.Range(0, enemyPool.Count)];
        }
        string enemyData = possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Count)];
        enemyPool.Remove(enemyData);
        Save();
        return enemyData;
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = "";
        allData += String.Join(delimiterTwo, enemyPool) + delimiter;
        allData += String.Join(delimiterTwo, elitePool) + delimiter;
        allData += previousElite + delimiter;
        allData += floorBoss + delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(dataPath))
        {
            allData = File.ReadAllText(dataPath);
        }
        else
        {
            // Pretend you're entering a new floor.
            return;
        }
        string[] blocks = allData.Split(delimiter);
        enemyPool = blocks[0].Split(delimiterTwo).ToList();
        elitePool = blocks[1].Split(delimiterTwo).ToList();
        previousElite = blocks[2];
        floorBoss = blocks[3];
    }
}
