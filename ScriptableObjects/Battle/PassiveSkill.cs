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
            case "Attack/2":
                return (actor.GetAttack() / 2).ToString();
        }
        return specifics;
    }

    public void AffectMap(TacticActor actor, string effect, string specifics, BattleMap map)
    {
        map.ChangeTile(actor.GetLocation(), effect, specifics);
    }

    public void ApplyStartBattlePassives(TacticActor actor, BattleState battleState)
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

    public void ApplyPassive(TacticActor actor, BattleMap map, string passive)
    {
        List<TacticActor> targets = new List<TacticActor>();
        List<int> tiles = new List<int>();
        List<string> passiveData = passive.Split("|").ToList();
        if (passiveData.Count <= 4) { return; }
        if (!CheckStartEndConditions(actor, passiveData[1], passiveData[2], map)){return;}
        string[] effects = passiveData[4].Split(",");
        string[] effectSpecifics = passiveData[5].Split(",");
        for (int i = 0; i < effects.Length; i++)
        {
            switch (passiveData[3])
            {
                // Move based on wording (effect) and distance (specifics).
                case "MoveSelf":
                    map.MoveActorPassive(actor, effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                    break;
                case "Self":
                    AffectActor(actor, effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                    break;
                case "AdjacentAllies":
                    AffectActor(actor, effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                    targets = map.GetAdjacentAllies(actor);
                    for (int j = 0; j < targets.Count; j++)
                    {
                        AffectActor(targets[j], effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                    }
                    break;
                case "AdjacentEnemies":
                    targets = map.GetAdjacentEnemies(actor);
                    for (int j = 0; j < targets.Count; j++)
                    {
                        AffectActor(targets[j], effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                    }
                    break;
                case "AdjacentActors":
                    break;
                case "BackAllies":
                    tiles.Add(map.ReturnTileInRelativeDirection(actor, 2));
                    tiles.Add(map.ReturnTileInRelativeDirection(actor, 3));
                    tiles.Add(map.ReturnTileInRelativeDirection(actor, 4));
                    targets = map.ReturnAlliesInTiles(actor, tiles);
                    for (int j = 0; j < targets.Count; j++)
                    {
                        AffectActor(targets[j], effects[i], GetEffectSpecifics(actor, effectSpecifics[i]));
                    }
                    break;
                case "BackAlly":
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

    public void ApplyPassives(TacticActor actor, string timing, BattleMap map)
    {
        List<string> passives = new List<string>();
        if (timing == "End")
        {
            passives = actor.GetEndTurnPassives();
        }
        if (timing == "Start")
        {
            passives = actor.GetStartTurnPassives();
        }
        for (int h = passives.Count - 1; h >= 0; h--)
        {
            ApplyPassive(actor, map, passives[h]);
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
            case "Elevation<":
                return map.ReturnElevation(currentTile) <= int.Parse(specifics);
            case "Elevation>":
                return map.ReturnElevation(currentTile) >= int.Parse(specifics);
            case "Elevation":
                return map.ReturnElevation(currentTile) == int.Parse(specifics);
        }
        return true;
    }

    public bool CheckStartBattleCondition(string condition, string specifics, TacticActor actor, BattleState battleState)
    {
        switch (condition)
        {
            case "Weapon":
                return specifics == actor.GetWeaponType();
            case "Weapon<>":
                return actor.NoWeapon();
            case "Weather":
                return battleState.GetWeather().Contains(specifics);
            case "Weather<>":
                return !battleState.GetWeather().Contains(specifics);
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
            case "Energy":
                return CheckEnergyConditions(conditionSpecifics, actor);
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
            case "Weather<>":
                return !map.GetWeather().Contains(conditionSpecifics);
            case "Time":
                return conditionSpecifics == map.GetTime();
            case "MoveType":
                return conditionSpecifics == actor.GetMoveType();
            case "MoveType<>":
                return conditionSpecifics != actor.GetMoveType();
            case "Range>":
                return actor.GetAttackRange() > int.Parse(conditionSpecifics);
            case "Range<":
                return actor.GetAttackRange() < int.Parse(conditionSpecifics);
            case "MentalState":
                return conditionSpecifics == actor.GetMentalState();
            case "Status":
                return actor.StatusExists(conditionSpecifics);
            case "Status<>":
                return !actor.StatusExists(conditionSpecifics); 
            case "Buff":
                return actor.BuffExists(conditionSpecifics);
            case "StatusCount>":
                return actor.GetStatuses().Count > int.Parse(conditionSpecifics);
            case "AdjacentAllySprite":
                return map.AllyAdjacentWithSpriteName(actor, conditionSpecifics);
            case "AdjacentAlly":
                return map.AllyAdjacentToActor(actor);
            case "AdjacentAlly<>":
                return !map.AllyAdjacentToActor(actor);
            case "AdjacentAllyCount>":
                return map.GetAdjacentAllies(actor).Count > int.Parse(conditionSpecifics);
            case "AllyCount<":
                return map.AllAllies(actor).Count < int.Parse(conditionSpecifics);
            case "AllyCount>":
                return map.AllAllies(actor).Count > int.Parse(conditionSpecifics);
            case "Ally<Enemy":
                return map.AllAllies(actor).Count < map.AllEnemies(actor).Count;
            case "Ally>Enemy":
                return map.AllAllies(actor).Count > map.AllEnemies(actor).Count;
            case "AllyEqualsEnemy":
                return map.AllAllies(actor).Count == map.AllEnemies(actor).Count;
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
            case "CounterAttack":
            return actor.CounterAttackAvailable();
            case "Elevation<":
            return map.ReturnElevation(actor.GetLocation()) <= int.Parse(conditionSpecifics);
            case "Elevation>":
            return map.ReturnElevation(actor.GetLocation()) >= int.Parse(conditionSpecifics);
            case "Element":
            return actor.SameElement(conditionSpecifics);
            case "Element<>":
            return !actor.SameElement(conditionSpecifics);
            case "Weapon<>":
            return actor.NoWeapon();
            case "Weapon":
            return conditionSpecifics == actor.GetWeaponType();
            case "AverageHP>":
            return actor.GetHealth() > map.AverageActorHealth();
            case "AverageHP<":
            return actor.GetHealth() < map.AverageActorHealth();
            // Too hard to activate, this should be a win battle condition or something.
            /*case "AverageHP":
            return actor.GetHealth() == map.AverageActorHealth();*/
            case "Grappling":
            return actor.Grappling();
            case "Grappled":
            return actor.Grappled();
            case "BadRNG":
            return utility.Roll(actor.GetLuck()) < int.Parse(conditionSpecifics);
            // Good RNG means you want to roll lower to get the chance.
            case "GoodRNG":
            return utility.Roll(-actor.GetLuck()) < int.Parse(conditionSpecifics);
        }
        // Most of them have no condition.
        return true;
    }
    // Need to know about the actor, might have other actors to check as well. Might need to know about the tile.
    public bool CheckBattleConditions(string condition, string conditionSpecifics, TacticActor target, TacticActor attacker, BattleMap map)
    {
        string[] conditions = condition.Split(",");
        string[] specifics = conditionSpecifics.Split(",");
        for (int i = 0; i < conditions.Length; i++)
        {
            if (!CheckBattleCondition(conditions[i], specifics[i], target, attacker, map))
            {
                return false;
            }
        }
        return true;
    }
    public bool CheckBattleCondition(string condition, string conditionSpecifics, TacticActor target, TacticActor attacker, BattleMap map)
    {
        switch (condition)
        {
            case "TileD":
                return map.GetTileInfoOfActor(target).Contains(conditionSpecifics);
            case "Tile<>D":
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
            case "AdjacentAlly<>A":
                return !map.AllyAdjacentToActor(attacker);
            case "AdjacentAlly<>D":
                return !map.AllyAdjacentToActor(target);
            case "AdjacentAllyA":
                return map.AllyAdjacentToActor(attacker);
            case "AdjacentAllyD":
                return map.AllyAdjacentToActor(target);
            case "AdjacentAllySpriteA":
                return map.AllyAdjacentWithSpriteName(attacker, conditionSpecifics);
            case "AdjacentAllySpriteD":
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
                return map.DistanceBetweenActors(target, attacker) <= int.Parse(conditionSpecifics);
            case "Distance>":
                return map.DistanceBetweenActors(target, attacker) > int.Parse(conditionSpecifics);
            case "Sprite":
                return CheckConditionSpecifics(conditionSpecifics, target.GetSpriteName());
            case "Health":
                return CheckHealthConditions(conditionSpecifics, target);
            case "HealthD":
                return CheckHealthConditions(conditionSpecifics, target);
            case "HealthA":
                return CheckHealthConditions(conditionSpecifics, attacker);
            case "EnergyA":
                return CheckEnergyConditions(conditionSpecifics, attacker);
            case "EnergyD":
                return CheckEnergyConditions(conditionSpecifics, target);
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
            case "WeaponA":
                return conditionSpecifics == attacker.GetWeaponType();
            case "WeaponD":
                return conditionSpecifics == target.GetWeaponType();
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
            case "Direction":
                return CheckDirectionSpecifics(conditionSpecifics, CheckRelativeDirections(target.GetDirection(), attacker.GetDirection()));
            case "Direction<>":
                return !CheckDirectionSpecifics(conditionSpecifics, CheckRelativeDirections(target.GetDirection(), attacker.GetDirection()));
            case "Direction<>D":
                return GetAttackDirectionFromDefenderPOV(attacker.GetDirection(), target.GetDirection()) != int.Parse(conditionSpecifics);
            case "DirectionD":
                return GetAttackDirectionFromDefenderPOV(attacker.GetDirection(), target.GetDirection()) == int.Parse(conditionSpecifics);
            case "ElevationEqualsA":
                return map.ReturnElevation(attacker.GetLocation()) == map.ReturnElevation(target.GetLocation());
            case "Elevation>A":
                return map.ReturnElevation(attacker.GetLocation()) > map.ReturnElevation(target.GetLocation());
            case "Elevation<A":
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
            case "TargetD":
                return target.GetTarget() == attacker;
            case "TargetD<>":
                return target.GetTarget() != attacker;
            case "CounterAttack":
                return target.CounterAttackAvailable();
            case "AllyCount<A":
                return map.AllAllies(attacker).Count < int.Parse(conditionSpecifics);
            case "AllyCount>A":
                return map.AllAllies(attacker).Count > int.Parse(conditionSpecifics);
            case "Ally<EnemyA":
                return map.AllAllies(attacker).Count < map.AllEnemies(attacker).Count;
            case "Ally>EnemyA":
                return map.AllAllies(attacker).Count > map.AllEnemies(attacker).Count;
            case "AllyEqualsEnemyA":
                return map.AllAllies(attacker).Count == map.AllEnemies(attacker).Count;
            case "EnemyCount<A":
                return map.AllEnemies(attacker).Count < int.Parse(conditionSpecifics);
            case "EnemyCount>A":
                return map.AllEnemies(attacker).Count > int.Parse(conditionSpecifics);
            case "AllyCount<D":
                return map.AllAllies(target).Count < int.Parse(conditionSpecifics);
            case "AllyCount>D":
                return map.AllAllies(target).Count > int.Parse(conditionSpecifics);
            case "EnemyCount<D":
                return map.AllEnemies(target).Count < int.Parse(conditionSpecifics);
            case "EnemyCount>D":
                return map.AllEnemies(target).Count > int.Parse(conditionSpecifics);
            case "AverageHP>A":
                return attacker.GetHealth() > map.AverageActorHealth();
            case "AverageHP<A":
                return attacker.GetHealth() < map.AverageActorHealth();
            case "AverageHP>D":
                return target.GetHealth() > map.AverageActorHealth();
            case "AverageHP<D":
                return target.GetHealth() < map.AverageActorHealth();
            case "GrapplingA":
                return attacker.Grappling();
            case "GrappledA":
                return attacker.Grappled();
            case "GrapplingD":
                return target.Grappling();
            case "GrappledD":
                return target.Grappled();
            // Bad RNG means you want to roll higher to avoid the chance.
            case "BadRNGA":
                return utility.Roll(attacker.GetLuck()) < int.Parse(conditionSpecifics);
            case "BadRNGD":
                return utility.Roll(target.GetLuck()) < int.Parse(conditionSpecifics);
            // Good RNG means you want to roll lower to get the chance.
            case "GoodRNGA":
                return utility.Roll(-attacker.GetLuck()) < int.Parse(conditionSpecifics);
            case "GoodRNGD":
                return utility.Roll(-target.GetLuck()) < int.Parse(conditionSpecifics);
            case "HurtByA":
                return attacker.WasHurtByActor(target);
            case "HurtBy<>A":
                return !attacker.WasHurtByActor(target);
            case "HurtMostA":
                return attacker.GetHurtBy() == target;
            case "HurtLeastA":
                return attacker.GetHurtBy(false) == target;
            case "HurtByD":
                return target.WasHurtByActor(attacker);
            case "HurtBy<>D":
                return !target.WasHurtByActor(attacker);
            case "HurtMostD":
                return target.GetHurtBy() == attacker;
            case "HurtLeastD":
                return target.GetHurtBy(false) == attacker;
        }
        return true;
    }

    public bool CheckEnergyConditions(string specifics, TacticActor actor)
    {
        int max = actor.GetBaseEnergy();
        int current = actor.GetEnergy();
        switch (specifics)
        {
            case "0":
                return current == 0;
            case "1":
                return current == 1;
            case "<25%":
                return (current * 4) < max;
            case "<Half":
                return (current * 2) < max;
            case ">Half":
                return (current * 2) > max;
            case "Full":
                return current >= max;
        }
        return false;
    }

    public bool CheckHealthConditions(string specifics, TacticActor actor)
    {
        int max = actor.GetBaseHealth();
        int current = actor.GetHealth();
        switch (specifics)
        {
            case "1":
                if (actor.GetHealth() == 1){return true;}
                return (current * 100) < max;
            case "<25%":
                return (current * 4) < max;
            case "<Half":
                return (current * 2) < max;
            case ">Half":
                return (current * 2) > max;
            case "Full":
                return current >= max;
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

    // Side Conditions Have Been Tested In MISCTESTER
    public bool CheckDirectionSpecifics(string conditionSpecifics, string specifics)
    {
        if (conditionSpecifics == "Back" && specifics == "Same") { return true; }
        else if (conditionSpecifics == "Front" && specifics == "Opposite") { return true; }
        else if (conditionSpecifics == "Side")
        {
            if (specifics != "Opposite" && specifics != "Same")
            {
                return true;
            }
        }
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
                affected += Mathf.Max(1, power * affected / basicDenominator);
                break;
            case "Decrease%":
                affected -= Mathf.Max(1, power * affected / basicDenominator);
                break;
        }
        return affected;
    }
    // Other passive-like things can borrow the passive to apply their effects
    // For example an aura can borrow this so all the passive conditions are in 1 place.
    protected bool CheckAuraCondition(AuraEffect aura, TacticActor actor, BattleMap map)
    {
        // Check if already triggered.
        if (aura.AlreadyTriggered(actor)){return false;}
        // Check if the aura team.
        if (!aura.TeamCheck(actor)){return false;}
        // Check the aura tiles.
        if (!aura.ActorInAura(actor, map)){return false;}
        // Check the aura conditions.
        if (!CheckStartEndCondition(aura.condition, aura.conditionSpecifics, actor, map)){return false;}
        return true;
    }
    protected void TriggerAuraEffect(AuraEffect aura, TacticActor actor, BattleMap map)
    {
        if (!aura.TriggerAura()){return;}
        if (CheckAuraCondition(aura, actor, map))
        {
            map.combatLog.UpdateNewestLog(actor.GetPersonalName() + " is affected by " + aura.GetAuraName() + ".");
            map.combatLog.AddDetailedLogs(map.detailViewer.ReturnAuraDetails(aura));
            AffectActor(actor, aura.effect, aura.effectSpecifics);
            aura.ActorTriggersAura(actor);
        }
    }
    protected void TriggerAuraEffects(List<AuraEffect> allAura, TacticActor actor, BattleMap map)
    {
        for (int i = 0; i < allAura.Count; i++)
        {
            TriggerAuraEffect(allAura[i], actor, map);
        }
    }
    public void TriggerAllAuraEffects(List<AuraEffect> allAura, List<TacticActor> actors, BattleMap map)
    {
        for (int i = 0; i < actors.Count; i++)
        {
            TriggerAuraEffects(allAura, actors[i], map);
        }
    }
}
