using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "ScriptableObjects/Passive", order = 1)]
public class PassiveSkill : ScriptableObject
{
    // Need to know about the actor, might have other actors to check as well. Might need to know about the tile.
    public bool CheckBattleCondition(string condition, string conditionSpecifics, TacticActor targetedActor, TacticActor otherActor = null, int distance = -1, List<string> mapInfo = null)
    {
        switch (condition)
        {
            case "Tile":
            return CheckConditionSpecifics(conditionSpecifics, mapInfo[targetedActor.GetLocation()]);
            case "None":
            return true;
            case "Distance":
            return distance < int.Parse(conditionSpecifics);
            case "Sprite":
            return CheckConditionSpecifics(conditionSpecifics, targetedActor.GetSpriteName());
            case "Direction":
            return CheckConditionSpecifics(conditionSpecifics, CheckRelativeDirections(targetedActor.GetDirection(), otherActor.GetDirection()));
        }
        return false;
    }

    public string CheckRelativeDirections(int dir1, int dir2)
    {
        if (dir1 == dir2){return "Same";}
        return "None";
    }

    public bool CheckConditionSpecifics(string conditionSpecifics, string specifics)
    {
        return (conditionSpecifics == specifics);
    }

    public bool CheckTiming(string timing, string time)
    {
        return (timing == time);
    }

    public void AffectTarget(TacticActor target, string effect, string effectSpecifics, int level = 1)
    {
        switch (effect)
        {
            case "Status":
            target.allStats.AddCondition(effectSpecifics, level);
            break;
            // Default is increasing health.
            case "Health":
            target.allStats.UpdateHealth(int.Parse(effectSpecifics)*level, false);
            break;
        }
    }

    public int AffectInt(int affected, string effect, string effectSpecifics, int level = 1)
    {
        int power = int.Parse(effectSpecifics) * level;
        switch (effect)
        {
            case "Increase":
            affected += power;
            break;
            case "Decrease":
            affected -= power;
            break;
            case "Increase%":
            affected += power * affected / 10;
            break;
            case "Decrease%":
            affected -= power * affected / 10;
            break;
        }
        return affected;
    }
}
