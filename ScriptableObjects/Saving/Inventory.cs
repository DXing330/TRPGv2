using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/DataContainers/SavedData/Inventory", order = 1)]
public class Inventory : SavedData
{
    public GeneralUtility utility;
    public string delimiterTwo;
    public List<string> items;
    public List<string> quantities;
    public string goldString = "Gold";

    public override void NewGame()
    {
        allData = newGameData;
        if (allData.Contains(delimiter)){dataList = allData.Split(delimiter).ToList();}
        else{return;}
        items = dataList[0].Split(delimiterTwo).ToList();
        quantities = dataList[1].Split(delimiterTwo).ToList();
        items = utility.RemoveEmptyListItems(items);
        quantities = utility.RemoveEmptyListItems(quantities);
        Save();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        string tempData = "";
        for (int i = 0; i < items.Count; i++)
        {
            tempData += items[i];
            if (i < items.Count - 1){tempData += delimiterTwo;}
        }
        tempData += delimiter;
        for (int i = 0; i < quantities.Count; i++)
        {
            tempData += quantities[i];
            if (i < quantities.Count - 1){tempData += delimiterTwo;}
        }
        tempData += delimiter;
        allData = tempData;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else{allData = newGameData;}
        if (allData.Contains(delimiter)){dataList = allData.Split(delimiter).ToList();}
        else{return;}
        items = dataList[0].Split(delimiterTwo).ToList();
        quantities = dataList[1].Split(delimiterTwo).ToList();
        items = utility.RemoveEmptyListItems(items);
        quantities = utility.RemoveEmptyListItems(quantities);
    }

    public int ReturnGold()
    {
        return ReturnQuantityOfItem(goldString);
    }

    public int ReturnQuantityOfItem(string itemName)
    {
        int indexOf = items.IndexOf(itemName);
        if (indexOf < 0){return 0;}
        return int.Parse(quantities[indexOf]);
    }

    public bool ItemExists(string itemName)
    {
        return ReturnQuantityOfItem(itemName) > 0;
    }

    public bool QuantityExists(int quantity, string itemName = "Gold")
    {
        return ReturnQuantityOfItem(itemName) >= quantity;
    }

    // Should only be called after confirming that the quantity exists.
    public void RemoveItemQuantity(int quantity, string itemName = "Gold")
    {
        int indexOf = items.IndexOf(itemName);
        int currentQuantity = int.Parse(quantities[indexOf]);
        quantities[indexOf] = (currentQuantity - quantity).ToString();
    }

    public void AddItemQuantity(string itemName, int quantity = 1)
    {
        int indexOf = items.IndexOf(itemName);
        if (indexOf < 0)
        {
            items.Add(itemName);
            quantities.Add(quantity.ToString());
        }
        else
        {
            int currentQuantity = int.Parse(quantities[indexOf]);
            quantities[indexOf] = (currentQuantity + quantity).ToString();
        }
    }
}
