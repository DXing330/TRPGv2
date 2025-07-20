using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public PassiveOrganizer passiveOrganizer;
    public PassiveSkill passive;
    public StatDatabase passiveData;
    // Condition is a bad name, since passives have conditions to activate.
    public Condition status;
    public StatDatabase statusData;
    public BattleState battleState;


    public void StartBattle(TacticActor actor)
    {
        passive.ApplyStartBattlePassives(actor, passiveData, battleState);
    }

    public void StartTurn(TacticActor actor, BattleMap map)
    {
        status.ApplyEffects(actor, statusData, "Start");
        passive.ApplyPassives(actor, passiveData, "Start", map);
    }

    public void EndTurn(TacticActor actor, BattleMap map)
    {
        status.ApplyEffects(actor, statusData, "End");
        passive.ApplyPassives(actor, passiveData, "End", map);
        List<string> removedPassives = actor.DecreaseTempPassiveDurations();
        if (removedPassives.Count > 0)
        {
            for (int i = 0; i < removedPassives.Count; i++)
            {
                passiveOrganizer.RemoveSortedPassive(actor, removedPassives[i]);
            }
        }
    }
}
