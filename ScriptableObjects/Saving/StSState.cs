using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSState", menuName = "ScriptableObjects/DataContainers/SavedData/StSState", order = 1)]
public class StSState : SavedState
{
    // Save the map, the floor, the party location.

    public override void NewGame()
    {
        return;
    }
    public List<string> mapInfo;
    public List<int> partyPathing;
    public void SetDataFromMap(StSLikeMap map)
    {
        mapInfo = new List<string>(map.mapInfo);
        partyPathing = new List<int>(map.partyPathing);
        Save();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = String.Join(delimiterTwo, mapInfo) + delimiter + String.Join(delimiterTwo, partyPathing);
        File.WriteAllText(dataPath, allData);
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        mapInfo = dataList[0].Split(delimiterTwo).ToList();
        partyPathing = utility.ConvertStringListToIntList(dataList[1].Split(delimiterTwo).ToList());
    }
}
