using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : FactionUnit
{
    public TacticActor dummyActor;
    // Same as a civilian except with combat stats.
    public List<string> actorUnits;
    public List<string> actorStats;
    public List<string> actorEquipment;
    public void AddNewUnit(string unit, string stats, string equipment, int maxHealth)
    {
        actorUnits.Add(unit);
        actorStats.Add(stats);
        actorEquipment.Add(equipment);
        unitMaxHealths.Add(maxHealth);
        unitHealths.Add(maxHealth);
    }
    // The unit health tracks how many of each unit appears in battle.
    public List<int> unitMaxHealths;
    public List<int> unitHealths;
    public void RemoveActorAtIndex(int index)
    {
        actorUnits.RemoveAt(index);
        actorStats.RemoveAt(index);
        actorEquipment.RemoveAt(index);
        unitMaxHealths.RemoveAt(index);
        unitHealths.RemoveAt(index);
    }
    public override void GainMaxHealth(int amount)
    {
        for (int i = 0; i < unitMaxHealths.Count; i++)
        {
            unitMaxHealths[i] += amount;
        }
    }
    public override void Heal(int amount)
    {
        for (int i = 0; i < unitHealths.Count; i++)
        {
            unitHealths[i] = Mathf.Min(unitHealths[i] + amount, unitMaxHealths[i]);
        }
    }
    public override void TakeDamage(int amount = 1)
    {
        for (int i = 0; i < unitHealths.Count; i++)
        {
            unitHealths[i] -= amount;
        }
        RemoveDeadUnits();
    }
    public override void Rest()
    {
        for (int i = 0; i < unitHealths.Count; i++)
        {
            unitHealths[i] = Mathf.Min(unitHealths[i] + 1, unitMaxHealths[i]);
        }
    }
    public void RemoveDeadUnits()
    {
        for (int i = actorStats.Count - 1; i >= 0; i--)
        {
            if (unitHealths[i] <= 0)
            {
                RemoveActorAtIndex(i);
            }
        }
    }
    public override bool Dead()
    {
        RemoveDeadUnits();
        if (actorStats.Count <= 0){return true;}
        for (int i = 0; i < unitHealths.Count; i++)
        {
            if (unitHealths[i] > 0){return false;}
        }
        return true;
    }
    public override void GainExp(int amount = 1)
    {
        exp += amount;
        if (exp > level * level * actorUnits.Count)
        {
            LevelUp();
        }
    }
    public override void LevelUp()
    {
        level++;
        exp = 0;
        GainMaxHealth(1);
        Heal(1);
        // We can also gain a random combat stat (hp/atk/def).
        AdjustMaxLoyalty(1);
        AdjustInventorySize(1);
    }

    public override void ResetStats()
    {
        base.ResetStats();
        unitType = "Soldier";
        actorUnits.Clear();
        actorStats.Clear();
        actorEquipment.Clear();
        unitMaxHealths.Clear();
        unitHealths.Clear();
    }

    public override string GetStats()
    {
        string stats = GetBasicStats();
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
                level = int.Parse(stat);
                break;
            case 3:
                exp = int.Parse(stat);
                break;
            case 4:
                maxHealth = int.Parse(stat);
                break;
            case 5:
                health = int.Parse(stat);
                break;
            case 6:
                maxLoyalty = int.Parse(stat);
                break;
            case 7:
                loyalty = int.Parse(stat);
                break;
            case 8:
                maxEnergy = int.Parse(stat);
                break;
            case 9:
                energy = int.Parse(stat);
                break;
            case 10:
                baseSpeed = int.Parse(stat);
                break;
            case 11:
                movement = int.Parse(stat);
                break;
            case 12:
                location = int.Parse(stat);
                break;
            case 13:
                inventorySize = int.Parse(stat);
                break;
            case 14:
                inventory = stat.Split(delimiter2).ToList();
                utility.RemoveEmptyListItems(inventory);
                break;
            case 15:
                goal = stat;
                break;
            case 16:
                goalSpecifics = stat;
                break;
            case 17:
            actorUnits = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(actorUnits);
            break;
            case 18:
            actorStats = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(actorStats);
            break;
            case 19:
            actorEquipment = stat.Split(delimiter2).ToList();
            utility.RemoveEmptyListItems(actorEquipment);
            break;
            case 20:
            unitMaxHealths = utility.ConvertStringListToIntList(stat.Split(delimiter2).ToList());
            break;
            case 21:
            unitHealths = utility.ConvertStringListToIntList(stat.Split(delimiter2).ToList());
            break;
        }
    }
}
