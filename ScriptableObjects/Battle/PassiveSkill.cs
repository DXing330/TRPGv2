using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "ScriptableObjects/BattleLogic/Passive", order = 1)]
public class PassiveSkill : SkillEffect
{
    public void ApplyStartBattlePassives(TacticActor actor, StatDatabase allData)
    {
        List<string> startBattlePassives = actor.GetStartBattlePassives();
        if (startBattlePassives.Count <= 0){return;}
        string passiveName = "";
        List<string> passiveData = new List<string>();
        for (int i = 0; i < startBattlePassives.Count; i++)
        {
            passiveName = startBattlePassives[i];
            if (passiveName.Length <= 1){continue;}
            passiveData = allData.ReturnStats(passiveName);
            if (!CheckStartBattleCondition(passiveData[1], passiveData[2], actor)){continue;}
            AffectActor(actor, passiveData[4], passiveData[5]);
        }
    }

    public void ApplyPassives(TacticActor actor, StatDatabase allData, string timing)
    {
        List<string> passives = new List<string>();
        switch (timing)
        {
            case "Start":
            passives = actor.GetStartTurnPassives();
            break;
            case "End":
            passives = actor.GetEndTurnPassives();
            break;
        }
        string passiveName = "";
        List<string> passiveData = new List<string>();
        for (int i = 0; i < passives.Count; i++)
        {
            passiveName = passives[i];
            if (passiveName.Length <= 1){continue;}
            passiveData = allData.ReturnStats(passiveName);
            if (!CheckStartEndCondition(passiveData[1], passiveData[2], actor)){continue;}
            AffectActor(actor, passiveData[4], passiveData[5]);
        }
    }

    // Moving passives usually depend on the tile moved over.
    public bool CheckMovingCondition(string specifics, string tileType)
    {
        return specifics == tileType;
    }

    // Start Battle passives only depend on passive user/equipment.
    public bool CheckStartBattleCondition(string condition, string specifics, TacticActor actor)
    {
        switch (condition)
        {
            case "Weapon":
            return specifics == actor.GetWeaponType();
        }
        return true;
    }
    
    // Start/End passives only depend on the passive user.
    public bool CheckStartEndCondition(string condition, string conditionSpecifics, TacticActor actor)
    {
        switch (condition)
        {
            case "None":
            return true;
            case "Health":
            return CheckHealthConditions(conditionSpecifics, actor);
        }
        // Most of them have no condition.
        return true;
    }
    // Need to know about the actor, might have other actors to check as well. Might need to know about the tile.
    public bool CheckBattleCondition(string condition, string conditionSpecifics, TacticActor target, TacticActor attacker, BattleMap map, MoveCostManager moveManager)
    {
        switch (condition)
        {
            case "Tile":
            return CheckConditionSpecifics(conditionSpecifics, map.mapInfo[target.GetLocation()]);
            case "Adjacent Ally":
            // Need to check adjacent tiles for allies.
            return false;
            case "None":
            return true;
            case "Distance":
            return moveManager.DistanceBetweenActors(target, attacker) <= int.Parse(conditionSpecifics);
            case "Sprite":
            return CheckConditionSpecifics(conditionSpecifics, target.GetSpriteName());
            case "Direction":
            return CheckDirectionSpecifics(conditionSpecifics, CheckRelativeDirections(target.GetDirection(), attacker.GetDirection()));
            case "Health":
            return CheckHealthConditions(conditionSpecifics, target);
        }
        return false;
    }

    public bool CheckHealthConditions(string conditionSpecifics, TacticActor target)
    {
        int maxHealth = target.GetBaseHealth();
        int currentHealth = target.GetHealth();
        switch (conditionSpecifics)
        {
            case "<Half":
            return (currentHealth * 2) < maxHealth;
            case ">Half":
            return (currentHealth * 2) > maxHealth;
            case "Full":
            return currentHealth >= maxHealth;
        }
        return false;
    }

    public bool CheckTakeDamageCondition(string condition, string conditionSpecifics, int damageAmount, string damageType)
    {
        switch (condition)
        {
            case "None":
            return true;
            case "Type":
            return conditionSpecifics == damageType;
            case "<":
            return int.Parse(conditionSpecifics) < damageAmount;
            case ">":
            return int.Parse(conditionSpecifics) > damageAmount;
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
            affected += power * affected / basicDenominator;
            break;
            case "Decrease%":
            affected -= power * affected / basicDenominator;
            break;
        }
        return affected;
    }
}
