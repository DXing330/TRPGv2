using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "ScriptableObjects/BattleLogic/Condition", order = 1)]
public class Condition : SkillEffect
{
    List<string> statuses;
    List<string> statusInfo;

    // Maybe good use of enum? Either start/end of turn or both.
    protected bool Timing(string timing, string time)
    {
        if (timing == "ALL"){return true;}
        return timing == time;
    }

    public void ApplyEffects(TacticActor actor, StatDatabase allData, string timing)
    {
        statuses = actor.GetStatuses();
        for (int i = 0; i < statuses.Count; i++)
        {
            statusInfo = allData.ReturnStats(statuses[i]);
            if (!Timing(timing, statusInfo[0])){continue;}
            AffectActor(actor, statusInfo[1], statusInfo[2]);
            // Decrease duration by 1.
            actor.AdjustStatusDuration(i);
        }
    }
}
