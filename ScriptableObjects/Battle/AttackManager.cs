using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackManager", menuName = "ScriptableObjects/AttackManager", order = 1)]
public class AttackManager : ScriptableObject
{
    public void ActorAttacksActor(TacticActor attacker, TacticActor defender, BattleMap map, MoveCostManager moveManager)
    {
        int advantage = 0;
        // Determine advantage based on passives/etc.
        int damage = attacker.allStats.GetAttack();
        damage = Advantage(damage, advantage);
        // Adjust damage based on passives, terrain effects, direction, etc.
        damage -= defender.allStats.GetDefense();
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
}
