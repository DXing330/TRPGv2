using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStats : MonoBehaviour
{
    public TacticActor actor;
    public void SetActor(TacticActor newActor)
    {
        actor = newActor;
        UpdateBasicStats();
        UpdateSpendableStats();
    }
    public List<StatImageText> basicStats;
    public List<StatImageText> spendableStats;

    public void UpdateBasicStats()
    {
        List<string> stats = actor.ReturnStats();
        for (int i = 0; i < Mathf.Min(stats.Count, basicStats.Count); i++)
        {
            basicStats[i].SetText(stats[i]);
        }
    }

    public void UpdateSpendableStats()
    {
        List<string> stats = actor.ReturnSpendableStats();
        for (int i = 0; i < Mathf.Min(stats.Count, spendableStats.Count); i++)
        {
            spendableStats[i].SetText(stats[i]);
        }
    }
}
