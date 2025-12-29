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
    public void UpdateBattleModifiers()
    {
        allyBattleModifiers = partyList.GetBattleModifiers();
        enemyBattleModifiers = enemyList.GetBattleModifiers();
    }
    public List<string> allyBattleModifiers;
    public List<string> GetAllyBattleModifiers()
    {
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
    public StatDatabase allWeather;
    public string weather;
    public List<string> weatherTypes;
    public void ResetWeather(){SetWeather();}
    public void SetWeather(string newInfo = "")
    {
        weather = newInfo;
    }
    public virtual string GetWeather()
    {
        if (weather.Length > 0)
        {
            return weather;
        }
        if (overworldState == null)
        {
            return weatherTypes[UnityEngine.Random.Range(0, weatherTypes.Count)];
        }
        return overworldState.GetWeather();
    }
    public List<string> allStartingFormations;
    public string spawnPattern;
    public void SetStartingFormation(string newInfo)
    {
        spawnPattern = newInfo;
        int indexOf = allStartingFormations.IndexOf(spawnPattern);
        if (indexOf < 0)
        {
            ResetSpawnPatterns();
        }
        else
        {
            SetAllySpawnPattern(p1StartingFormations[indexOf]);
            SetEnemySpawnPattern(p2StartingFormations[indexOf]);
        }
    }
    public List<string> p1StartingFormations;
    public List<string> p2StartingFormations;
    public string allySpawnPattern;
    public void SetAllySpawnPattern(string newInfo = "Left"){allySpawnPattern = newInfo;}
    public virtual string GetAllySpawnPattern(){return allySpawnPattern;}
    public string enemySpawnPattern;
    public void SetEnemySpawnPattern(string newInfo = "Right"){enemySpawnPattern = newInfo;}
    public virtual string GetEnemySpawnPattern(){return enemySpawnPattern;}
    public void ResetSpawnPatterns()
    {
        SetAllySpawnPattern();
        SetEnemySpawnPattern();
    }
    public string alternateWinCondition;
    public void SetAltWinCon(string newInfo = "")
    {
        alternateWinCondition = newInfo;
    }
    public string alternateWinConditionSpecifics;
    public void SetAltWinConSpecifics(string newInfo = "")
    {
        alternateWinConditionSpecifics = newInfo;
    }
    public void SetNewAlternateWinCondition(string condition = "", string specifics = "")
    {
        alternateWinCondition = condition;
        alternateWinConditionSpecifics = specifics;
    }
    public string GetAltWinCon(){return alternateWinCondition;}
    public string GetAltWinConSpecifics(){return alternateWinConditionSpecifics;}

    public void ResetStats()
    {
        ResetWeather();
        ResetSpawnPatterns();
        SetNewAlternateWinCondition();
    }

    public void SetBattleDetailsFromDungeon(DungeonState dState)
    {
        SetWeather(dState.dungeon.GetWeather());
        ForceTerrainType(dState.dungeon.GenerateTerrain());
        string newInfo = dState.dungeon.GetQuestBattleInfo();
        Debug.Log(newInfo);
        string[] blocks = newInfo.Split(dState.dungeon.bossQuestBattleDelimiter);
        if (blocks.Length <= 3){return;}
        SetStartingFormation(blocks[1]);
        SetAltWinCon(blocks[2]);
        SetAltWinConSpecifics(blocks[3]);
    }

    public override void NewGame()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = newGameData;
        File.WriteAllText(dataPath, allData);
        Load();
        Save();
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
        UpdateBattleModifiers();
        allData += String.Join(delimiterTwo, allyBattleModifiers);
        allData += delimiter;
        allData += String.Join(delimiterTwo, enemyBattleModifiers);
        allData += delimiter;
        allData += weather;
        allData += delimiter;
        allData += allySpawnPattern;
        allData += delimiter;
        allData += enemySpawnPattern;
        allData += delimiter;
        allData += alternateWinCondition;
        allData += delimiter;
        allData += alternateWinConditionSpecifics;
        allData += delimiter;
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
        sceneTracker.SetPreviousScene(previousScene);
        enemyList.ResetLists();
        enemyList.AddCharacters(enemies);
        battleMapFeatures.SetTerrainType(terrainType);
    }

    protected void LoadStat(string stat, int index)
    {
        switch (index)
        {
            case 0:
                previousScene = stat;
                break;
            case 1:
                enemies = stat.Split(delimiterTwo).ToList();
                break;
            case 2:
                terrainType = stat;
                break;
            case 3:
                winningTeam = utility.SafeParseInt(stat, -1);
                break;
            case 4:
                allyBattleModifiers = utility.RemoveEmptyListItems(stat.Split(delimiterTwo).ToList());
                utility.RemoveEmptyListItems(allyBattleModifiers);
                partyList.SetBattleModifiers(allyBattleModifiers);
                break;
            case 5:
                enemyBattleModifiers = utility.RemoveEmptyListItems(stat.Split(delimiterTwo).ToList());
                enemyList.SetBattleModifiers(enemyBattleModifiers);
                break;
            case 6:
                SetWeather(stat);
                break;
            case 7:
                SetAllySpawnPattern(stat);
                break;
            case 8:
                SetEnemySpawnPattern(stat);
                break;
            case 9:
                SetAltWinCon(stat);
                break;
            case 10:
                SetAltWinConSpecifics(stat);
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                break;
            case 14:
                break;
            case 15:
                break;
            case 16:
                break;
            case 17:
                break;
            case 18:
                break;
            case 19:
                break;
            case 20:
                break;
            default:
                break;
        }
    }
}
