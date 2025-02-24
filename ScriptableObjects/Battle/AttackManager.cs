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
    protected int advantage;
    protected int baseDamage;
    protected int damageMultiplier;
    public void ActorAttacksActor(TacticActor attacker, TacticActor defender, BattleMap map, MoveCostManager moveManager, int attackMultiplier = -1)
    {
        advantage = 0;
        if (attackMultiplier < 0){damageMultiplier = baseMultiplier;}
        else {damageMultiplier = attackMultiplier;}
        // Forests/other cover will reduce ranged damage greatly.
        baseDamage = attacker.GetAttack();
        CheckTerrainPassives(defender, attacker, map, moveManager);
        CheckPassives(attacker.attackingPassives, defender, attacker, map, moveManager);
        CheckPassives(defender.defendingPassives, defender, attacker, map, moveManager);
        baseDamage = Advantage(baseDamage, advantage);
        baseDamage = damageMultiplier * baseDamage / baseMultiplier;
        // Adjust damage based on passives, terrain effects, direction, etc.
        // Check if the passive affects damage.
        baseDamage = CheckTakeDamagePassives(defender.GetTakeDamagePassives(), baseDamage, "");
        baseDamage = Mathf.Max(0, baseDamage - defender.GetDefense());
        defender.TakeDamage(baseDamage);
        defender.SetTarget(attacker);
        Debug.Log(defender.GetSpriteName()+" takes "+baseDamage+" damage.");
        attacker.SetDirection(moveManager.DirectionBetweenActors(attacker, defender));
    }

    protected int RollAttackDamage(int baseAttack)
    {
        return baseAttack + Random.Range(-baseAttack/3, baseAttack/3);
    }

    protected int Advantage(int baseAttack, int advantage)
    {
        if (advantage < 0){return Disadvantage(baseAttack, Mathf.Abs(advantage));}
        int damage = RollAttackDamage(baseAttack);
        if (advantage == 0){return damage;}
        for (int i = 0; i < advantage; i++)
        {
            damage = Mathf.Max(damage, RollAttackDamage(baseAttack));
        }
        return damage + advantage;
    }

    protected int Disadvantage(int baseAttack, int disadvantage)
    {
        int damage = RollAttackDamage(baseAttack);
        if (disadvantage == 0){return damage;}
        for (int i = 0; i < disadvantage; i++)
        {
            damage = Mathf.Min(damage, RollAttackDamage(baseAttack));
        }
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
            Debug.Log(passiveName);
            switch (passiveStats[3])
            {
                case "Advantage":
                Debug.Log("Advantage");
                Debug.Log(advantage);
                Debug.Log(">");
                advantage = passive.AffectInt(advantage, passiveStats[4], passiveStats[5]);
                Debug.Log(advantage);
                break;
                case "Damage%":
                Debug.Log("Damage%");
                Debug.Log(damageMultiplier);
                Debug.Log(">");
                damageMultiplier = passive.AffectInt(damageMultiplier, passiveStats[4], passiveStats[5]);
                Debug.Log(damageMultiplier);
                break;
                case "BaseDamage":
                Debug.Log("BaseDamage");
                Debug.Log(baseDamage);
                Debug.Log(">");
                baseDamage = passive.AffectInt(baseDamage, passiveStats[4], passiveStats[5]);
                Debug.Log(baseDamage);
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
