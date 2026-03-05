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

    public int GetScalingSpecifics(TacticActor actor, string specifics)
    {
        if (specifics.EndsWith("A") || specifics.EndsWith("D"))
        {
            specifics = specifics[..^1];
        }
        switch (specifics)
        {
            default:
            return 0;
            case "Defense":
                return actor.GetDefense();
            case "Attack":
                return actor.GetAttack();
            case "Attack/2":
                return (actor.GetAttack() / 2);
            case "SkillsUsed":
            return actor.ReturnTotalRoundSkills();
            case "Attacks":
            return actor.ReturnTotalRoundAttacks();
            case "Defends":
            return actor.ReturnTotalRoundDefends();
            case "Moves":
            return actor.ReturnTotalRoundMoves();
        }
    }

    public void AffectMap(TacticActor actor, string effect, string specifics, BattleMap map)
    {
        map.ChangeTile(actor.GetLocation(), effect, specifics);
    }

    public void ApplyAdjustCostPassive(TacticActor actor, ActiveSkill active, BattleMap map, string passiveData)
    {
        string[] data = passiveData.Split("|");
        if (data.Length < 6){return;}
        // Check the conditions.
        string[] conditions = data[1].Split(",");
        string[] cSpecifics = data[2].Split(",");
        for (int i = 0; i < conditions.Length; i++)
        {
            if (!CheckAdjustCostCondition(actor, active, map, conditions[i], cSpecifics[i])){return;}
        }
        // Apply the effects.
        string target = data[3];
        string effect = data[4];
        int change = int.Parse(data[5]);
        switch (effect)
        {
            case "ActionCost":
                active.AdjustFlatActionCost(change);
                break;
            case "ActionCost%":
                active.AdjustPercentActionCost(change);
                break;
            case "EnergyCost":
                active.AdjustFlatEnergyCost(change);
                break;
            case "EnergyCost%":
                active.AdjustPercentEnergyCost(change);
                break;
            case "OverrideA":
                active.SetActionCostOverride(change);
                break;
            case "OverrideE":
                active.SetEnergyCostOverride(change);
                break;
        }
    }

    protected bool CheckAdjustCostCondition(TacticActor actor, ActiveSkill active, BattleMap map, string condition, string specifics)
    {
        // First check if the condition is related to the active.
        switch (condition)
        {
            case "SkillName":
            return active.GetSkillName().Contains(specifics);
            case "SkillType":
            return active.GetSkillType() == specifics;
            case "SkillEffect":
            return active.GetEffect().Contains(specifics);
            case "SkillRange>":
            return active.GetRange(actor) > int.Parse(specifics);
            case "SkillRange<":
            return active.GetRange(actor) < int.Parse(specifics);
            case "SkillRangeShape":
            return active.GetRangeShape() == specifics;
            case "SkillSpan>":
            return active.GetSpan(actor) > int.Parse(specifics);
            case "SkillSpan<":
            return active.GetSpan(actor) < int.Parse(specifics);
            case "SkillShape":
            return active.GetShape() == specifics;
        }
        // Else check if the condition is related to the active user.
        return CheckStartEndCondition(condition, specifics, actor, map);
    }

    public void ApplyAfterAttackPassives(TacticActor actor, TacticActor attackTarget, int damage, BattleMap map, bool hit = true, bool crit = false, bool counterAttack = false)
    {
        List<string> passives = actor.GetAfterAttackPassives();
        if (passives.Count <= 0) { return; }
        for (int i = 0; i < passives.Count; i++)
        {
            string[] passiveData = passives[i].Split("|");
            if (passiveData.Length <= 4) { continue; }
            string[] conditions = passiveData[1].Split(",");
            string[] specifics = passiveData[2].Split(",");
            bool conditionsMet = true;
            for (int j = 0; j < conditions.Length; j++)
            {
                conditionsMet = CheckAfterAttackCondition(conditions[j], specifics[j], actor, attackTarget, map, hit, crit, counterAttack);
                if (!conditionsMet)
                {
                    break;
                }
            }
            if (!conditionsMet)
            {
                continue;
            }
            // For now only affect the actor, later might after other things.
            string[] effects = passiveData[4].Split(",");
            string[] effectSpecifics = passiveData[5].Split(",");
            for (int h = 0; h < effects.Length; h++)
            {
                AffectActor(actor, effects[h], GetEffectSpecifics(actor, effectSpecifics[h]));
            }
        }
    }

    public void ApplyAfterDefendPassives(TacticActor actor, TacticActor attackTarget, int damage, BattleMap map, bool hit = true, bool crit = false, bool counterAttack = false)
    {
        List<string> passives = attackTarget.GetAfterDefendPassives();
        if (passives.Count <= 0) { return; }
        for (int i = 0; i < passives.Count; i++)
        {
            string[] passiveData = passives[i].Split("|");
            if (passiveData.Length <= 4) { continue; }
            string[] conditions = passiveData[1].Split(",");
            string[] specifics = passiveData[2].Split(",");
            bool conditionsMet = true;
            for (int j = 0; j < conditions.Length; j++)
            {
                conditionsMet = CheckAfterAttackCondition(conditions[j], specifics[j], actor, attackTarget, map, hit, crit, counterAttack);
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
            for (int h = 0; h < effects.Length; h++)
            {
                AffectActor(attackTarget, effects[h], GetEffectSpecifics(attackTarget, effectSpecifics[h]));
            }
        }
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

    public bool CheckAfterAttackCondition(string condition, string specifics, TacticActor attacker, TacticActor target, BattleMap map, bool attackHit, bool attackCrit, bool counterAttack)
    {
        switch (condition)
        {
            case "LethalAttack":
                return target.GetHealth() <= 0;
            case "CriticalAttack":
                return attackCrit;
            case "DodgedAttack":
                return !attackHit;
            case "CounterAttack":
                return counterAttack;
        }
        // Large overlap with battle conditions.
        return CheckBattleCondition(condition, specifics, target, attacker, map);
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
            case "RawElevation<":
                return map.ReturnElevation(currentTile) <= int.Parse(specifics);
            case "RawElevation>":
                return map.ReturnElevation(currentTile) >= int.Parse(specifics);
            case "RawElevation":
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
            case "Time<>":
                return specifics != battleState.GetTime();
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
            case "Time<>":
                return conditionSpecifics != map.GetTime();
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
            case "AdjacentAllyCount<":
                return map.GetAdjacentAllies(actor).Count < int.Parse(conditionSpecifics);
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
            case "RawElevation<":
            return map.ReturnElevation(actor.GetLocation()) <= int.Parse(conditionSpecifics);
            case "RawElevation>":
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
            case "FirstStrike":
                return actor.ReturnTotalRoundAttacks() <= 0;
            case "Moved":
                return actor.ReturnCurrentRoundMoves() > 0;
            case "Moved<>":
                return actor.ReturnCurrentRoundMoves() == 0;
            case "SkillUsed":
                return actor.ReturnCurrentRoundSkills() > 0;
            case "SkillUsed<>":
                return actor.ReturnCurrentRoundSkills() == 0;
            case "Attacked":
                return actor.ReturnCurrentRoundAttacks() > 0;
            case "Attacked<>":
                return actor.ReturnCurrentRoundAttacks() == 0;
            case "Defended":
                return actor.ReturnCurrentRoundDefends() > 0;
            case "Defended<>":
                return actor.ReturnCurrentRoundDefends() == 0;
            case "PrevDefended":
                return actor.ReturnPreviousRoundDefends() > 0;
            case "PrevDefended<>":
                return actor.ReturnPreviousRoundDefends() == 0;
            case "PrevMoved":
                return actor.ReturnPreviousRoundMoves() > 0;
            case "PrevMoved<>":
                return actor.ReturnPreviousRoundMoves() == 0;
            case "PrevSkillUsed":
                return actor.ReturnPreviousRoundSkills() > 0;
            case "PrevSkillUsed<>":
                return actor.ReturnPreviousRoundSkills() == 0;
            case "PrevAttacked":
                return actor.ReturnPreviousRoundAttacks() > 0;
            case "PrevAttacked<>":
                return actor.ReturnPreviousRoundAttacks() == 0;
            case "AttackCount":
                return actor.ReturnCurrentRoundAttacks() == int.Parse(conditionSpecifics);
            case "AttackCount>":
                return actor.ReturnCurrentRoundAttacks() > int.Parse(conditionSpecifics);
            case "AttackCount<":
                return actor.ReturnCurrentRoundAttacks() < int.Parse(conditionSpecifics);
            case "AttackCount%":
                return (actor.ReturnCurrentRoundAttacks() % int.Parse(conditionSpecifics) == 0);
            case "PrevAttackCount":
                return actor.ReturnPreviousRoundAttacks() == int.Parse(conditionSpecifics);
            case "PrevAttackCount>":
                return actor.ReturnPreviousRoundAttacks() > int.Parse(conditionSpecifics);
            case "PrevAttackCount<":
                return actor.ReturnPreviousRoundAttacks() < int.Parse(conditionSpecifics);
            case "PrevAttackCount%":
                return (actor.ReturnPreviousRoundAttacks() % int.Parse(conditionSpecifics) == 0);
            case "PrevSkillCount":
                return actor.ReturnPreviousRoundSkills() == int.Parse(conditionSpecifics);
            case "PrevSkillCount>":
                return actor.ReturnPreviousRoundSkills() > int.Parse(conditionSpecifics);
            case "PrevSkillCount<":
                return actor.ReturnPreviousRoundSkills() < int.Parse(conditionSpecifics);
            case "PrevSkillCount%":
                return (actor.ReturnPreviousRoundSkills() % int.Parse(conditionSpecifics) == 0);
            case "PrevMoveCount":
                return actor.ReturnPreviousRoundMoves() == int.Parse(conditionSpecifics);
            case "PrevMoveCount>":
                return actor.ReturnPreviousRoundMoves() > int.Parse(conditionSpecifics);
            case "PrevMoveCount<":
                return actor.ReturnPreviousRoundMoves() < int.Parse(conditionSpecifics);
            case "PrevMoveCount%":
                return (actor.ReturnPreviousRoundMoves() % int.Parse(conditionSpecifics) == 0);
            case "DefendCount":
                return actor.ReturnCurrentRoundDefends() == int.Parse(conditionSpecifics);
            case "DefendCount>":
                return actor.ReturnCurrentRoundDefends() > int.Parse(conditionSpecifics);
            case "DefendCount<":
                return actor.ReturnCurrentRoundDefends() < int.Parse(conditionSpecifics);
            case "DefendCount%":
                return (actor.ReturnCurrentRoundDefends() % int.Parse(conditionSpecifics) == 0);
            case "PrevDefendCount":
                return actor.ReturnPreviousRoundDefends() == int.Parse(conditionSpecifics);
            case "PrevDefendCount>":
                return actor.ReturnPreviousRoundDefends() > int.Parse(conditionSpecifics);
            case "PrevDefendCount<":
                return actor.ReturnPreviousRoundDefends() < int.Parse(conditionSpecifics);
            case "PrevDefendCount%":
                return (actor.ReturnPreviousRoundDefends() % int.Parse(conditionSpecifics) == 0);
        }
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
        if (condition == ""){return true;}
        TacticActor checkedActor = target;
        TacticActor comparedActor = attacker;
        if (condition.EndsWith("A"))
        {
            checkedActor = attacker;
            comparedActor = target;
            condition = condition[..^1];
        }
        else if (condition.EndsWith("D"))
        {
            checkedActor = target;
            comparedActor = attacker;
            condition = condition[..^1];
        }
        switch (condition)
        {
            case "AdjacentEnemyCount>":
                return map.GetAdjacentEnemies(checkedActor).Count > int.Parse(conditionSpecifics);
            case "AdjacentEnemyCount<":
                return map.GetAdjacentEnemies(checkedActor).Count < int.Parse(conditionSpecifics);
            case "Distance":
                return map.DistanceBetweenActors(checkedActor, comparedActor) <= int.Parse(conditionSpecifics);
            case "Distance>":
                return map.DistanceBetweenActors(checkedActor, comparedActor) > int.Parse(conditionSpecifics);
            case "PassiveLevels>":
                return checkedActor.GetTotalPassiveLevels() > int.Parse(conditionSpecifics);
            case "PassiveLevels<":
                return checkedActor.GetTotalPassiveLevels() < int.Parse(conditionSpecifics);
            case "Team":
                if (conditionSpecifics == "Same")
                {
                    return checkedActor.GetTeam() == comparedActor.GetTeam();
                }
                return checkedActor.GetTeam() != comparedActor.GetTeam();
            case "Direction":
                return CheckDirectionSpecifics(conditionSpecifics, CheckRelativeDirections(comparedActor.GetDirection(), checkedActor.GetDirection()));
            case "Direction<>":
                return !CheckDirectionSpecifics(conditionSpecifics, CheckRelativeDirections(comparedActor.GetDirection(), checkedActor.GetDirection()));
            case "IntDirection<>":
                return GetAttackDirectionFromDefenderPOV(comparedActor.GetDirection(), checkedActor.GetDirection()) != int.Parse(conditionSpecifics);
            case "IntDirection":
                return GetAttackDirectionFromDefenderPOV(comparedActor.GetDirection(), checkedActor.GetDirection()) == int.Parse(conditionSpecifics);
            case "ElevationEquals":
                return map.ReturnElevation(checkedActor.GetLocation()) == map.ReturnElevation(comparedActor.GetLocation());
            case "Elevation>":
                return map.ReturnElevation(checkedActor.GetLocation()) > map.ReturnElevation(comparedActor.GetLocation());
            case "Elevation<":
                return map.ReturnElevation(checkedActor.GetLocation()) < map.ReturnElevation(comparedActor.GetLocation());
            case "Species":
                return checkedActor.GetSpecies() == conditionSpecifics;
            case "Species<>":
                return checkedActor.GetSpecies() != conditionSpecifics;
            case "Target":
                return checkedActor.GetTarget() == comparedActor;
            case "Target<>":
                return checkedActor.GetTarget() != comparedActor;
            case "HurtBy":
                return checkedActor.WasHurtByActor(comparedActor);
            case "HurtBy<>":
                return !checkedActor.WasHurtByActor(comparedActor);
            case "HurtMost":
                return checkedActor.GetHurtBy() == comparedActor;
            case "HurtLeast":
                return checkedActor.GetHurtBy(false) == comparedActor;
        }
        return CheckStartEndCondition(condition, conditionSpecifics, checkedActor, map);
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

    protected int ScalingAffectInt(int affected, string effect, string scalingSpecifics, TacticActor scalingActor)
    {
        string[] scalingBasedOn = scalingSpecifics.Split("Equals");
        if (scalingBasedOn.Length < 2){return affected;}
        int power = GetScalingSpecifics(scalingActor, scalingBasedOn[1]);
        return AffectInt(affected, effect, power.ToString());
    }

    // Some effect scale based on the actors.
    public int AffectInt(int affected, string effect, string effectSpecifics, TacticActor actor = null, TacticActor target = null)
    {
        TacticActor scalingActor = actor;
        int power = 0;
        if (effectSpecifics.Contains("Scaling") && scalingActor != null)
        {
            if (effectSpecifics.EndsWith("D") && target != null)
            {
                scalingActor = target;
            }
            return ScalingAffectInt(affected, effect, effectSpecifics, scalingActor);
        }
        bool intEffect = int.TryParse(effectSpecifics, out power);
        if (!intEffect){return affected;}
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
    public bool CheckAuraCondition(AuraEffect aura, TacticActor actor, BattleMap map)
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
}
