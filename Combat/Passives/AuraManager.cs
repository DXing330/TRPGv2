using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraManager : MonoBehaviour
{
    public BattleMap map;
    public PassiveSkill passive;
    public string GetAuraSpecifics(AuraEffect aura)
    {
        int eSpecifics = passive.GetScalingSpecifics(aura.actor, aura.effectSpecifics);
        if (eSpecifics <= 0)
        {
            return aura.effectSpecifics;
        }
        return eSpecifics.ToString();
    }
    public bool SpecialAuraEffect(TacticActor auraTarget, AuraEffect aura)
    {
        switch (aura.GetTarget())
        {
            default:
            return false;
            case "Attack":
            // Battle Manager Attack, Don't Consume Actions.
            map.battleManager.PublicAAA(aura.actor, auraTarget, false);
            return true;
        }
    }
    protected void TriggerAuraEffect(AuraEffect aura, TacticActor actor)
    {
        if (!aura.TriggerAura()){return;}
        if (passive.CheckAuraCondition(aura, actor, map))
        {
            map.combatLog.UpdateNewestLog(actor.GetPersonalName() + " is affected by " + aura.GetAuraName() + ".");
            map.combatLog.AddDetailedLogs(map.detailViewer.ReturnAuraDetails(aura));
            // Check if the effect is special.
            if (SpecialAuraEffect(actor, aura))
            {
                aura.ActorTriggersAura(actor);
                return;
            }
            passive.AffectActor(actor, aura.effect, GetAuraSpecifics(aura));
            aura.ActorTriggersAura(actor);
        }
    }
    protected void TriggerAuraEffects(List<AuraEffect> allAura, TacticActor actor)
    {
        for (int i = 0; i < allAura.Count; i++)
        {
            TriggerAuraEffect(allAura[i], actor);
        }
    }
    public void TriggerAllAuraEffects(List<AuraEffect> allAura, List<TacticActor> actors)
    {
        for (int i = 0; i < actors.Count; i++)
        {
            TriggerAuraEffects(allAura, actors[i]);
        }
    }
}
