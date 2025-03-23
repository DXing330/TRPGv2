using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDescriptionViewer : MonoBehaviour
{
    public string ReturnActiveDescription(ActiveSkill activeSkill)
    {
        return AED(activeSkill.GetEffect(), activeSkill.GetSpecifics(), activeSkill.GetPower().ToString());
    }

    // ActiveEffectDescription
    public string AED(string e, string s, string p)
    {
        switch (e)
        {
            case "Attack":
            return "Attack the target(s) "+ASD(s)+" time(s) with "+APD(p)+"% damage.";
            // The specifics will determine the direction anyway.
            case "Displace":
            return ASD(s)+" the target(s) with "+APD(p)+" additional force.";
            case "Teleport":
            return "Move to the targeted tile.";
            case "Status":
            return "Give the target(s) "+ASD(s)+" for "+APD(p)+" turns.";
            case "Summon":
            return "Summon a "+ASD(s)+".";
            case "TerrainEffect":
            return "Create "+ASD(s)+" on targeted tile(s).";
            case "Trap":
            return "Set up a "+ASD(s)+" trap on the targeted tile(s).";
            case "Taunt":
            return "Make target(s) more likely to attack.";
            case "Attack+Move":
            return "Attack the target(s) and move "+ASD(s)+" "+APD(p)+" tile(s).";
            case "Move+Attack":
            return "Move to the targeted tile and attack any target in "+ASD(s)+" of the the targeted tile.";
            case "Temporary Passive":
            return "Give the target(s) "+ASD(s)+" for "+APD(p)+" turns.";
            case "Attack+Status":
            return "Attack the target(s) and inflict "+ASD(s)+" for "+APD(p)+" turns.";
            case "Attack+Drain":
            return "Attack the target(s) and absorb some damage as health.";
        }
        return "The target(s) gain "+ASD(s)+" "+e+".";
    }

    // ActiveSpecificsDescription
    public string ASD(string specifics)
    {
        return specifics;
    }

    // ActivePowerDescription
    public string APD(string power)
    {

        return power;
    }
}
