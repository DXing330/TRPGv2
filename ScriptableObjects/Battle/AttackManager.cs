using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackManager", menuName = "ScriptableObjects/BattleLogic/AttackManager", order = 1)]
public class AttackManager : ScriptableObject
{
    public PassiveSkill passive;
    public StatDatabase passiveData;
    public TerrainPassivesList terrainPassives;
    public int baseMultiplier;
    protected string damageRolls;
    protected string passiveEffectString;
    protected string finalDamageCalculation;
    protected int advantage;
    protected int baseDamage;
    protected int damageMultiplier;

    public void TrueDamageAttack(TacticActor attacker, TacticActor defender, BattleMap map, MoveCostManager moveManager, int attackMultiplier = -1, string type = "Attack")
    {
        if (attackMultiplier < 0) { damageMultiplier = baseMultiplier; }
        else { damageMultiplier = attackMultiplier; }
        // True damage ignores defender/terrain passives, making it even stronger than it should be.
        CheckPassives(attacker.attackingPassives, defender, attacker, map, moveManager);
        baseDamage = attacker.GetAttack();
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
        defender.SetTarget(attacker);
        map.combatLog.UpdateNewestLog(defender.GetPersonalName() + " takes " + baseDamage + " damage.");
        defender.TakeDamage(baseDamage);
    }
    public void ActorAttacksActor(TacticActor attacker, TacticActor defender, BattleMap map, MoveCostManager moveManager, int attackMultiplier = -1)
    {
        attacker.SetDirection(moveManager.DirectionBetweenActors(attacker, defender));
        advantage = 0;
        damageRolls = "Damage Rolls: ";
        passiveEffectString = "Applied Passives: ";
        if (attackMultiplier < 0) { damageMultiplier = baseMultiplier; }
        else { damageMultiplier = attackMultiplier; }
        // Forests/other cover will reduce ranged damage greatly.
        baseDamage = attacker.GetAttack();
        CheckTerrainPassives(defender, attacker, map, moveManager);
        CheckPassives(attacker.attackingPassives, defender, attacker, map, moveManager);
        CheckPassives(defender.defendingPassives, defender, attacker, map, moveManager);
        baseDamage = Advantage(baseDamage, advantage);
        if (damageMultiplier < 0) { damageMultiplier = 0; }
        finalDamageCalculation = "Damage Multiplier: " + baseDamage + " * " + damageMultiplier + "% = ";
        baseDamage = damageMultiplier * baseDamage / baseMultiplier;
        finalDamageCalculation += baseDamage;
        finalDamageCalculation += "\n" + "Subtract Defense: " + baseDamage + " - " + defender.GetDefense() + " = ";
        // Adjust damage based on passives, terrain effects, direction, etc.
        // Check if the passive affects damage.
        baseDamage = CheckTakeDamagePassives(defender.GetTakeDamagePassives(), baseDamage, "");
        baseDamage = Mathf.Max(0, baseDamage - defender.GetDefense());
        finalDamageCalculation += baseDamage;
        defender.TakeDamage(baseDamage);
        defender.SetTarget(attacker);
        map.combatLog.UpdateNewestLog(defender.GetPersonalName() + " takes " + baseDamage + " damage.");
        map.combatLog.AddDetailedLogs(passiveEffectString);
        map.combatLog.AddDetailedLogs(damageRolls);
        map.combatLog.AddDetailedLogs(finalDamageCalculation);
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

    protected void CheckPassives(List<string> characterPassives, TacticActor target, TacticActor attacker, BattleMap map, MoveCostManager moveManager)
    {
        for (int i = 0; i < characterPassives.Count; i++)
        {
            ApplyPassiveEffect(characterPassives[i], target, attacker, map, moveManager);
        }
    }

    protected void ApplyPassiveEffect(string passiveName, TacticActor target, TacticActor attacker, BattleMap map, MoveCostManager moveManager)
    {
        List<string> passiveStats = passiveData.ReturnStats(passiveName);
        if (passive.CheckBattleCondition(passiveStats[1], passiveStats[2], target, attacker, map, moveManager))
        {
            switch (passiveStats[3])
            {
                case "Advantage":
                passiveEffectString += "\n";
                passiveEffectString += passiveName+";"+passiveStats[3]+":"+advantage+"->";
                advantage = passive.AffectInt(advantage, passiveStats[4], passiveStats[5]);
                passiveEffectString += advantage;
                break;
                case "Damage%":
                passiveEffectString += "\n";
                passiveEffectString += passiveName+";"+passiveStats[3]+":"+damageMultiplier+"->";
                damageMultiplier = passive.AffectInt(damageMultiplier, passiveStats[4], passiveStats[5]);
                passiveEffectString += damageMultiplier;
                break;
                case "BaseDamage":
                passiveEffectString += "\n";
                passiveEffectString += passiveName+";"+passiveStats[3]+":"+baseDamage+"->";
                baseDamage = passive.AffectInt(baseDamage, passiveStats[4], passiveStats[5]);
                passiveEffectString += baseDamage;
                break;
                case "Target":
                passive.AffectActor(target, passiveStats[4], passiveStats[5]);
                break;
                case "Attacker":
                passive.AffectActor(attacker, passiveStats[4], passiveStats[5]);
                break;
            }
        }
    }

    protected int CheckTakeDamagePassives(List<string> passives, int damage, string damageType)
    {
        int originalDamage = damage;
        for (int i = 0; i < passives.Count; i++)
        {
            List<string> passiveStats = passiveData.ReturnStats(passives[i]);
            if (passive.CheckTakeDamageCondition(passiveStats[1], passiveStats[2], originalDamage, damageType))
            {
                damage = passive.AffectInt(damage, passiveStats[4], passiveStats[5]);
            }
        }
        return damage;
    }

    protected void CheckTerrainPassives(TacticActor target, TacticActor attacker, BattleMap map, MoveCostManager moveManager)
    {
        string targetTile = map.mapInfo[target.GetLocation()];
        string attackingTile = map.mapInfo[attacker.GetLocation()];
        if (terrainPassives.TerrainPassivesExist(targetTile))
        {
            string defendingPassive = terrainPassives.ReturnTerrainPassive(targetTile).GetDefendingPassive();
            if (defendingPassive.Length > 1)
            {
                ApplyPassiveEffect(defendingPassive, target, attacker, map, moveManager);
            }
        }
        if (terrainPassives.TerrainPassivesExist(attackingTile))
        {
            string attackingPassive = terrainPassives.ReturnTerrainPassive(attackingTile).GetAttackingPassive();
            if (attackingPassive.Length > 1)
            {
                ApplyPassiveEffect(attackingPassive, target, attacker, map, moveManager);
            }
        }
    }
}
