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
        passive.ApplyStartBattlePassives(actor, battleState);
    }

    public void StartTurn(TacticActor actor, BattleMap map)
    {
        map.ActorStartsTurn(actor);
        passive.ApplyPassives(actor, "Start", map);
        // Status effects apply last so that passives have a chance to remove negative status effects.
        status.ApplyBuffEffects(actor, statusData, "Start");
        status.ApplyEffects(actor, statusData, "Start");
        // Check on grapples at the start of every turn.
        if (actor.Grappled(map))
        {
            // Can't move while grappled.
            actor.currentSpeed = 0;
        }
        actor.Grappling(map);
    }

    public void EndTurn(TacticActor actor, BattleMap map)
    {
        map.ActorEndsTurn(actor);
        map.AuraActorEndsTurn(actor);
        passive.ApplyPassives(actor, "End", map);
        status.ApplyBuffEffects(actor, statusData, "End");
        status.ApplyEffects(actor, statusData, "End");
        List<string> removedPassives = actor.DecreaseTempPassiveDurations();
        if (removedPassives.Count > 0)
        {
            for (int i = 0; i < removedPassives.Count; i++)
            {
                passiveOrganizer.RemoveSortedPassive(actor, removedPassives[i]);
            }
        }
        // Check on grapples at the end of every turn.
        actor.Grappled(map);
        actor.Grappling(map);
    }
}
