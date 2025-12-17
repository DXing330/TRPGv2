using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorEquipment : ActorPassives
{
    public void SetStats(List<string> newStats)
    {
        slot = newStats[0];
        equipType = newStats[1];
        SetPassiveSkills(newStats[2].Split(",").ToList());
    }
    public string slot;
    public string equipType;
}
