using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "ScriptableObjects/BattleLogic/Passive", order = 1)]
public class PassiveSkill : SkillEffect
{
    public void ApplyStartBattlePassives(TacticActor actor, StatDatabase allData, BattleState battleState)
    {
        List<string> startBattlePassives = actor.GetStartBattlePassives();
        if (startBattlePassives.Count <= 0) { return; }
        string passiveName = "";
        List<string> passiveData = new List<string>();
        for (int i = 0; i < startBattlePassives.Count; i++)
        {
            passiveName = startBattlePassives[i];
            if (passiveName.Length <= 1) { continue; }
            passiveData = allData.ReturnStats(passiveName);
            if (!CheckStartBattleCondition(passiveData[1], passiveData[2], actor, battleState)) { continue; }
            AffectActor(actor, passiveData[4], passiveData[5]);
        }
    }

    public void ApplyPassives(TacticActor actor, StatDatabase allData, string timing, BattleMap map)
    {
        List<string> passives = new List<string>();
        switch (timing)
        {
            case "Start":
                passives = new List<string>(actor.GetStartTurnPassives());
                passives.Add(map.ReturnTerrainStartPassive(actor));
                break;
            case "End":
                passives = new List<string>(actor.GetEndTurnPassives());
                passives.Add(map.ReturnTerrainEndPassive(actor));
                break;
        }
        string passiveName = "";
        List<string> passiveData = new List<string>();
        List<TacticActor> targets = new List<TacticActor>();
        for (int i = passives.Count - 1; i >= 0; i--)
        {
            targets.Clear();
            passiveName = passives[i];
            if (passiveName.Length <= 1) { continue; }
            passiveData = allData.ReturnStats(passiveName);
            if (!CheckStartEndCondition(passiveData[1], passiveData[2], actor, map)) { continue; }
            switch (passiveData[3])
            {
                case "Self":
                    AffectActor(actor, passiveData[4], passiveData[5]);
                    break;
                case "Adjacent Allies":
                    AffectActor(actor, passiveData[4], passiveData[5]);
                    targets = map.GetAdjacentAllies(actor);
                    for (int j = 0; j < targets.Count; j++)
                    {
                        AffectActor(targets[j], passiveData[4], passiveData[5]);
                    }
                    break;
                case "Adjacent Enemies":
                    targets = map.GetAdjacentEnemies(actor);
                    for (int j = 0; j < targets.Count; j++)
                    {
                        AffectActor(targets[j], passiveData[4], passiveData[5]);
                    }
                    break;
            }
        }
    }

    // Moving passives usually depend on the tile moved over.
    public bool CheckMovingCondition(string condition, string specifics, string currentTile)
    {
        switch (condition)
        {
            case "Tile":
                return currentTile.Contains(specifics); // Contains, since DeepWater counts as Water
            case "Tile<>":
                return !currentTile.Contains(specifics);
        }
        return true;
    }

    public bool CheckStartBattleCondition(string condition, string specifics, TacticActor actor, BattleState battleState)
    {
        switch (condition)
        {
            case "Weapon":
                return specifics == actor.GetWeaponType();
            case "Weather":
                return specifics == battleState.GetWeather();
            case "Time":
                return specifics == battleState.GetTime();
        }
        return true;
    }

    public bool CheckStartEndCondition(string condition, string conditionSpecifics, TacticActor actor, BattleMap map)
    {
        switch (condition)
        {
            case "None":
                return true;
            case "Health":
                return CheckHealthConditions(conditionSpecifics, actor);
            case "Tile":
                return map.GetTileInfoOfActor(actor).Contains(conditionSpecifics); // Contains, since DeepWater counts as Water
            case "Tile<>":
                return !map.GetTileInfoOfActor(actor).Contains(conditionSpecifics); // Contains, since DeepWater counts as Water
            case "Weather":
                return conditionSpecifics == map.GetWeather();
            case "Time":
                return conditionSpecifics == map.GetTime();
            case "MoveType":
                return conditionSpecifics == actor.GetMoveType();
            case "MoveType<>":
                return conditionSpecifics != actor.GetMoveType();
            case "MentalState":
                return conditionSpecifics == actor.GetMentalState();
            case "Status":
                return actor.StatusExists(conditionSpecifics);
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
                return map.GetTileInfoOfActor(target).Contains(conditionSpecifics);
            case "Tile<>":
                return !map.GetTileInfoOfActor(target).Contains(conditionSpecifics);
            case "Adjacent Ally":
                // Need to check adjacent tiles for allies.
                return false;
            case "None":
                return true;
            case "Distance":
                return moveManager.DistanceBetweenActors(target, attacker) <= int.Parse(conditionSpecifics);
            case "Distance>":
                return moveManager.DistanceBetweenActors(target, attacker) >= int.Parse(conditionSpecifics);
            case "Sprite":
                return CheckConditionSpecifics(conditionSpecifics, target.GetSpriteName());
            case "Direction":
                return CheckDirectionSpecifics(conditionSpecifics, CheckRelativeDirections(target.GetDirection(), attacker.GetDirection()));
            case "Health":
                return CheckHealthConditions(conditionSpecifics, target);
            case "Weather":
                return conditionSpecifics == map.GetWeather();
            case "Time":
                return conditionSpecifics == map.GetTime();
            case "MentalStateA":
                return conditionSpecifics == attacker.GetMentalState();
            case "MentalStateD":
                return conditionSpecifics == target.GetMentalState();
            case "StatusA":
                return attacker.StatusExists(conditionSpecifics);
            case "StatusD":
                return target.StatusExists(conditionSpecifics);
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
                return "Front";
            case 3:
                return "Opposite";
            case 4:
                return "Front";
            case 5:
                return "Back";
        }
        return "None";
    }

    public bool CheckDirectionSpecifics(string conditionSpecifics, string specifics)
    {
        if (conditionSpecifics == "Back" && specifics == "Same") { return true; }
        else if (conditionSpecifics == "Front" && specifics == "Opposite") { return true; }
        return (conditionSpecifics == specifics);
    }

    public bool CheckConditionSpecifics(string conditionSpecifics, string specifics)
    {
        return (conditionSpecifics == specifics);
    }

    public int AffectInt(int affected, string effect, string effectSpecifics, int level = 1)
    {
        int power = 0;
        bool intEffect = int.TryParse(effectSpecifics, out power);
        if (!intEffect) { return affected; }
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

    public void AffectMap(BattleMap map, int location, string effect, string effectSpecifics)
    {
        switch (effect)
        {
            case "Tile":
                map.ChangeTerrain(location, effectSpecifics);
                break;
            case "TerrainEffect":
                map.ChangeTerrainEffect(location, effectSpecifics);
                break;
        }
    }
}
