using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionUnit : MonoBehaviour
{
    public GeneralUtility utility;
    public string delimiter = "#";
    public string delimiter2 = "|";
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
    public virtual void GainMaxHealth(int amount)
    {
        maxHealth += amount;
    }
    public int health;
    public virtual void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    public virtual void TakeDamage(int amount)
    {
        health -= amount;
    }
    public virtual bool Hurt()
    {
        return health < maxHealth;
    }
    public virtual bool Dead()
    {
        return health <= 0;
    }
    public virtual void Rest()
    {
        health++;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        Relax();
    }
    public int maxEnergy = 6;
    public int energy = 6;
    public bool EnergyAvailable()
    {
        return energy > 0;
    }
    public bool UseEnergy()
    {
        if (EnergyAvailable())
        {
            energy--;
            return true;
        }
        return false;
    }
    public virtual void Relax()
    {
        energy = maxEnergy;
    }
    public int maxLoyalty = 6;
    public void AdjustMaxLoyalty(int amount)
    {
        maxLoyalty += amount;
    }
    public int loyalty;
    public void AdjustLoyalty(int amount)
    {
        loyalty += amount;
        if (loyalty > maxLoyalty)
        {
            loyalty = maxLoyalty;
        }
    }
    public int GetLoyalty(){return loyalty;}
    public int level;
    public int exp;
    public virtual void GainExp(int amount)
    {
        exp += amount;
        if (exp > level * level * level)
        {
            LevelUp();
        }
    }
    public virtual void LevelUp()
    {
        level++;
        exp = 0;
        GainMaxHealth(1);
        Heal(1);
        AdjustMaxLoyalty(1);
        AdjustInventorySize(1);
        if (level % 3 == 0)
        {
            maxEnergy++;
        }
        if (level % 4 == 0)
        {
            baseSpeed++;
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
    public int baseSpeed = 2;
    public int movement = 2;
    public void MoveFaster()
    {
        if (UseEnergy())
        {
            movement += baseSpeed;
        }
    }
    public int GetMovement()
    {
        return movement;
    }
    public void UseMovement(int moveCost)
    {
        movement -= moveCost;
    }
    public int inventorySize;
    public virtual void AdjustInventorySize(int amount)
    {
        inventorySize += amount;
    }
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
        if (goalSpecifics == ""){return "Gold";}
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

    public virtual bool CompletedGoal()
    {
        return inventory.Contains(goalSpecifics);
    }

    public void ResetStats()
    {
        unitType = "Worker";
        faction = "";
        maxHealth = 1;
        health = 1;
        maxEnergy = 3;
        energy = 3;
        maxLoyalty = 6;
        loyalty = 3;
        location = -1;
        baseSpeed = 2;
        movement = 2;
        inventorySize = 6;
        inventory.Clear();
        goal = "Gather";
        goalSpecifics = "";
        level = 1;
        exp = 0;
    }

    protected virtual string GetBasicStats()
    {
        string stats = "";
        stats += unitType + delimiter;
        stats += faction + delimiter;
        stats += level + delimiter;
        stats += exp + delimiter;
        stats += maxHealth + delimiter;
        stats += health + delimiter;
        stats += maxLoyalty + delimiter;
        stats += loyalty + delimiter;
        stats += maxEnergy + delimiter;
        stats += energy + delimiter;
        stats += baseSpeed + delimiter;
        stats += movement + delimiter;
        stats += location + delimiter;
        stats += inventorySize + delimiter;
        stats += String.Join(delimiter2, inventory) + delimiter;
        stats += goal + delimiter;
        stats += goalSpecifics + delimiter;
        return stats;
    }

    public virtual string GetStats()
    {
        return GetBasicStats();
    }

    protected void NewTurn()
    {
        movement = baseSpeed;
    }

    public virtual void LoadStats(string newInfo)
    {
        string[] stats = newInfo.Split(delimiter);
        for (int i = 0; i < stats.Length; i++)
        {
            LoadStat(stats[i], i);
        }
        // Whenever loading it's a new turn.
        NewTurn();
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
        }
    }
}
