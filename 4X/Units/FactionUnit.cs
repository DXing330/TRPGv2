using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionUnit : MonoBehaviour
{
    public GeneralUtility utility;
    protected string delimiter = "#";
    protected string delimiter2 = "|";
    public string unitType; // Civilian/Combat
    public string faction;
    public void SetFaction(string factionName)
    {
        faction = factionName;
    }
    public string GetFaction()
    {
        return faction;
    }
    public int maxHealth;
    public int health;
    public bool Dead()
    {
        return health <= 0;
    }
    public void Rest()
    {
        health++;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    public int location;
    public int GetLocation()
    {
        return location;
    }
    public void SetLocation(int newInfo)
    {
        location = newInfo;
    }
    public int inventorySize;
    public bool InventoryFull()
    {
        return inventory.Count >= inventorySize;
    }
    public List<string> inventory; // If inventory is full then return to city and drop off inventory.
    public void GainItems(List<string> newItems)
    {
        for (int i = 0; i < newItems.Count; i++)
        {
            if (InventoryFull()){return;}
            if (newItems[i].Length < 2){continue;}
            inventory.Add(newItems[i]);
        }
    }
    public string goal; // If inventory is not full try to acquire the goal items, goal is updated at the city.
    public string goalSpecifics;
    public string GetGoal()
    {
        return goal;
    }
    public string GetGoalSpecifics()
    {
        return goalSpecifics;
    }
    public void SetGoal(string nG, string nGS = "")
    {
        goal = nG;
        goalSpecifics = nGS;
    }
    public void SetGoalSpecifics(string nGS)
    {
        goalSpecifics = nGS;
    }

    public void ResetStats()
    {
        unitType = "Worker";
        faction = "";
        maxHealth = 1;
        health = 1;
        location = -1;
        inventorySize = 6;
        inventory.Clear();
        goal = "Gather";
        goalSpecifics = "";
    }

    public virtual string GetStats()
    {
        string stats = "";
        stats += unitType + delimiter;
        stats += faction + delimiter;
        stats += maxHealth + delimiter;
        stats += health + delimiter;
        stats += location + delimiter;
        stats += inventorySize + delimiter;
        stats += String.Join(delimiter2, inventory) + delimiter;
        stats += goal + delimiter;
        stats += goalSpecifics + delimiter;
        return stats;
    }

    public virtual void LoadStats(string newInfo)
    {
        string[] stats = newInfo.Split(delimiter);
        for (int i = 0; i < stats.Length; i++)
        {
            LoadStat(stats[i], i);
        }
    }

    protected virtual void LoadStat(string stat, int index)
    {
        switch (index)
        {
            default:
            break;
            case 0:
            unitType = stat;
            break;
            case 1:
            faction = stat;
            break;
            case 2:
            maxHealth = int.Parse(stat);
            break;
            case 3:
            health = int.Parse(stat);
            break;
            case 4:
            location = int.Parse(stat);
            break;
            case 5:
            inventorySize = int.Parse(stat);
            break;
            case 6:
            inventory = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(inventory);
            break;
            case 7:
            goal = stat;
            break;
            case 8:
            goalSpecifics = stat;
            break;
        }
    }
}
