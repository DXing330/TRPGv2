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
        string[] dataBlocks = allStats.Split("|");
        slot = dataBlocks[1];
        type = dataBlocks[2];
        passives = dataBlocks[3].Split(",");
        passiveLevels = dataBlocks[4].Split(",");
        maxLevel = dataBlocks[5];
    }
    public string slot;
    public string type;
    public List<string> passives;
    public List<string> passiveLevels;
    public string maxLevel;

    public void EquipToActor(TacticActor actor)
    {
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
