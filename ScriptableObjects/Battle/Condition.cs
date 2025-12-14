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
        statuses = new List<string>(actor.GetStatuses());
        for (int i = 0; i < statuses.Count; i++)
        {
            statusInfo = allData.ReturnStats(statuses[i]);
            if (!Timing(timing, statusInfo[0])){continue;}
            // Statuses can apply multiple effects at once.
            string[] effects = statusInfo[1].Split(",");
            string[] specifics = statusInfo[2].Split(",");
            for (int j = 0; j < effects.Length; j++)
            {
                AffectActor(actor, effects[j], specifics[j]);
            }
            // Decrease duration by 1.
            actor.AdjustStatusDuration(i);
        }
    }

    public void ApplyBuffEffects(TacticActor actor, StatDatabase allData, string timing)
    {
        statuses = new List<string>(actor.GetBuffs());
        for (int i = 0; i < statuses.Count; i++)
        {
            statusInfo = allData.ReturnStats(statuses[i]);
            if (statusInfo.Count < 3)
            {
                continue;
            }
            if (!Timing(timing, statusInfo[0])){continue;}
            // Statuses can apply multiple effects at once.
            string[] effects = statusInfo[1].Split(",");
            string[] specifics = statusInfo[2].Split(",");
            for (int j = 0; j < effects.Length; j++)
            {
                AffectActor(actor, effects[j], specifics[j]);
            }
            actor.AdjustBuffDuration(i);
        }
    }
}
