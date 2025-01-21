using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStatTextList : StatTextList
{
    public int selectedIndex;
    public void Select(int index)
    {
        selectedIndex = index + (page*objects.Count);
    }
    public int GetSelected(){return selectedIndex;}
    // This is more specialized but placed here for now.
    public void UpdateActorStatTexts(TacticActor actor)
    {
        DisableChangePage();
        page = 0;
        stats.Clear();
        data.Clear();
        stats.Add("Health");
        stats.Add("Attack");
        stats.Add("Defense");
        stats.Add("Move Speed");
        data.Add(actor.GetBaseHealth().ToString());
        data.Add(actor.GetBaseAttack().ToString());
        data.Add(actor.GetBaseDefense().ToString());
        data.Add(actor.GetMoveSpeed().ToString());
        for (int i = 0; i < 4; i++)
        {
            objects[i].SetActive(true);
            statTexts[i].SetStatText(stats[i]);
            statTexts[i].SetText(data[i]);
        }
    }

    public void UpdateActorPassiveTexts(TacticActor actor)
    {
        page = 0;
        stats.Clear();
        data.Clear();
        stats = new List<string>(actor.GetPassiveSkills());
        data = new List<string>(actor.GetPassiveLevels());
        for (int i = 0; i < stats.Count; i++)
        {
            objects[i].SetActive(true);
            statTexts[i].SetStatText(stats[i]);
            statTexts[i].SetText(data[i]);
        }
        if (stats.Count > objects.Count){EnableChangePage();}
    }
}
