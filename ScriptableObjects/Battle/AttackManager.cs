using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackManager", menuName = "ScriptableObjects/AttackManager", order = 1)]
public class AttackManager : ScriptableObject
{
    public PassiveSkill passive;
    public StatDatabase passiveData;
    public int baseMultiplier;
    protected int advantage;
    protected int damageMultiplier;
    public void ActorAttacksActor(TacticActor attacker, TacticActor defender, BattleMap map, MoveCostManager moveManager)
    {
        advantage = 0;
        damageMultiplier = baseMultiplier;
        CheckPassives(attacker.allStats.attackingPassives, defender, attacker, map, moveManager);
        CheckPassives(defender.allStats.defendingPassives, defender, attacker, map, moveManager);
        int damage = attacker.allStats.GetAttack();
        Debug.Log(advantage);
        Debug.Log(damageMultiplier);
        damage = Advantage(damage, advantage);
        damage = damageMultiplier * damage / baseMultiplier;
        // Adjust damage based on passives, terrain effects, direction, etc.
        damage = Mathf.Max(0, damage - defender.allStats.GetDefense());
        // Check if the passive affects damage.
        defender.allStats.UpdateHealth(damage);
        Debug.Log(defender.GetSpriteName()+" takes "+damage+" damage.");
        attacker.PayAttackCost();
        attacker.SetDirection(moveManager.DirectionBetweenActors(attacker, defender));
    }

    protected int RollAttackDamage(int baseAttack)
    {
        return Random.Range(0, baseAttack) + baseAttack/2;
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

    protected void CheckPassives(List<string> characterPassives, TacticActor target, TacticActor other, BattleMap map, MoveCostManager moveManager)
    {
        for (int i = 0; i < characterPassives.Count; i++)
        {
            List<string> passiveStats = passiveData.ReturnStats(characterPassives[i]);
            if (passive.CheckBattleCondition(passiveStats[1], passiveStats[2], target, other, map, moveManager))
            {
                switch (passiveStats[3])
                {
                    case "Advantage":
                    advantage = passive.AffectInt(advantage, passiveStats[4], passiveStats[5]);
                    break;
                    case "Damage":
                    damageMultiplier = passive.AffectInt(damageMultiplier, passiveStats[4], passiveStats[5]);
                    break;
                }
            }
        }
    }
}
