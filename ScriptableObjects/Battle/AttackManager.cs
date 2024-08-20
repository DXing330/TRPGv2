using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackManager", menuName = "ScriptableObjects/AttackManager", order = 1)]
public class AttackManager : ScriptableObject
{
    public void ActorAttacksActor(TacticActor attacker, TacticActor defender, List<string> mapInfo)
    {
        int damage = attacker.allStats.GetAttack();
        // Passives, terrain effects, direction, etc.
        damage -= defender.allStats.GetDefense();
        defender.allStats.UpdateHealth(damage);
        attacker.PayAttackCost();
    }
}
