using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : FactionUnit
{
    // Same as a civilian except with combat stats.
    public List<string> actorUnits;
    public List<string> actorStats;
    public List<string> actorEquipment;
    // The unit health tracks how many of each unit appears in battle.
    public List<string> unitMaxHealths;
    public List<string> unitHealths;

    public override string GetStats()
    {
        string stats = "";
        stats += unitType + delimiter;
        stats += faction + delimiter;
        stats += maxHealth + delimiter;
        stats += health + delimiter;
        stats += location + delimiter;
        stats += inventorySize + delimiter;
        stats += String.Join(delimiter2, inventorySize) + delimiter;
        stats += goal + delimiter;
        stats += goalSpecifics + delimiter;
        stats += String.Join(delimiter2, actorUnits) + delimiter;
        stats += String.Join(delimiter2, actorStats) + delimiter;
        stats += String.Join(delimiter2, actorEquipment) + delimiter;
        stats += String.Join(delimiter2, unitMaxHealths) + delimiter;
        stats += String.Join(delimiter2, unitHealths) + delimiter;
        return stats;
    }

    protected override void LoadStat(string stat, int index)
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
            case 9:
            actorUnits = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(actorUnits);
            break;
            case 10:
            actorStats = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(actorStats);
            break;
            case 11:
            actorEquipment = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(actorEquipment);
            break;
            case 12:
            unitMaxHealths = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(unitMaxHealths);
            break;
            case 13:
            unitHealths = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(unitHealths);
            break;
        }
    }
}
