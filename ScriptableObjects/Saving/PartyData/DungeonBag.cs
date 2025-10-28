using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonBag", menuName = "ScriptableObjects/DataContainers/SavedData/DungeonBag", order = 1)]
public class DungeonBag : SavedData
{
    public string delimiterTwo;
    public int maxCapacity = 16;
    public void AddCapacity(int amount = 1)
    {
        maxCapacity += amount;
    }
    public bool BagFull()
    {
        return items.Count >= maxCapacity;
    }
    public List<string> items;
    public List<string> GetItems(){return items;}
    public void SetItems(List<string> newInfo)
    {
        utility.RemoveEmptyListItems(newInfo);
        items = newInfo;
    }
    public void GainItem(string newItem)
    {
        if (newItem == ""){return;}
        items.Add(newItem);
    }
    public void UseItem(string usedItem)
    {
        int indexOf = items.IndexOf(usedItem);
        if (indexOf >= 0)
        {
            items.RemoveAt(indexOf);
        }
    }
    public override void NewGame()
    {
        allData = newGameData;
        if (allData.Contains(delimiter)){dataList = allData.Split(delimiter).ToList();}
        else{return;}
        maxCapacity = int.Parse(dataList[0]);
        items = dataList[1].Split(delimiterTwo).ToList();
        Save();
        Load();
    }
    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        allData += maxCapacity + delimiter;
        allData += String.Join(delimiterTwo, items) + delimiter;
        File.WriteAllText(dataPath, allData);
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else{allData = newGameData;}
        if (allData.Contains(delimiter)){dataList = allData.Split(delimiter).ToList();}
        else{return;}
    }
    protected void LoadStat(string stat, int index)
    {
        switch (index)
        {
            case 0:
            maxCapacity = int.Parse(stat);
            break;
            case 1:
            SetItems(stat.Split(delimiterTwo).ToList());
            break;
            default:
            break;
        }
    }
}
