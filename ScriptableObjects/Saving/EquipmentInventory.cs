using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentInventory", menuName = "ScriptableObjects/DataContainers/SavedData/EquipmentInventory", order = 1)]
public class EquipmentInventory : SavedData
{
    public StatDatabase equipData;
    public void AddEquipment(string newEquipment)
    {
        dataList.Add(equipData.ReturnValue(newEquipment));
        SortEquipment();
    }
    public List<string> allWeapons;
    public List<string> allArmor;
    public List<string> allCharms;

    public override void NewGame()
    {
        allData = newGameData;
        dataList = allData.Split(delimiter);
        Save();
    }

    public void SortEquipment()
    {
        allWeapons.Clear();
        allArmor.Clear();
        allCharms.Clear();
        string[] dataBlocks = dataList[0].Split("|");
        for (int i = 0; i < dataList.Count; i++)
        {
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
}
