using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "ScriptableObjects/BattleLogic/Condition", order = 1)]
public class Condition : SkillEffect
{
    List<string> conditions;
    List<string> conditionInfo;

    // Maybe good use of enum? Either start/end of turn or both.
    protected bool Timing(string timing, string time)
    {
        if (timing == "ALL"){return true;}
        return timing == time;
    }

    public void ApplyEffects(TacticActor actor, StatDatabase allData, string timing)
    {
        conditions = actor.GetConditions();
        for (int i = 0; i < conditions.Count; i++)
        {
            conditionInfo = allData.ReturnStats(conditions[i]);
            if (!Timing(timing, conditionInfo[0])){continue;}
            AffectActor(actor, conditionInfo[1], conditionInfo[2]);
        }
    }
}
