using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public StatDatabase weaponReach;
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
        allStats += maxUpgrades + "|";
        allStats += rarity + "|";
    }
    public void ResetStats()
    {
        equipName = "";
        slot = "-1";
        type = "-1";
        passives.Clear();
        passiveLevels.Clear();
        maxUpgrades = 0;
        rarity = "0";
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
        for (int i = 0; i < dataBlocks.Length; i++)
        {
            SetStat(dataBlocks[i], i);
        }
    }
    protected void SetStat(string stat, int index)
    {
        switch (index)
        {
            case 0:
            SetName(stat);
            break;
            case 1:
            SetSlot(stat);
            break;
            case 2:
            SetType(stat);
            break;
            case 3:
            SetPassives(stat.Split(",").ToList());
            break;
            case 4:
            SetPassiveLevels(stat.Split(",").ToList());
            break;
            case 5:
            SetMaxUpgrades(int.Parse(stat));
            break;
            case 6:
            SetRarity(stat);
            break;
        }
    }
    public string equipName;
    public void SetName(string newInfo)
    {
        equipName = newInfo;
    }
    public string GetName(){return equipName;}
    public string slot;
    public void SetSlot(string newInfo)
    {
        slot = newInfo;
    }
    public string GetSlot(){return slot;}
    public string type;
    public void SetType(string newInfo)
    {
        type = newInfo;
    }
    public string GetEquipType(){return type;}
    public List<string> GetPassivesAndLevels()
    {
        List<string> passivesAndLevels = new List<string>();
        for (int i = 0; i < passives.Count; i++)
        {
            passivesAndLevels.Add(passives[i] + ":" + passiveLevels[i]);
        }
        return passivesAndLevels;
    }
    public List<string> passives;
    public void SetPassives(List<string> newInfo)
    {
        passives = newInfo;
    }
    public List<string> GetPassives()
    {
        return passives;
    }
    public void AddPassive(string passiveName)
    {
        if (passiveName.Length <= 1){return;}
        int indexOf = passives.IndexOf(passiveName);
        int cLevel = GetLevelOfPassive(passiveName);
        if (cLevel > 0)
        {
            passiveLevels[indexOf] = (cLevel + 1).ToString();
        }
        else
        {
            passives.Add(passiveName);
            passiveLevels.Add("1");
        }
    }
    public int GetLevelOfPassive(string passiveName)
    {
        int indexOf = passives.IndexOf(passiveName);
        if (indexOf >= 0)
        {
            return int.Parse(passiveLevels[indexOf]);
        }
        return 0;
    }
    public int GetTotalLevel()
    {
        int level = 0;
        for(int i = 0; i < passiveLevels.Count; i++)
        {
            level += int.Parse(passiveLevels[i]);
        }
        return level;
    }
    public void DebugPassives()
    {
        for (int i = 0; i < passives.Count; i++)
        {
            Debug.Log(passives[i] + ":" + passiveLevels[i]);
        }
    }
    public List<string> passiveLevels;
    public void SetPassiveLevels(List<string> newInfo)
    {
        passiveLevels = newInfo;
    }
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
    public int maxUpgrades;
    public bool UpgradesAvailable()
    {
        return maxUpgrades > 0;
    }
    public void ConsumeUpgrade()
    {
        maxUpgrades--;
    }
    public void SetMaxUpgrades(int newInfo)
    {
        maxUpgrades = newInfo;
    }
    public int GetMaxUpgrades()
    {
        return maxUpgrades;
    }
    public string rarity;
    public void SetRarity(string newInfo)
    {
        rarity = newInfo;
    }
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
            actor.SetWeaponStats(allStats);
            actor.SetWeaponReach(int.Parse(weaponReach.ReturnValue(type)));
        }
        for (int i = 0; i < passives.Count; i++)
        {
            actor.AddPassiveSkill(passives[i], passiveLevels[i]);
        }
    }

    public void UpgradeEquipment(string passiveUpgrade, int level = 1)
    {
        for (int i = 0; i < level; i++)
        {
            if (!UpgradesAvailable()){return;}
            AddPassive(passiveUpgrade);
            ConsumeUpgrade();
            RefreshStats();
        }   
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
