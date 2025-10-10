using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentInventory", menuName = "ScriptableObjects/DataContainers/SavedData/EquipmentInventory", order = 1)]
public class EquipmentInventory : SavedData
{
    public override void NewGame()
    {
        base.NewGame();
        Load();
    }
    public StatDatabase equipData;
    public List<string> GetAllEquipment()
    {
        utility.RemoveEmptyListItems(dataList);
        return new List<string>(dataList);
    }
    public int GetEquipmentCount()
    {
        return dataList.Count;
    }
    public void AddEquipmentByName(string newEquipment)
    {
        dataList.Add(equipData.ReturnValue(newEquipment));
        SortEquipment();
    }
    public void AddEquipmentByStats(string newStats)
    {
        if (newStats.Length < 6){return;}
        dataList.Add(newStats);
        SortEquipment();
    }
    public void RemoveEquipment(int index)
    {
        dataList.RemoveAt(index);
        SortEquipment();
    }
    public string TakeEquipment(int index, int slot, bool remove = true)
    {
        switch (slot)
        {
            case 0:
            return TakeWeapon(index, remove);
            case 1:
            return TakeArmor(index, remove);
            case 2:
            return TakeCharm(index, remove);
        }
        return "";
    }
    public string TakeWeapon(int otherIndex, bool remove = true)
    {
        string data = allWeapons[otherIndex];
        int indexOf = dataList.IndexOf(allWeapons[otherIndex]);
        if (remove)
        {
            allWeapons.RemoveAt(otherIndex);
            RemoveEquipment(indexOf);
        }
        return data;
    }
    public string TakeArmor(int otherIndex, bool remove = true)
    {
        string data = allArmor[otherIndex];
        int indexOf = dataList.IndexOf(allArmor[otherIndex]);
        if (remove)
        {
            allArmor.RemoveAt(otherIndex);
            RemoveEquipment(indexOf);
        }
        return data;
    }
    public string TakeCharm(int otherIndex, bool remove = true)
    {
        string data = allCharms[otherIndex];
        int indexOf = dataList.IndexOf(allCharms[otherIndex]);
        if (remove)
        {
            allCharms.RemoveAt(otherIndex);
            RemoveEquipment(indexOf);
        }
        return data;
    }
    public List<string> allWeapons;
    public int WeaponCount(){return allWeapons.Count;}
    public List<string> GetWeapons(){return allWeapons;}
    public List<string> allArmor;
    public int ArmorCount(){return allArmor.Count;}
    public List<string> GetArmor(){return allArmor;}
    public List<string> allCharms;
    public int CharmCount(){return allCharms.Count;}
    public List<string> GetCharms(){return allCharms;}

    public override void Load()
    {
        base.Load();
        SortEquipment();
    }

    public void SortEquipment()
    {
        utility.RemoveEmptyListItems(dataList);
        allWeapons.Clear();
        allArmor.Clear();
        allCharms.Clear();
        if (dataList.Count <= 0){return;}
        string[] dataBlocks = dataList[0].Split("|");
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].Length < 6){continue;}
            dataBlocks = dataList[i].Split("|");
            switch (dataBlocks[1])
            {
                case "Weapon":
                allWeapons.Add(dataList[i]);
                break;
                case "Armor":
                allArmor.Add(dataList[i]);
                break;
                case "Charm":
                allCharms.Add(dataList[i]);
                break;
            }
        }
    }

    public int ReturnEquipmentQuantity(string equipmentName)
    {
        int quantity = 0;
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].StartsWith(equipmentName)) { quantity++; }
        }
        return quantity;
    }
}
