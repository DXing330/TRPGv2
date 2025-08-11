using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSState", menuName = "ScriptableObjects/DataContainers/SavedData/StSState", order = 1)]
public class StSState : SavedState
{
    // Save the map, the floor, the party location, the store items, the random event details.
    public override void NewGame()
    {
        newGame = 1;
        return;
    }
    public int newGame = 1;
    public bool StartingNewGame()
    {
        return newGame == 1;
    }
    public List<string> mapInfo;
    public List<int> partyPathing;
    public List<string> storeInfo;
    public List<int> storePrices;
    public int eventIndex;
    public void SetDataFromMap(StSLikeMap map)
    {
        newGame = 0;
        mapInfo = new List<string>(map.mapInfo);
        partyPathing = new List<int>(map.partyPathing);
        Save();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = newGame + delimiter + String.Join(delimiterTwo, mapInfo) + delimiter + String.Join(delimiterTwo, partyPathing);
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
            return;
        }
        partyPathing = utility.ConvertStringListToIntList(dataList[2].Split(delimiterTwo).ToList());
    }
}
