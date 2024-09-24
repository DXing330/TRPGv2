using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "ScriptableObjects/BattleLogic/Passive", order = 1)]
public class PassiveSkill : ScriptableObject
{
    public void ApplyStartBattlePassives(TacticActor actor, StatDatabase allData)
    {
        List<string> startBattlePassives = actor.allStats.GetStartBattlePassives();
        if (startBattlePassives.Count <= 0){return;}
        string passiveName = "";
        List<string> passiveData = new List<string>();
        for (int i = 0; i < startBattlePassives.Count; i++)
        {
            passiveName = startBattlePassives[i];
            if (passiveName.Length <= 1){continue;}
            passiveData = allData.ReturnStats(passiveName);
            AffectTarget(actor, passiveData[4], passiveData[5]);
        }
    }
    // Need to know about the actor, might have other actors to check as well. Might need to know about the tile.
    public bool CheckBattleCondition(string condition, string conditionSpecifics, TacticActor targetedActor, TacticActor otherActor, BattleMap map, MoveCostManager moveManager)
    {
        switch (condition)
        {
            case "Tile":
            return CheckConditionSpecifics(conditionSpecifics, map.mapInfo[targetedActor.GetLocation()]);
            case "Adjacent Ally":
            // Need to check adjacent tiles for allies.
            return false;
            case "None":
            return true;
            case "Distance":
            return moveManager.DistanceBetweenActors(targetedActor, otherActor) <= int.Parse(conditionSpecifics);
            case "Sprite":
            return CheckConditionSpecifics(conditionSpecifics, targetedActor.GetSpriteName());
            case "Direction":
            return CheckDirectionSpecifics(conditionSpecifics, CheckRelativeDirections(targetedActor.GetDirection(), otherActor.GetDirection()));
        }
        return false;
    }

    public string CheckRelativeDirections(int dir1, int dir2)
    {
        int directionDiff = Mathf.Abs(dir1 - dir2);
        switch (directionDiff)
        {
            case 0:
            return "Same";
            case 1:
            return "Back";
            case 2:
            return "Face";
            case 3:
            return "Opposite";
            case 4:
            return "Face";
            case 5:
            return "Back";
        }
        return "None";
    }

    public bool CheckDirectionSpecifics(string conditionSpecifics, string specifics)
    {
        if (conditionSpecifics == "Back" && specifics == "Same"){return true;}
        else if (conditionSpecifics == "Face" && specifics == "Opposite"){return true;}
        return (conditionSpecifics == specifics);
    }

    public bool CheckConditionSpecifics(string conditionSpecifics, string specifics)
    {
        return (conditionSpecifics == specifics);
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
            case "Attack":
            target.allStats.UpdateAttack(int.Parse(effectSpecifics)*level, false);
            break;
            case "Defense":
            target.allStats.UpdateDefense(int.Parse(effectSpecifics)*level, false);
            break;
            case "BaseHealth":
            target.allStats.UpdateBaseHealth(int.Parse(effectSpecifics)*level, false);
            target.allStats.UpdateHealth(int.Parse(effectSpecifics)*level, false);
            break;
            case "BaseAttack":
            target.allStats.UpdateBaseAttack(int.Parse(effectSpecifics)*level, false);
            break;
            case "BaseDefense":
            target.allStats.UpdateBaseDefense(int.Parse(effectSpecifics)*level, false);
            break;
            case "Skill":
            // Add an active skill.
            string[] newSkills = effectSpecifics.Split(",");
            for (int i = 0; i < newSkills.Length; i++)
            {
                target.allStats.AddActiveSkill(newSkills[i]);
            }
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
