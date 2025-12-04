using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionEnemyUnitData", menuName = "ScriptableObjects/FactionObjects/FactionEnemyUnitData", order = 1)]
public class FactionEnemyUnitData : SavedData
{
    public string delimiterTwo;
    // Fixed per enemy faction.
    public string enemyFactionName;
    public StatDatabase actorData;
    public List<string> spawnedUnits;
    public List<int> unitWeights;
    public StatDatabase equipmentData;
    public List<string> spawnedWeapons;
    public List<int> weaponWeights;
    public List<string> spawnedArmors;
    public List<int> armorWeights;
    public string spawnerSpriteName;
    public string GetSpawnerSpriteName()
    {
        return spawnerSpriteName;
    }
    public string unitSpriteName;
    public string GetUnitSpriteName()
    {
        return unitSpriteName;
    }
    // Variables depending on game state.
    public List<int> spawnPoints;
    public List<int> spawnPointLevels;
    public List<int> spawnPointExps;
    public void SpawnUnits(CombatUnit cUnit)
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            cUnit.ResetStats();
            // Generate the unit.
            // Random unit, level based on spawn point level.
            string unit = utility.RandomStringBasedOnWeight(spawnedUnits, unitWeights);
            string stats = actorData.ReturnValue(unit);
            // Random equipment.
            string equipment = equipmentData.ReturnValue(utility.RandomStringBasedOnWeight(spawnedWeapons, weaponWeights)) + "@" + equipmentData.ReturnValue(utility.RandomStringBasedOnWeight(spawnedArmors, armorWeights));
            cUnit.AddNewUnit(unit, stats, equipment, spawnPointLevels[i]);
            // Update the faction.
            cUnit.SetFaction(enemyFactionName);
            // Update the location.
            cUnit.SetLocation(spawnPoints[i]);
            // No need for a goal: rob, destroy and kill is all monsters need.
            AddUnit(cUnit.GetStats(), spawnPoints[i]);
        }
    }
    // Units will deposit their stolen goods to increase exp of their spawn point.
    public void CollectTreasure(CombatUnit cUnit)
    {

    }
    public int UnitCount()
    {
        return unitData.Count;
    }
    public List<string> unitData;
    public List<string> GetUnitData(){return unitData;}
    public void RemoveUnitAtIndex(int index)
    {
        unitData.RemoveAt(index);
        unitLocations.RemoveAt(index);
    }
    public void UpdateUnitAtIndex(CombatUnit cUnit, int index)
    {
        unitData[index] = cUnit.GetStats();
        unitLocations[index] = cUnit.GetLocation().ToString();
    }
    public List<string> unitLocations;
    public int ReturnUnitLocationAtIndex(int index)
    {
        return int.Parse(unitLocations[index]);
    }
    public void AddUnit(string unit, int location)
    {
        unitData.Add(unit);
        unitLocations.Add(location.ToString());
    }
    public void UpdateUnitLocation(int index, int newLocation)
    {
        unitLocations[index] = newLocation.ToString();
    }
    
    // Saving/Loading Stuff.
    public override void NewGame()
    {
        unitData.Clear();
        unitLocations.Clear();
        spawnPoints.Clear();
        spawnPointLevels.Clear();
        spawnPointExps.Clear();
        Save();
        Load();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        allData += String.Join(delimiterTwo, unitData) + delimiter;
        allData += String.Join(delimiterTwo, unitLocations) + delimiter;
        allData += String.Join(delimiterTwo, spawnPoints) + delimiter;
        allData += String.Join(delimiterTwo, spawnPointLevels) + delimiter;
        allData += String.Join(delimiterTwo, spawnPointExps) + delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else
        {
            NewGame();
            Save();
            return;
        }
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
            default:
            break;
            case 0:
            unitData = stat.Split(delimiterTwo).ToList();
            utility.RemoveEmptyListItems(unitData);
            break;
            case 1:
            unitLocations = stat.Split(delimiterTwo).ToList();
            utility.RemoveEmptyListItems(unitLocations);
            break;
            case 2:
            spawnPoints = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
            utility.RemoveEmptyValues(spawnPoints);
            break;
            case 3:
            spawnPointLevels = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
            utility.RemoveEmptyValues(spawnPointLevels);
            break;
            case 4:
            spawnPointExps = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
            utility.RemoveEmptyValues(spawnPointExps);
            break;
        }
    }
}
