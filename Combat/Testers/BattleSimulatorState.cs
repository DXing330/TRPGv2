using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleSimulatorState", menuName = "ScriptableObjects/Debug/BattleSimulatorState", order = 1)]
public class BattleSimulatorState : BattleState
{
    public string delimiterThree;
    public CharacterList partyOneList;
    public CharacterList partyTwoList;
    // Select which terrains that battle may take place one, then randomly select one of the selected.
    public List<string> allTerrainTypes;
    public List<string> selectedTerrainTypes;
    public void SelectTerrainType(int index)
    {
        string newInfo = allTerrainTypes[index];
        if (selectedTerrainTypes.Contains(newInfo))
        {
            selectedTerrainTypes.Remove(newInfo);
        }
        else
        {
            selectedTerrainTypes.Add(newInfo);
        }
    }
    public string selectedTerrain;
    public override void SetTerrainType()
    {
        battleMapFeatures.SetTerrainType(GetTerrainType());
    }
    public override string GetTerrainType()
    {
        if (selectedTerrainTypes.Count > 0)
        {
            selectedTerrain = selectedTerrainTypes[UnityEngine.Random.Range(0, selectedTerrainTypes.Count)];
        }
        else
        {
            selectedTerrain = "Random";
        }
        if (selectedTerrain == "Random")
        {
            return terrainTypes[UnityEngine.Random.Range(0, terrainTypes.Count)];
        }
        return selectedTerrain;
    }
    public List<string> allWeathers;
    public List<string> selectedWeathers;
    public void SelectWeather(int index)
    {
        string newInfo = allWeathers[index];
        if (selectedWeathers.Contains(newInfo))
        {
            selectedWeathers.Remove(newInfo);
        }
        else
        {
            selectedWeathers.Add(newInfo);
        }
    }
    public string selectedWeather;
    public override string GetWeather()
    {
        if (selectedWeathers.Count > 0)
        {
            selectedWeather = selectedWeathers[UnityEngine.Random.Range(0, selectedWeathers.Count)];
        }
        else
        {
            selectedWeather = "Random";
        }
        if (selectedWeather == "Random")
        {
            return weatherTypes[UnityEngine.Random.Range(0, weatherTypes.Count)];
        }
        return selectedWeather;
    }
    public List<string> allTimes;
    public List<string> selectedTimes;
    public void SelectTime(int index)
    {
        string newInfo = allTimes[index];
        if (selectedTimes.Contains(newInfo))
        {
            selectedTimes.Remove(newInfo);
        }
        else
        {
            selectedTimes.Add(newInfo);
        }
    }
    public string selectedTime;
    public override string GetTime()
    {
        if (selectedTimes.Count > 0)
        {
            selectedTime = selectedTimes[UnityEngine.Random.Range(0, selectedTimes.Count)];
        }
        else
        {
            selectedTime = "Random";
        }
        if (selectedTime == "Random")
        {
            return allTimes[UnityEngine.Random.Range(0, allTimes.Count)];
        }
        return selectedTime;
    }
    public int multiBattle = 0;
    public int multiBattleCount = 2;
    public void ChangeMultiBattleCount(bool right = true)
    {
        multiBattleCurrent = 0;
        multiBattleCount = utility.ChangeIndex(multiBattleCount, right, maxMultiBattle, minMultiBattle);
    }
    public int minMultiBattle = 2;
    public int maxMultiBattle = 30;
    public int multiBattleCurrent = 0;
    public void ResetBattleIteration()
    {
        multiBattle = 0;
        multiBattleCurrent = 0;
        Save();
    }
    public int GetCurrentMultiBattleIteration()
    {
        return multiBattleCurrent;
    }
    public void IncrementMultiBattle()
    {
        multiBattleCurrent++;
        Save();
    }
    public void EnableMultiBattle()
    {
        multiBattleCurrent = 0;
        multiBattle = (multiBattle + 1) % 2;
    }
    public bool MultiBattleEnabled()
    {
        return multiBattle == 1;
    }
    public bool MultiBattleFinished()
    {
        return (multiBattleCurrent >= multiBattleCount);
    }
    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = "";
        allData += String.Join(delimiterThree, partyOneList.GetCharacterNames()) + delimiterTwo;
        allData += String.Join(delimiterThree, partyOneList.GetCharacterSprites()) + delimiterTwo;
        allData += String.Join(delimiterThree, partyOneList.GetCharacterStats()) + delimiterTwo;
        allData += String.Join(delimiterThree, partyOneList.GetCharacterEquipment());
        allData += delimiter;
        allData += String.Join(delimiterThree, partyTwoList.GetCharacterNames()) + delimiterTwo;
        allData += String.Join(delimiterThree, partyTwoList.GetCharacterSprites()) + delimiterTwo;
        allData += String.Join(delimiterThree, partyTwoList.GetCharacterStats()) + delimiterTwo;
        allData += String.Join(delimiterThree, partyTwoList.GetCharacterEquipment());
        allData += delimiter;
        allData += string.Join(delimiterThree, selectedTerrainTypes) + delimiter;
        allData += string.Join(delimiterThree, selectedWeathers) + delimiter;
        allData += string.Join(delimiterThree, selectedTimes) + delimiter;
        allData += multiBattle + delimiter + multiBattleCount + delimiter + multiBattleCurrent;
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
            return;
        }
        dataList = allData.Split(delimiter).ToList();
        List<string> dataBlocks = dataList[0].Split(delimiterTwo).ToList();
        if (dataBlocks[1].Length < 1){ return; }
        partyOneList.SetLists(dataBlocks[1].Split(delimiterThree).ToList(), dataBlocks[2].Split(delimiterThree).ToList(), dataBlocks[0].Split(delimiterThree).ToList(), dataBlocks[3].Split(delimiterThree).ToList());
        dataBlocks = dataList[1].Split(delimiterTwo).ToList();
        partyTwoList.SetLists(dataBlocks[1].Split(delimiterThree).ToList(), dataBlocks[2].Split(delimiterThree).ToList(), dataBlocks[0].Split(delimiterThree).ToList(), dataBlocks[3].Split(delimiterThree).ToList());
        selectedTerrainTypes = dataList[2].Split(delimiterThree).ToList();
        selectedWeathers = dataList[3].Split(delimiterThree).ToList();
        selectedTimes = dataList[4].Split(delimiterThree).ToList();
        multiBattle = int.Parse(dataList[5]);
        multiBattleCount = int.Parse(dataList[6]);
        multiBattleCurrent = int.Parse(dataList[7]);
    }
}
