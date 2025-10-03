using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string allStats;
    public string GetStats(){ return allStats; }
    public void RefreshStats()
    {
        allStats = "";
        allStats += equipName + "|";
        allStats += slot + "|";
        allStats += type + "|";
        allStats += String.Join(",", passives) + "|";
        allStats += String.Join(",", passiveLevels) + "|";
        allStats += maxLevel + "|";
        allStats += rarity + "|";
    }
    public void SetAllStats(string newStats)
    {
        allStats = newStats;
        string[] dataBlocks = allStats.Split("|");
        if (allStats.Length < 6 || dataBlocks.Length < 7)
        {
            equipName = "None";
            slot = "-1";
            type = "-1";
            return;
        }
        equipName = dataBlocks[0];
        slot = dataBlocks[1];
        type = dataBlocks[2];
        passives = dataBlocks[3].Split(",").ToList();
        passiveLevels = dataBlocks[4].Split(",").ToList();
        maxLevel = dataBlocks[5];
        rarity = dataBlocks[6];
    }
    public string equipName;
    public string GetName(){return equipName;}
    public string slot;
    public string GetSlot(){return slot;}
    public string type;
    public string GetEquipType(){return type;}
    public List<string> passives;
    public List<string> GetPassives()
    {
        return passives;
    }
    public void DebugPassives()
    {
        for (int i = 0; i < passives.Count; i++)
        {
            Debug.Log(passives[i] + ":" + passiveLevels[i]);
        }
    }
    public List<string> passiveLevels;
    public List<string> GetPassiveLevels()
    {
        return passiveLevels;
    }
    public int GetCurrentLevel()
    {
        int level = 0;
        for (int i = 0; i < passiveLevels.Count; i++)
        {
            level += int.Parse(passiveLevels[i]);
        }
        return level;
    }
    public string maxLevel;
    public int GetMaxLevel()
    {
        return int.Parse(maxLevel);
    }
    public string rarity;
    public int GetRarity()
    {
        return int.Parse(rarity);
    }

    public void EquipToActor(TacticActor actor)
    {
        if (allStats.Length < 6) { return; }
        if (slot == "Weapon")
        {
            actor.SetWeaponType(type);
            actor.SetWeaponName(equipName);
        }
        for (int i = 0; i < passives.Count; i++)
        {
            actor.AddPassiveSkill(passives[i], passiveLevels[i]);
        }
    }

    public void UpgradeEquipment(string passiveUpgrade, int level = 1)
    {
        int indexOf = passives.IndexOf(passiveUpgrade);
        if (indexOf < 0)
        {
            passives.Add(passiveUpgrade);
            passiveLevels.Add(level.ToString());
        }
        else
        {
            passiveLevels[indexOf] = (int.Parse(passiveLevels[indexOf]) + level).ToString();
        }    
        RefreshStats();
    }

    public void EquipWeapon(TacticActor actor)
    {
        if (allStats.Length < 6){return;}
        if (slot == "Weapon")
        {
            actor.SetWeaponType(type);
        }
    }
}
