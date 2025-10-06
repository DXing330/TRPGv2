using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleState", menuName = "ScriptableObjects/DataContainers/SavedData/BattleState", order = 1)]
public class BattleState : SavedState
{
    public bool subGame = false;
    public void SetBattleModifiers()
    {
        allyBattleModifiers = partyList.GetBattleModifiers();
        enemyBattleModifiers = enemyList.GetBattleModifiers();
    }
    public List<string> allyBattleModifiers;
    public List<string> GetAllyBattleModifiers()
    {
        if (!subGame)
        {
            return new List<string>();
        }
        return allyBattleModifiers;
    }
    public List<string> enemyBattleModifiers;
    public List<string> GetEnemyBattleModifiers()
    {
        return enemyBattleModifiers;
    }
    public int winningTeam = -1;
    public void SetWinningTeam(int newInfo)
    {
        winningTeam = newInfo;
        Save();
    }
    public void ResetWinningTeam()
    {
        winningTeam = -1;
        Save();
    }
    public int GetWinningTeam()
    {
        return winningTeam;
    }
    public CharacterList partyList;
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
        allData += String.Join(delimiterTwo, enemies);
        allData += delimiter;
        allData += terrainType;
        allData += delimiter;
        allData += winningTeam;
        allData += delimiter;
        // Whenever moving to a battle scene, the battle modifiers should already be determined.
        SetBattleModifiers();
        allData += String.Join(delimiterTwo, allyBattleModifiers);
        allData += delimiter;
        allData += String.Join(delimiterTwo, enemyBattleModifiers);
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
        winningTeam = int.Parse(dataList[3]);
        allyBattleModifiers = dataList[4].Split(delimiterTwo).ToList();
        enemyBattleModifiers = dataList[5].Split(delimiterTwo).ToList();
        sceneTracker.SetPreviousScene(previousScene);
        enemyList.ResetLists();
        enemyList.AddCharacters(enemies);
        enemyList.SetBattleModifiers(enemyBattleModifiers);
        partyList.SetBattleModifiers(allyBattleModifiers);
        battleMapFeatures.SetTerrainType(terrainType);
    }
}
