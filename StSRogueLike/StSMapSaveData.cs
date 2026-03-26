using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSMapSaveData", menuName = "ScriptableObjects/StS/StSMapSaveData", order = 1)]
public class StSMapSaveData : SavedData
{
    public string delimiter2 = ",";
    public override void Save()
    {
        // Don't bother saving if blank?
        if (allData.Length < 1){return;}
        dataPath = Application.persistentDataPath + "/" + filename;
        File.WriteAllText(dataPath, allData);
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = File.ReadAllText(dataPath);
    }
    public void SaveMap(StSMap mapToSave)
    {
        allData = "";
        allData += "Map=" + String.Join(delimiter2, mapToSave.mapInfo) + delimiter;
        allData += "Path=" + String.Join(delimiter2, mapToSave.pathTaken) + delimiter;
        allData += "Ancient=" + mapToSave.floorAncient + delimiter;
        allData += "Boss=" + mapToSave.floorBoss + delimiter;
        Save();
    }
    public string LoadMap()
    {
        Load();
        return allData;
    }
}
