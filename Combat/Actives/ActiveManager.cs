using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveManager : MonoBehaviour
{
    public ActiveSkill active;
    public StatDatabase activeData;

    public void SetSkill(TacticActor actor, int skillIndex)
    {
        active.LoadSkill(activeData.ReturnStats(actor.activeSkills[skillIndex]));
    }

    public List<int> GetTargetableTiles(ActiveSkill skill, int skillUserLocation, MoveCostManager moveManager)
    {
        List<int> tiles = new List<int>();
        return tiles;
    }
}
