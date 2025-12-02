using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionUnitDataManager", menuName = "ScriptableObjects/FactionObjects/FactionUnitDataManager", order = 1)]
public class FactionUnitDataManager : SavedData
{
    public string delimiterTwo;
    public string unitSpriteName;
    public string GetUnitSpriteName()
    {
        return unitSpriteName;
    }
    public string combatUnitSpriteName;
    public string GetCombatSpriteName()
    {
        return combatUnitSpriteName;
    }
    public int UnitCount()
    {
        return unitData.Count + combatUnitData.Count;
    }
    public List<string> unitData;
    public List<string> GetUnitData(){return unitData;}
    public int WorkerUnitCount(){return unitData.Count;}
    public void RemoveUnitAtIndex(int index)
    {
        unitData.RemoveAt(index);
        unitLocations.RemoveAt(index);
    }
    public void UpdateUnitAtIndex(FactionUnit fUnit, int index)
    {
        unitData[index] = fUnit.GetStats();
        unitLocations[index] = fUnit.GetLocation().ToString();
    }
    public List<string> unitLocations;
    public int ReturnUnitLocationAtIndex(int index, bool combat = false)
    {
        if (combat)
        {
            return int.Parse(combatUnitLocations[index]);
        }
        return int.Parse(unitLocations[index]);
    }
    public void AddUnit(string unit, int location, bool combat = false)
    {
        if (combat)
        {
            combatUnitData.Add(unit);
            combatUnitLocations.Add(location.ToString());
            return;
        }
        unitData.Add(unit);
        unitLocations.Add(location.ToString());
    }
    public void UpdateUnitLocation(int index, int newLocation, bool combat = false)
    {
        if (combat)
        {
            combatUnitLocations[index] = newLocation.ToString();
            return;
        }
        unitLocations[index] = newLocation.ToString();
    }
    public List<string> combatUnitData;
    public List<string> GetCombatUnitData(){return combatUnitData;}
    public void RemoveCombatUnitAtIndex(int index)
    {
        combatUnitData.RemoveAt(index);
        combatUnitLocations.RemoveAt(index);
    }
    public void UpdateCombatUnitAtIndex(CombatUnit cUnit, int index)
    {
        combatUnitData[index] = cUnit.GetStats();
        combatUnitLocations[index] = cUnit.GetLocation().ToString();
    }
    public List<string> combatUnitLocations;
    public int ReturnCombatUnitLocationAtIndex(int index)
    {
        return int.Parse(combatUnitLocations[index]);
    }
    public string GetUnitDataAtIndex(int index, bool combat = false)
    {
        if (combat)
        {
            return combatUnitData[index];
        }
        return unitData[index];
    }

    public override void NewGame()
    {
        unitData.Clear();
        unitLocations.Clear();
        combatUnitData.Clear();
        combatUnitLocations.Clear();
        Save();
        Load();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        allData += String.Join(delimiterTwo, unitData) + delimiter;
        allData += String.Join(delimiterTwo, unitLocations) + delimiter;
        allData += String.Join(delimiterTwo, combatUnitData) + delimiter;
        allData += String.Join(delimiterTwo, combatUnitLocations) + delimiter;
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
            combatUnitData = stat.Split(delimiterTwo).ToList();
            utility.RemoveEmptyListItems(combatUnitData);
            break;
            case 3:
            combatUnitLocations = stat.Split(delimiterTwo).ToList();
            utility.RemoveEmptyListItems(combatUnitLocations);
            break;
        }
    }
}
