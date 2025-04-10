using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores items, equipment, cargo, etc.
[CreateAssetMenu(fileName = "GuildStorage", menuName = "ScriptableObjects/DataContainers/SavedData/GuildStorage", order = 1)]
public class GuildStorage : SavedData
{
    public string delimiterTwo;
    public int maxStorage;
    public List<string> storedItems;
    public List<string> storedQuantities;

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
        allData = maxStorage+delimiter;
        for (int i = 0; i < storedItems.Count; i++)
        {
            allData += storedItems[i];
            if (i < storedItems.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < storedQuantities.Count; i++)
        {
            allData += storedQuantities[i];
            if (i < storedQuantities.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else
        {
            NewGame();
            return;
        }
        dataList = allData.Split(delimiter).ToList();
        maxStorage = int.Parse(dataList[0]);
        storedItems = dataList[1].Split(delimiterTwo).ToList();
        storedQuantities = dataList[2].Split(delimiterTwo).ToList();
    }
}
