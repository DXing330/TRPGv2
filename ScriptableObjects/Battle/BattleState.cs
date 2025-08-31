using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleState", menuName = "ScriptableObjects/DataContainers/SavedData/BattleState", order = 1)]
public class BattleState : SavedState
{
    public CharacterList enemyList;
    public OverworldState overworldState;
    public BattleMapFeatures battleMapFeatures;
    public List<string> enemies;
    public void AddEnemyName(string newName){enemies.Add(newName);}
    public void SetEnemyNames(List<string> newEnemies){enemies = new List<string>(newEnemies);}
    public void UpdateEnemyNames(){enemies = new List<string>(enemyList.characters);}
    public string terrainType;
    public List<string> terrainTypes;
    public virtual void ForceTerrainType(string newInfo)
    {
        terrainType = newInfo;
        battleMapFeatures.SetTerrainType(terrainType);
    }
    public virtual void SetTerrainType()
    {
        if (overworldState == null)
        {
            return;
        }
        terrainType = overworldState.GetLocationTerrain();
        battleMapFeatures.SetTerrainType(terrainType);
    }
    public virtual string GetTerrainType(){return terrainType;}
    public void UpdateTerrainType(){battleMapFeatures.SetTerrainType(terrainType);}
    public List<string> timeOfDayTypes;
    public virtual string GetTime()
    {
        if (overworldState == null)
        {
            return timeOfDayTypes[UnityEngine.Random.Range(0, timeOfDayTypes.Count)];
        }
        return overworldState.GetTime();
    }
    public List<string> weatherTypes;
    public virtual string GetWeather()
    {
        if (overworldState == null)
        {
            return weatherTypes[UnityEngine.Random.Range(0, weatherTypes.Count)];
        }
        return overworldState.GetWeather();
    }

    public override void NewGame()
    {
        base.NewGame();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = previousScene+delimiter;
        for (int i = 0; i < enemies.Count; i++)
        {
            allData += enemies[i];
            if (i < enemies.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        allData += terrainType;
        allData += delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        previousScene = dataList[0];
        enemies = dataList[1].Split(delimiterTwo).ToList();
        terrainType = dataList[2];
        sceneTracker.SetPreviousScene(previousScene);
        enemyList.ResetLists();
        enemyList.AddCharacters(enemies);
        battleMapFeatures.SetTerrainType(terrainType);
    }
}
