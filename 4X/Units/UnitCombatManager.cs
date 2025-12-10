using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombatManager : MonoBehaviour
{
    // Both units take damage and potentially die.
    // Roll to see who wins the battle based on a combination of stats and unit count.
    // Winner takes less damage, loser takes more damage, both gain exp.
    public TacticActor dummyActor;
    protected void StatClashRoll(CombatUnit unit1, CombatUnit unit2, int stat1, int stat2)
    {
        int roll1 = Random.Range(0, stat1);
        int roll2 = Random.Range(0, stat2);
        if (roll1 > roll2)
        {
            unit2.TakeDamage();
        }
        else if (roll2 > roll1)
        {
            unit1.TakeDamage();
        }
        // Ties hurt everyone.
        else
        {
            unit1.TakeDamage();
            unit2.TakeDamage();
        }
    }
    public void UnitsBattle(CombatUnit unit1, CombatUnit unit2)
    {
        int team1HP = 0;
        int team2HP = 0;
        int team1Atk = 0;
        int team2Atk = 0;
        int team1Def = 0;
        int team2Def = 0;
        for (int i = 0; i < unit1.actorStats.Count; i++)
        {
            dummyActor.SetStatsFromString(unit1.actorStats[i]);
            team1HP += dummyActor.GetBaseHealth() * unit1.unitHealths[i];
            team1Atk += dummyActor.GetBaseAttack() * unit1.unitHealths[i];
            team1Def += dummyActor.GetBaseDefense() * unit1.unitHealths[i];
        }
        for (int i = 0; i < unit2.actorStats.Count; i++)
        {
            dummyActor.SetStatsFromString(unit2.actorStats[i]);
            team2HP += dummyActor.GetBaseHealth() * unit2.unitHealths[i];
            team2Atk += dummyActor.GetBaseAttack() * unit2.unitHealths[i];
            team2Def += dummyActor.GetBaseDefense() * unit2.unitHealths[i];
        }
        unit1.TakeDamage();
        unit2.TakeDamage();
        StatClashRoll(unit1, unit2, team1HP, team2HP);
        StatClashRoll(unit1, unit2, team1Atk, team2Atk);
        StatClashRoll(unit1, unit2, team1Def, team2Def);
    }
}
