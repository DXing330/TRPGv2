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

    public void StartBattle(TacticActor actor)
    {
        passive.ApplyStartBattlePassives(actor, passiveData);
    }

    public void StartTurn(TacticActor actor)
    {
        status.ApplyEffects(actor, statusData, "Start");
        passive.ApplyPassives(actor, passiveData, "Start");
    }

    public void EndTurn(TacticActor actor)
    {
        status.ApplyEffects(actor, statusData, "End");
        passive.ApplyPassives(actor, passiveData, "End");
        if (actor.DecreaseTempPassiveDurations())
        {
            passiveOrganizer.OrganizeActorPassives(actor);
        }
    }
}
