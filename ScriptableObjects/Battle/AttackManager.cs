using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackManager", menuName = "ScriptableObjects/BattleLogic/AttackManager", order = 1)]
public class AttackManager : ScriptableObject
{
    public PassiveSkill passive;
    public StatDatabase passiveData;
    public TerrainPassivesList terrainPassives;
    public TerrainPassivesList tEffectPassives;
    public TerrainPassivesList weatherPassives;
    public TerrainPassivesList borderPassives;
    public int stabMultiplier = 150;
    public int baseMultiplier;
    protected string damageRolls;
    protected string passiveEffectString;
    protected string finalDamageCalculation;
    // Based on attacker/defender.
    protected int advantage;
    protected int baseDamage;
    protected int damageMultiplier;
    // Based on defender.
    protected int dodgeChance;
    protected int defenseValue;
    // Based on attacker.
    protected int attackValue;
    protected int hitChance;
    protected int critDamage;
    protected int critChance;

    public bool GuardActive(TacticActor defender, BattleMap map)
    {
        return map.GetAdjacentGuardingAlly(defender) != null;
    }

    public TacticActor GetGuard(TacticActor defender, BattleMap map)
    {
        TacticActor guard = map.GetAdjacentGuardingAlly(defender);
        if (guard == null){return defender;}
        return guard;
    }

    public bool RollToHit(TacticActor attacker, TacticActor defender, BattleMap map, bool showLog = true)
    {
        int hitRoll = Random.Range(0, 100);
        if (hitRoll >= (hitChance - dodgeChance))
        {
            if (showLog)
            {
                map.combatLog.UpdateNewestLog("The attack misses!");
                map.combatLog.AddDetailedLogs(passiveEffectString);
            }
            return false;
        }
        return true;
    }
    public int STAB(TacticActor attacker, int damage, string type, bool showLog = true)
    {
        if (attacker.SameElement(type))
        {
            if (showLog)
            {
                finalDamageCalculation += "STAB: " + damage + " * " + stabMultiplier + "% = ";
            }
            damage = damage * stabMultiplier / baseMultiplier;
            if (showLog)
            {
                finalDamageCalculation += damage + "\n";
            }
        }
        return damage;
    }
    public int ElementalMastery(TacticActor attacker, int damage, string type, bool showLog = true)
    {
        // Check for bonus damage.
        int elementBonus = attacker.ReturnDamageBonusOfType(type);
        if (elementBonus != 0)
        {
            if (showLog)
            {
                finalDamageCalculation += type + " Mastery = +" + elementBonus + "% Damage";
                finalDamageCalculation += "\n" + damage + " * " + (100 + elementBonus) + "% = " + (damage * (100 + elementBonus) / 100) + "\n";
            }
            return damage * (100 + elementBonus) / 100;
        }
        return damage;
    }
    public void ElementalResistance(TacticActor defender, int damage, string type, bool showLog = true)
    {
        int resistance = defender.ReturnDamageResistanceOfType(type);
        if (resistance != 0)
        {
            if (showLog)
            {
                finalDamageCalculation += "\n" + type + " Resistance = " + resistance + "%";
                finalDamageCalculation += "\n" + baseDamage + " * " + (100 - resistance) + "% = " + (baseDamage * (100 - resistance) / 100);
            }
        }
    }
    public int CritRoll(TacticActor attacker, int damage, bool showLog = true)
    {
        int critRoll = Random.Range(0, 100);
        if (critRoll < critChance)
        {
            if (showLog)
            {
                finalDamageCalculation += "CRITICAL HIT: " + damage + " * " + critDamage + "% = ";
            }
            damage = damage * critDamage / baseMultiplier;
            if (showLog)
            {
                finalDamageCalculation += damage + "\n";
            }
        }
        return damage;
    }
    // Simplified since it's just bonus damage.
    protected void ElementalBonusDamage(TacticActor dealer, TacticActor receiver, int damage, string element, BattleMap map = null)
    {
        damage = STAB(dealer, damage, element, false);
        damage = ElementalMastery(dealer, damage, element, false);
        ElementalResistance(receiver, damage, element, false);
        damage = receiver.TakeDamage(damage, element);
        if (map != null)
        {
            map.combatLog.UpdateNewestLog(receiver.GetPersonalName() + " takes " + damage + " " + element + " damage.");
        }
    }
    // Used for most spell effects.
    public void ElementalFlatDamage(TacticActor attacker, TacticActor defender, BattleMap map, int damage, string element)
    {
        attacker.SetDirection(map.DirectionBetweenActors(attacker, defender));
        if (!RollToHit(attacker, defender, map)){return;}
        baseDamage = damage;
        finalDamageCalculation = "";
        // Stab
        baseDamage = STAB(attacker, baseDamage, element);
        // Elemental Mastery
        baseDamage = ElementalMastery(attacker, baseDamage, element);
        // Crit
        baseDamage = CritRoll(attacker, baseDamage);
        // Resistance
        ElementalResistance(defender, baseDamage, element);
        baseDamage = defender.TakeDamage(baseDamage, element);
        defender.SetTarget(attacker);
        map.combatLog.UpdateNewestLog(defender.GetPersonalName() + " takes " + baseDamage + " damage.");
        map.damageTracker.UpdateDamageStat(attacker, defender, baseDamage);
        map.combatLog.AddDetailedLogs(finalDamageCalculation);
    }
    protected void CheckMapPassives(TacticActor attacker, TacticActor defender, BattleMap map, bool forAttacker = true, bool forDefender = true)
    {
        CheckAuraEffects(defender, attacker, map);
        CheckTerrainPassives(defender, attacker, map, forAttacker, forDefender);
        CheckTEffectPassives(defender, attacker, map, forAttacker, forDefender);
        CheckBorderPassives(defender, attacker, map, forAttacker, forDefender);
        CheckWeatherPassives(defender, attacker, map, forAttacker, forDefender);
    }
    // Basically used for guns/cannons other non scaling damage
    public void FlatDamageAttack(TacticActor attacker, TacticActor defender, BattleMap map, int damage)
    {
        attacker.SetDirection(map.DirectionBetweenActors(attacker, defender));
        UpdateBattleStats(attacker, defender);
        advantage = 0;
        damageMultiplier = baseMultiplier;
        baseDamage = damage;
        damageRolls = "Damage Rolls: ";
        passiveEffectString = "Applied Passives: ";
        finalDamageCalculation = "";
        // Only the defender gets passive bonuses(?) from tile/teffects
        CheckMapPassives(attacker, defender, map, false, true);
        CheckPassives(defender.GetDefendingPassives(), defender, attacker, map);
        if (!RollToHit(attacker, defender, map)){return;}
        baseDamage = Advantage(baseDamage, advantage);
        finalDamageCalculation += "Subtract Defense: " + baseDamage + " - " + defender.GetDefense() + " = ";
        baseDamage = Mathf.Max(0, baseDamage - defender.GetDefense());
        finalDamageCalculation += baseDamage;
        if (damageMultiplier < 0) { damageMultiplier = 0; }
        finalDamageCalculation += "\n" + "Damage Multiplier: " + baseDamage + " * " + damageMultiplier + "% = ";
        baseDamage = damageMultiplier * baseDamage / baseMultiplier;
        finalDamageCalculation += baseDamage;
        // Flat damage is always physical type.
        defender.TakeDamage(baseDamage);
        defender.SetTarget(attacker);
        map.combatLog.UpdateNewestLog(defender.GetPersonalName() + " takes " + baseDamage + " damage.");
        map.combatLog.AddDetailedLogs(passiveEffectString);
        map.combatLog.AddDetailedLogs(damageRolls);
        map.combatLog.AddDetailedLogs(finalDamageCalculation);
        map.damageTracker.UpdateDamageStat(attacker, defender, baseDamage);
    }
    public void TrueDamageAttack(TacticActor attacker, TacticActor defender, BattleMap map, int attackMultiplier = -1, string type = "Attack")
    {
        attacker.SetDirection(map.DirectionBetweenActors(attacker, defender));
        UpdateBattleStats(attacker, defender);
        baseDamage = attackValue;
        if (attackMultiplier < 0) { damageMultiplier = baseMultiplier; }
        else { damageMultiplier = attackMultiplier; }
        // True damage ignores defender/terrain passives, making it even stronger than it should be.
        CheckMapPassives(attacker, defender, map, true, false);
        CheckPassives(attacker.GetAttackingPassives(), defender, attacker, map);
        switch (type)
        {
            case "Health":
                baseDamage = attacker.GetHealth();
                break;
            case "Defense":
                baseDamage = attacker.GetDefense();
                break;
        }
        baseDamage = damageMultiplier * baseDamage / baseMultiplier;
        int critRoll = Random.Range(0, 100);
        if (critRoll < attacker.GetCritChance())
        {
            baseDamage = baseDamage * attacker.GetCritDamage() / baseMultiplier;
        }
        defender.SetTarget(attacker);
        map.combatLog.UpdateNewestLog(defender.GetPersonalName() + " takes " + baseDamage + " damage.");
        map.damageTracker.UpdateDamageStat(attacker, defender, baseDamage);
        baseDamage = defender.TakeDamage(baseDamage, "True");
    }
    protected void UpdateBattleStats(TacticActor attacker, TacticActor defender)
    {
        dodgeChance = defender.GetDodgeChance();
        defenseValue = defender.GetDefense();
        attackValue = attacker.GetAttack();
        hitChance = attacker.GetHitChance();
        critChance = attacker.GetCritChance();
        critDamage = attacker.GetCritDamage();
    }
    // Basic attack damage calculation
    public void ActorAttacksActor(TacticActor attacker, TacticActor target, BattleMap map, int attackMultiplier = -1, string type = "Physical")
    {
        bool guard = GuardActive(target, map);
        attacker.SetDirection(map.DirectionBetweenActors(attacker, target));
        attacker.UpdateTempInitiative(-1);
        TacticActor attackTarget = target;
        // Check if there is a guard.
        if (guard)
        {
            attackTarget = GetGuard(target, map);
            map.combatLog.UpdateNewestLog(attackTarget.GetPersonalName() + " defends " + target.GetPersonalName() + " from the attack.");
            // Switch the actor locations?
        }
        advantage = 0;
        if (attackMultiplier < 0) { damageMultiplier = baseMultiplier; }
        else { damageMultiplier = attackMultiplier; }
        UpdateBattleStats(attacker, attackTarget);
        baseDamage = attackValue;
        damageRolls = "Damage Rolls: ";
        passiveEffectString = "Applied Passives: ";
        finalDamageCalculation = "";
        CheckMapPassives(attacker, attackTarget, map, true, true);
        // Bonus damage can be calculated here and triggers regardless of hit/miss.
        CheckPassives(attacker.GetAttackingPassives(), attackTarget, attacker, map);
        CheckPassives(attackTarget.GetDefendingPassives(), attackTarget, attacker, map);
        // Determine if you miss or not.
        if (!RollToHit(attacker, attackTarget, map)){return;}
        baseDamage = Advantage(baseDamage, advantage);
        // Check for stab.
        baseDamage = STAB(attacker, baseDamage, type);
        // Check for bonus damage.
        baseDamage = ElementalMastery(attacker, baseDamage, type);
        // Check for a critical hit.
        baseDamage = CritRoll(attacker, baseDamage);
        // First subtract defense.
        finalDamageCalculation += "Subtract Defense: " + baseDamage + " - " + defenseValue + " = ";
        baseDamage = baseDamage - defenseValue;
        if (baseDamage < 0){ baseDamage = 0; }
        finalDamageCalculation += baseDamage;
        // Then multiply by damage multiplier.
        if (damageMultiplier < 0) { damageMultiplier = 0; }
        finalDamageCalculation += "\n" + "Damage Multiplier: " + baseDamage + " * " + damageMultiplier + "% = ";
        baseDamage = damageMultiplier * baseDamage / baseMultiplier;
        // Check if the passive affects damage.
        baseDamage = CheckTakeDamagePassives(attackTarget.GetTakeDamagePassives(), baseDamage, type);
        finalDamageCalculation += baseDamage;
        // Show the resistance calculation.
        ElementalResistance(attackTarget, baseDamage, type);
        baseDamage = attackTarget.TakeDamage(baseDamage, type);
        attackTarget.SetTarget(attacker);
        map.combatLog.UpdateNewestLog(attackTarget.GetPersonalName() + " takes " + baseDamage + " damage.");
        map.damageTracker.UpdateDamageStat(attacker, attackTarget, baseDamage);
        map.combatLog.AddDetailedLogs(passiveEffectString);
        map.combatLog.AddDetailedLogs(damageRolls);
        map.combatLog.AddDetailedLogs(finalDamageCalculation);
        // Check if the defender is alive, has counter attacks available and is in range.
        if (attackTarget.GetHealth() > 0 && attackTarget.CounterAttackAvailable() && map.DistanceBetweenActors(attackTarget, attacker) <= attackTarget.GetAttackRange())
        {
            attackTarget.UseCounterAttack();
            map.combatLog.UpdateNewestLog(attackTarget.GetPersonalName() + " counter attacks " + attacker.GetPersonalName());
            ActorAttacksActor(attackTarget, attacker, map);
        }
    }

    protected int RollAttackDamage(int baseAttack)
    {
        int roll = baseAttack + Random.Range(-baseAttack/3, baseAttack/3);
        damageRolls += " "+roll+" ";
        return roll;
    }

    protected int Advantage(int baseAttack, int advantage)
    {
        if (advantage < 0)
        {
            return Disadvantage(baseAttack, Mathf.Abs(advantage));
        }
        if (advantage == 0)
        {
            return RollAttackDamage(baseAttack);
        }
        damageRolls += "Max(";
        int damage = RollAttackDamage(baseAttack);
        for (int i = 0; i < advantage; i++)
        {
            damage = Mathf.Max(damage, RollAttackDamage(baseAttack));
        }
        damageRolls += ") + "+advantage.ToString();
        return damage + advantage;
    }

    protected int Disadvantage(int baseAttack, int disadvantage)
    {
        damageRolls += "Min(";
        int damage = RollAttackDamage(baseAttack);
        for (int i = 0; i < disadvantage; i++)
        {
            damage = Mathf.Min(damage, RollAttackDamage(baseAttack));
        }
        damageRolls += ") - "+disadvantage.ToString();
        return damage - disadvantage;
    }

    protected void CheckPassives(List<string> characterPassives, TacticActor target, TacticActor attacker, BattleMap map)
    {
        for (int i = 0; i < characterPassives.Count; i++)
        {
            ApplyPassiveEffect(characterPassives[i], target, attacker, map);
        }
    }

    protected void ActivatePassiveEffect(string passiveName, List<string> passiveStats, TacticActor target, TacticActor attacker, BattleMap map)
    {
        switch (passiveStats[3])
        {
            case "Advantage":
            passiveEffectString += "\n";
            passiveEffectString += passiveName + "; " + passiveStats[3] + ":" + advantage+"->";
            advantage = passive.AffectInt(advantage, passiveStats[4], passiveStats[5]);
            passiveEffectString += advantage;
            break;
            case "Damage%":
            passiveEffectString += "\n";
            passiveEffectString += passiveName + "; " + passiveStats[3] + ":" + damageMultiplier+"->";
            damageMultiplier = passive.AffectInt(damageMultiplier, passiveStats[4], passiveStats[5]);
            if (damageMultiplier < 0)
            {
                damageMultiplier = 0;
            }
            passiveEffectString += damageMultiplier;
            break;
            case "AttackValue":
            passiveEffectString += "\n";
            passiveEffectString += passiveName + "; " + passiveStats[3] + ":" + baseDamage+"->";
            baseDamage = passive.AffectInt(baseDamage, passiveStats[4], passiveStats[5]);
            passiveEffectString += baseDamage;
            break;
            case "DefenseValue":
            passiveEffectString += "\n";
            passiveEffectString += passiveName + "; " + passiveStats[3] + ":" + defenseValue + "->";
            defenseValue = passive.AffectInt(defenseValue, passiveStats[4], passiveStats[5]);
            passiveEffectString += defenseValue;
            break;
            case "HitChance":
            passiveEffectString += "\n";
            passiveEffectString += passiveName + "; " + passiveStats[3] + ":" + hitChance+"->";
            hitChance = passive.AffectInt(hitChance, passiveStats[4], passiveStats[5]);
            passiveEffectString += hitChance;
            break;
            case "Dodge":
            passiveEffectString += "\n";
            passiveEffectString += passiveName + "; " + passiveStats[3] + ":" + dodgeChance+"->";
            dodgeChance = passive.AffectInt(dodgeChance, passiveStats[4], passiveStats[5]);
            passiveEffectString += dodgeChance;
            break;
            case "CritChance":
            passiveEffectString += "\n";
            passiveEffectString += passiveName + "; " + passiveStats[3] + ":" + critChance+"->";
            critChance = passive.AffectInt(critChance, passiveStats[4], passiveStats[5]);
            passiveEffectString += critChance;
            break;
            case "CritDamage":
            passiveEffectString += "\n";
            passiveEffectString += passiveName + "; " + passiveStats[3] + ":" + critDamage+"->";
            critDamage = passive.AffectInt(critDamage, passiveStats[4], passiveStats[5]);
            passiveEffectString += critDamage;
            break;
            case "Target":
            passive.AffectActor(target, passiveStats[4], passiveStats[5]);
            break;
            case "Attacker":
            passive.AffectActor(attacker, passiveStats[4], passiveStats[5]);
            break;
            case "Map":
            map.ChangeTile(target.GetLocation(), passiveStats[4], passiveStats[5]);
            break;
            case "ElementalBonusDamage":
            string[] eBD = passiveStats[5].Split(">>");
            ElementalBonusDamage(attacker, target, int.Parse(eBD[1]), eBD[0], map);
            break;
            case "ElementalReflectDamage":
            string[] eRD = passiveStats[5].Split(">>");
            ElementalBonusDamage(target, attacker, int.Parse(eRD[1]), eRD[0], map);
            break;
        }
    }

    protected void ApplyPassiveEffect(string pData, TacticActor target, TacticActor attacker, BattleMap map)
    {
        if (pData.Length < 6 || !pData.Contains("|")){return;}
        List<string> pStats = pData.Split("|").ToList();
        string passiveName = passiveData.ReturnKeyFromValue(pData);
        if (passive.CheckBattleConditions(pStats[1], pStats[2], target, attacker, map))
        {
            ActivatePassiveEffect(passiveName, pStats, target, attacker, map);
        }
    }

    protected void ApplyAuraEffect(AuraEffect aura, TacticActor target, TacticActor attacker, BattleMap map)
    {
        List<string> pStats = aura.ReturnPassiveStats();
        if (passive.CheckBattleConditions(pStats[1], pStats[2], target, attacker, map))
        {
            ActivatePassiveEffect(pStats[0], pStats, target, attacker, map);
        }
    }

    protected int CheckTakeDamagePassives(List<string> passives, int damage, string damageType)
    {
        int originalDamage = damage;
        for (int i = 0; i < passives.Count; i++)
        {
            List<string> passiveStats = passives[i].Split("|").ToList();
            if (passive.CheckTakeDamageCondition(passiveStats[1], passiveStats[2], originalDamage, damageType))
            {
                damage = passive.AffectInt(damage, passiveStats[4], passiveStats[5]);
            }
        }
        return damage;
    }

    protected void CheckWeatherPassives(TacticActor target, TacticActor attacker, BattleMap map, bool forAttacker = true, bool forTarget = true)
    {
        string weather = map.GetWeather();
        string defendingPassive = weatherPassives.ReturnDefendingPassive(weather);
        string attackingPassive = weatherPassives.ReturnAttackingPassive(weather);
        if (defendingPassive.Length > 1 && forTarget)
        {
            ApplyPassiveEffect(defendingPassive, target, attacker, map);
        }
        if (attackingPassive.Length > 1 && forAttacker)
        {
            ApplyPassiveEffect(attackingPassive, target, attacker, map);
        }
    }

    protected void CheckTEffectPassives(TacticActor target, TacticActor attacker, BattleMap map, bool forAttacker = true, bool forTarget = true)
    {
        string targetTile = map.terrainEffectTiles[target.GetLocation()];
        string attackingTile = map.terrainEffectTiles[attacker.GetLocation()];
        string defendingPassive = tEffectPassives.ReturnDefendingPassive(targetTile);
        if (defendingPassive.Length > 1 && forTarget)
        {
            ApplyPassiveEffect(defendingPassive, target, attacker, map);
        }
        string attackingPassive = tEffectPassives.ReturnAttackingPassive(attackingTile);
        if (attackingPassive.Length > 1 && forAttacker)
        {
            ApplyPassiveEffect(attackingPassive, target, attacker, map);
        }
    }

    protected void CheckTerrainPassives(TacticActor target, TacticActor attacker, BattleMap map, bool forAttacker = true, bool forTarget = true)
    {
        string targetTile = map.mapInfo[target.GetLocation()];
        string attackingTile = map.mapInfo[attacker.GetLocation()];
        string defendingPassive = terrainPassives.ReturnDefendingPassive(targetTile);
        if (defendingPassive.Length > 1 && forTarget)
        {
            ApplyPassiveEffect(defendingPassive, target, attacker, map);
        }
        string attackingPassive = terrainPassives.ReturnAttackingPassive(attackingTile);
        if (attackingPassive.Length > 1 && forAttacker)
        {
            ApplyPassiveEffect(attackingPassive, target, attacker, map);
        }
    }

    protected void CheckBorderPassives(TacticActor target, TacticActor attacker, BattleMap map, bool forAttacker = true, bool forTarget = true)
    {
        // Get the direction from the attacker to the direction.
        int attackerToTargetDir = map.DirectionBetweenActors(attacker, target);
        int targetToAttackerDir = (attackerToTargetDir + 3) % 6;
        // Check if there is a border in the attacker direction on the attacker tile.
        if (forAttacker)
        {
            string attackerBorder = map.ReturnBorderFromTileDirection(attacker.GetLocation(), attackerToTargetDir);
            string attackingPassive = borderPassives.ReturnAttackingPassive(attackerBorder);
            ApplyPassiveEffect(attackingPassive, target, attacker, map);
        }
        // Check if the defender has a border.
        if (forTarget)
        {
            string targetBorder = map.ReturnBorderFromTileDirection(target.GetLocation(), targetToAttackerDir);
            string defendingPassive = borderPassives.ReturnDefendingPassive(targetBorder);
            ApplyPassiveEffect(defendingPassive, target, attacker, map);
        }
    }

    // Always check for both.
    protected void CheckAuraEffects(TacticActor target, TacticActor attacker, BattleMap map)
    {
        // Iterate through the auras.
        for (int i = 0; i < map.auras.Count; i++)
        {
            if (map.auras[i].BattleTeamCheck(target, attacker, map))
            {
                ApplyAuraEffect(map.auras[i], target, attacker, map);
            }
        }
    }
}
