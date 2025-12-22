using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "ScriptableObjects/BattleLogic/Passive", order = 1)]
public class PassiveSkill : SkillEffect
{
    public string GetEffectSpecifics(TacticActor actor, string specifics)
    {
        switch (specifics)
        {
            case "Defense":
                return actor.GetDefense().ToString();
            case "Attack":
                return actor.GetAttack().ToString();
        }
        return specifics;
    }

    public void AffectMap(TacticActor actor, string effect, string specifics, BattleMap map)
    {
        map.ChangeTile(actor.GetLocation(), effect, specifics);
    }

    public void ApplyStartBattlePassives(TacticActor actor, StatDatabase allData, BattleState battleState)
    {
        List<string> startBattlePassives = actor.GetStartBattlePassives();
        if (startBattlePassives.Count <= 0) { return; }
        for (int h = 0; h < startBattlePassives.Count; h++)
        {
            string[] passiveData = startBattlePassives[h].Split("|");
            if (passiveData.Length <= 4) { continue; }
            string[] conditions = passiveData[1].Split(",");
            string[] specifics = passiveData[2].Split(",");
            bool conditionsMet = true;
            for (int j = 0; j < conditions.Length; j++)
            {
                conditionsMet = CheckStartBattleCondition(conditions[j], specifics[j], actor, battleState);
                if (!conditionsMet)
                {
                    break;
                }
            }
            if (!conditionsMet)
            {
                continue;
            }
            string[] effects = passiveData[4].Split(",");
            string[] effectSpecifics = passiveData[5].Split(",");
            for (int i = 0; i < effects.Length; i++)
            {
                AffectActor(actor, effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
            }
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
        List<string> passiveData = new List<string>();
        List<TacticActor> targets = new List<TacticActor>();
        List<int> tiles = new List<int>();
        for (int h = passives.Count - 1; h >= 0; h--)
        {
            targets.Clear();
            passiveData = passives[h].Split("|").ToList();
            if (passiveData.Count <= 4) { continue; }
            if (!CheckStartEndConditions(actor, passiveData[1], passiveData[2], map))
            {
                continue;
            }
            string[] effects = passiveData[4].Split(",");
            string[] effectSpecifics = passiveData[5].Split(",");
            for (int i = 0; i < effects.Length; i++)
            {
                switch (passiveData[3])
                {
                    case "Self":
                        AffectActor(actor, effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                        break;
                    case "Adjacent Allies":
                        AffectActor(actor, effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                        targets = map.GetAdjacentAllies(actor);
                        for (int j = 0; j < targets.Count; j++)
                        {
                            AffectActor(targets[j], effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                        }
                        break;
                    case "Adjacent Enemies":
                        targets = map.GetAdjacentEnemies(actor);
                        for (int j = 0; j < targets.Count; j++)
                        {
                            AffectActor(targets[j], effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                        }
                        break;
                    case "Back Allies":
                        tiles.Add(map.ReturnTileInRelativeDirection(actor, 2));
                        tiles.Add(map.ReturnTileInRelativeDirection(actor, 3));
                        tiles.Add(map.ReturnTileInRelativeDirection(actor, 4));
                        targets = map.ReturnAlliesInTiles(actor, tiles);
                        for (int j = 0; j < targets.Count; j++)
                        {
                            AffectActor(targets[j], effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                        }
                        break;
                    case "Back Ally":
                        tiles.Add(map.ReturnTileInRelativeDirection(actor, 3));
                        targets = map.ReturnAlliesInTiles(actor,tiles);
                        for (int j = 0; j < targets.Count; j++)
                        {
                            AffectActor(targets[j], effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                        }
                        break;
                    case "Map":
                        AffectMap(actor, effects[i], effectSpecifics[i], map);
                        break;
                }
            }
        }
    }

    // Moving passives usually depend on the tile moved over.
    public bool CheckMovingCondition(string condition, string specifics, int currentTile, BattleMap map)
    {
        switch (condition)
        {
            case "Tile":
                return map.mapInfo[currentTile].Contains(specifics); // Contains, since DeepWater counts as Water
            case "Tile<>":
                return !map.mapInfo[currentTile].Contains(specifics);
            case "Elevation":
                return map.ReturnElevation(currentTile) == int.Parse(specifics);
            case "Elevation<>":
                return map.ReturnElevation(currentTile) != int.Parse(specifics);
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
                return battleState.GetWeather().Contains(specifics);
            case "Time":
                return specifics == battleState.GetTime();
        }
        return true;
    }

    public bool CheckStartEndConditions(TacticActor actor, string condition, string conditionSpecifics, BattleMap map)
    {
        string[] conditions = condition.Split(",");
        string[] specifics = conditionSpecifics.Split(",");
        for (int i = 0; i < conditions.Length; i++)
        {
            if (!CheckStartEndCondition(conditions[i], specifics[i], actor, map))
            {
                return false;
            }
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
            case "TileEffect":
                return map.GetTileEffectOfActor(actor).Contains(conditionSpecifics);
            case "TileEffect<>":
                return !map.GetTileEffectOfActor(actor).Contains(conditionSpecifics);
            case "Weather":
                return map.GetWeather().Contains(conditionSpecifics); // Contains, since we will add more weather tiers later.
            case "Time":
                return conditionSpecifics == map.GetTime();
            case "MoveType":
                return conditionSpecifics == actor.GetMoveType();
            case "MoveType<>":
                return conditionSpecifics != actor.GetMoveType();
            case "Range>":
                return actor.GetAttackRange() > int.Parse(conditionSpecifics);
            case "MentalState":
                return conditionSpecifics == actor.GetMentalState();
            case "Status":
                return actor.StatusExists(conditionSpecifics);
            case "Buff":
                return actor.BuffExists(conditionSpecifics);
            case "StatusCount>":
                return actor.GetStatuses().Count > int.Parse(conditionSpecifics);
            case "Adjacent Ally Sprite":
                return map.AllyAdjacentWithSpriteName(actor, conditionSpecifics);
            case "Adjacent Ally":
                return map.AllyAdjacentToActor(actor);
            case "Adjacent Ally<>":
                return !map.AllyAdjacentToActor(actor);
            case "AllyCount<":
                return map.AllAllies(actor).Count < int.Parse(conditionSpecifics);
            case "AllyCount>":
                return map.AllAllies(actor).Count > int.Parse(conditionSpecifics);
            case "AdjacentAllyCount>":
                return map.GetAdjacentAllies(actor).Count > int.Parse(conditionSpecifics);
            case "EnemyCount<":
                return map.AllEnemies(actor).Count < int.Parse(conditionSpecifics);
            case "EnemyCount>":
                return map.AllEnemies(actor).Count > int.Parse(conditionSpecifics);
            case "Round":
                switch (conditionSpecifics)
                {
                    case "Even":
                        return map.GetRound() % 2 == 0;
                    case "Odd":
                        return (map.GetRound() + 1) % 2 == 0;
                }
                return map.GetRound() % int.Parse(conditionSpecifics) == 0;
            case "Passive":
            return actor.GetPassiveSkills().Contains(conditionSpecifics);
            case "Passive<>":
            return !actor.GetPassiveSkills().Contains(conditionSpecifics);
            case "Counter":
            return actor.GetCounter() >= int.Parse(conditionSpecifics);
            case "Elevation":
            return map.ReturnElevation(actor.GetLocation()) == int.Parse(conditionSpecifics);
        }
        // Most of them have no condition.
        return true;
    }
    // Need to know about the actor, might have other actors to check as well. Might need to know about the tile.
    public bool CheckBattleConditions(string condition, string conditionSpecifics, TacticActor target, TacticActor attacker, BattleMap map, MoveCostManager moveManager)
    {
        string[] conditions = condition.Split(",");
        string[] specifics = conditionSpecifics.Split(",");
        for (int i = 0; i < conditions.Length; i++)
        {
            if (!CheckBattleCondition(conditions[i], specifics[i], target, attacker, map, moveManager))
            {
                return false;
            }
        }
        return true;
    }
    public bool CheckBattleCondition(string condition, string conditionSpecifics, TacticActor target, TacticActor attacker, BattleMap map, MoveCostManager moveManager)
    {
        switch (condition)
        {
            case "Tile":
                return map.GetTileInfoOfActor(target).Contains(conditionSpecifics);
            case "Tile<>":
                return !map.GetTileInfoOfActor(target).Contains(conditionSpecifics);
            case "TileA":
                return map.GetTileInfoOfActor(attacker).Contains(conditionSpecifics);
            case "Tile<>A":
                return !map.GetTileInfoOfActor(attacker).Contains(conditionSpecifics);
            case "TileEffectA":
                return map.GetTileEffectOfActor(attacker).Contains(conditionSpecifics);
            case "TileEffect<>A":
                return !map.GetTileEffectOfActor(attacker).Contains(conditionSpecifics);
            case "TileEffectD":
                return map.GetTileEffectOfActor(target).Contains(conditionSpecifics);
            case "TileEffect<>D":
                return !map.GetTileEffectOfActor(target).Contains(conditionSpecifics);
            case "Adjacent Ally A<>":
                return !map.AllyAdjacentToActor(attacker);
            case "Adjacent Ally D<>":
                return !map.AllyAdjacentToActor(target);
            case "Adjacent Ally A":
                return map.AllyAdjacentToActor(attacker);
            case "Adjacent Ally D":
                return map.AllyAdjacentToActor(target);
            case "Adjacent Ally Sprite A":
                return map.AllyAdjacentWithSpriteName(attacker, conditionSpecifics);
            case "Adjacent Ally Sprite D":
                return map.AllyAdjacentWithSpriteName(target, conditionSpecifics);
            case "AdjacentAllyCount>A":
                return map.GetAdjacentAllies(attacker).Count > int.Parse(conditionSpecifics);
            case "AdjacentAllyCount<A":
                return map.GetAdjacentAllies(attacker).Count < int.Parse(conditionSpecifics);
            case "AdjacentAllyCount>D":
                return map.GetAdjacentAllies(target).Count > int.Parse(conditionSpecifics);
            case "AdjacentAllyCount<D":
                return map.GetAdjacentAllies(target).Count < int.Parse(conditionSpecifics);
            case "AdjacentEnemyCount>A":
                return map.GetAdjacentEnemies(attacker).Count > int.Parse(conditionSpecifics);
            case "AdjacentEnemyCount<A":
                return map.GetAdjacentEnemies(attacker).Count < int.Parse(conditionSpecifics);
            case "AdjacentEnemyCount>D":
                return map.GetAdjacentEnemies(target).Count > int.Parse(conditionSpecifics);
            case "AdjacentEnemyCount<D":
                return map.GetAdjacentEnemies(target).Count < int.Parse(conditionSpecifics);
            case "None":
                return true;
            case "Killing":
                return attacker.GetAttack() >= target.GetDefense() + target.GetHealth();
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
            case "HealthD":
                return CheckHealthConditions(conditionSpecifics, target);
            case "HealthA":
                return CheckHealthConditions(conditionSpecifics, attacker);
            case "Weather":
                return map.GetWeather().Contains(conditionSpecifics);
            case "Weather<>":
                return !map.GetWeather().Contains(conditionSpecifics);
            case "Time":
                return conditionSpecifics == map.GetTime();
            case "Time<>":
                return conditionSpecifics != map.GetTime();
            case "MentalStateA":
                return conditionSpecifics == attacker.GetMentalState();
            case "MentalStateD":
                return conditionSpecifics == target.GetMentalState();
            case "StatusA":
                return attacker.StatusExists(conditionSpecifics);
            case "StatusD":
                return target.StatusExists(conditionSpecifics);
            case "RangeD>":
                return target.GetAttackRange() > int.Parse(conditionSpecifics);
            case "RangeD<":
                return target.GetAttackRange() < int.Parse(conditionSpecifics);
            case "RangeA>":
                return attacker.GetAttackRange() > int.Parse(conditionSpecifics);
            case "RangeA<":
                return attacker.GetAttackRange() < int.Parse(conditionSpecifics);
            case "PassiveLevelsD>":
                return target.GetTotalPassiveLevels() > int.Parse(conditionSpecifics);
            case "PassiveLevelsD<":
                return target.GetTotalPassiveLevels() < int.Parse(conditionSpecifics);
            case "PassiveLevelsA>":
                return attacker.GetTotalPassiveLevels() > int.Parse(conditionSpecifics);
            case "PassiveLevelsA<":
                return attacker.GetTotalPassiveLevels() < int.Parse(conditionSpecifics);
            case "MoveType<>A":
                return attacker.GetMoveType() != conditionSpecifics;
            case "MoveType<>D":
                return target.GetMoveType() != conditionSpecifics;
            case "MoveTypeA":
                return attacker.GetMoveType() == conditionSpecifics;
            case "MoveTypeD":
                return target.GetMoveType() == conditionSpecifics;
            case "Team":
                if (conditionSpecifics == "Same")
                {
                    return attacker.GetTeam() == target.GetTeam();
                }
                return attacker.GetTeam() != target.GetTeam();
            case "Direction<>D":
                return GetAttackDirectionFromDefenderPOV(attacker.GetDirection(), target.GetDirection()) != int.Parse(conditionSpecifics);
            case "DirectionD":
                return GetAttackDirectionFromDefenderPOV(attacker.GetDirection(), target.GetDirection()) == int.Parse(conditionSpecifics);
            case "Elevation<>D":
                return map.ReturnElevation(target.GetLocation()) != int.Parse(conditionSpecifics);
            case "Elevation<>A":
                return map.ReturnElevation(attacker.GetLocation()) != int.Parse(conditionSpecifics);
            case "ElevationD":
                return map.ReturnElevation(target.GetLocation()) == int.Parse(conditionSpecifics);
            case "ElevationA":
                return map.ReturnElevation(attacker.GetLocation()) == int.Parse(conditionSpecifics);
            case "Elevation=":
                return map.ReturnElevation(attacker.GetLocation()) == map.ReturnElevation(target.GetLocation());
            case "Elevation>":
                return map.ReturnElevation(attacker.GetLocation()) > map.ReturnElevation(target.GetLocation());
            case "Elevation<":
                return map.ReturnElevation(attacker.GetLocation()) < map.ReturnElevation(target.GetLocation());
            case "ElementD":
                return target.SameElement(conditionSpecifics);
            case "Element<>D":
                return target.SameElement(conditionSpecifics);
            case "ElementA":
                return attacker.SameElement(conditionSpecifics);
            case "Element<>A":
                return attacker.SameElement(conditionSpecifics);
            case "SpeciesD":
                return target.GetSpecies() == conditionSpecifics;
            case "Species<>D":
                return target.GetSpecies() != conditionSpecifics;
            case "SpeciesA":
                return attacker.GetSpecies() == conditionSpecifics;
            case "Species<>A":
                return attacker.GetSpecies() != conditionSpecifics;
        }
        return true;
    }

    public bool CheckHealthConditions(string conditionSpecifics, TacticActor target)
    {
        int maxHealth = target.GetBaseHealth();
        int currentHealth = target.GetHealth();
        switch (conditionSpecifics)
        {
            case "<25%":
                return (currentHealth * 4) < maxHealth;
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
        return true;
    }

    public string CheckRelativeDirections(int dir1, int dir2)
    {
        int directionDiff = GetRelativeDirections(dir1, dir2);
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

    public int GetRelativeDirections(int dir1, int dir2)
    {
        return Mathf.Abs(dir1-dir2);
    }

    public int GetAttackDirectionFromDefenderPOV(int atkDir, int defDir)
    {
        int mod = 3 - defDir;
        if (mod < 0)
        {
            mod += 6;
        }
        int final = (atkDir + mod) % 6;
        return final;
    }

    public bool CheckDirectionIntSpecifics(int conditionSpecifics, int directions)
    {
        return conditionSpecifics == directions;
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
}
