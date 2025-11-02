using System;
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
    public int maxDungeonStorage;
    public string ReturnDungeonStorageLimitString()
    {
        return storedDungeonItems.Count + "/" + maxDungeonStorage;
    }
    public bool MaxedDungeonStorage()
    {
        return storedDungeonItems.Count >= maxDungeonStorage;
    }
    public bool DungeonStorageAvailable(int count)
    {
        return storedDungeonItems.Count + count <= maxDungeonStorage;
    }
    public List<string> storedDungeonItems;
    public List<string> GetStoredDungeonItems(){return storedDungeonItems;}
    public void StoreDungeonItem(string newInfo)
    {
        storedDungeonItems.Add(newInfo);
        storedDungeonItems.Sort();
    }
    public void StoreDungeonItems(List<string> newInfo)
    {
        storedDungeonItems.AddRange(newInfo);
        storedDungeonItems.Sort();
    }
    public void WithdrawDungeonItem(string newInfo)
    {
        int indexOf = storedDungeonItems.IndexOf(newInfo);
        if (indexOf < 0){return;}
        storedDungeonItems.RemoveAt(indexOf);
        storedDungeonItems.Sort();
    }
    public int maxStorage;
    public bool MaxedStorage()
    {
        int quantity = 0;
        for (int i = 0; i < storedQuantities.Count; i++)
        {
            quantity += utility.SafeParseInt(storedQuantities[i], 0);
        }
        return quantity >= maxStorage;
    }
    public List<string> storedItems;
    public List<string> storedQuantities;
    public int storedGold;
    public int GetStoredGold(){return storedGold;}
    public void StoreGold(int amount)
    {
        storedGold += amount;
    }
    public void WithdrawGold(int amount)
    {
        storedGold -= amount;
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
        allData = "";
        allData += maxDungeonStorage + delimiter;
        allData += String.Join(delimiterTwo, storedDungeonItems) + delimiter;
        allData += maxStorage + delimiter;
        allData += String.Join(delimiterTwo, storedItems) + delimiter;
        allData += String.Join(delimiterTwo, storedQuantities) + delimiter;
        allData += storedGold + delimiter;
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
            maxDungeonStorage = utility.SafeParseInt(stat, 1);
            break;
            case 1:
            storedDungeonItems = utility.RemoveEmptyListItems(stat.Split(delimiterTwo).ToList());
            break;
            case 2:
            maxStorage = utility.SafeParseInt(stat, 1);
            break;
            case 3:
            storedItems = utility.RemoveEmptyListItems(stat.Split(delimiterTwo).ToList());
            break;
            case 4:
            storedQuantities = utility.RemoveEmptyListItems(stat.Split(delimiterTwo).ToList());
            break;
            case 5:
            storedGold = utility.SafeParseInt(stat);
            break;
        }
    }
}
