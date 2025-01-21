using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string allStats;
    public void SetAllStats(string newStats)
    {
        allStats = newStats;
        if (allStats.Length < 6){return;}
        string[] dataBlocks = allStats.Split("|");
        slot = dataBlocks[1];
        type = dataBlocks[2];
        passives = dataBlocks[3].Split(",").ToList();
        passiveLevels = dataBlocks[4].Split(",").ToList();
        maxLevel = dataBlocks[5];
    }
    public string slot;
    public string type;
    public List<string> passives;
    public List<string> passiveLevels;
    public string maxLevel;

    public void EquipToActor(TacticActor actor)
    {
        if (allStats.Length < 6){return;}
        if (slot == "Weapon")
        {
            actor.SetWeaponType(type);
        }
        for (int i = 0; i < passives.Count; i++)
        {
            actor.AddPassiveSkill(passives[i], passiveLevels[i]);
        }
    }
}
