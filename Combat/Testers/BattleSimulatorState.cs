using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleSimulatorState", menuName = "ScriptableObjects/DataContainers/SavedData/BattleSimulatorState", order = 1)]
public class BattleSimulatorState : BattleState
{
    public string delimiterThree;
    public CharacterList partyOneList;
    public CharacterList partyTwoList;
    public List<string> terrainTypes;
    public override void SetTerrainType()
    {
        battleMapFeatures.SetTerrainType(GetTerrainType());
    }
    public override string GetTerrainType()
    {
        return terrainTypes[UnityEngine.Random.Range(0, terrainTypes.Count)];
    }
    public List<string> weatherTypes;
    public override string GetWeather()
    {
        return weatherTypes[UnityEngine.Random.Range(0, weatherTypes.Count)];
    }
    public List<string> timeOfDayTypes;
    public override string GetTime()
    {
        return timeOfDayTypes[UnityEngine.Random.Range(0, timeOfDayTypes.Count)];
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
    }
}
