using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveOrganizer : MonoBehaviour
{
    public List<string> testPassiveList;
    [ContextMenu("Test Sorting")]
    public void TestSorting()
    {
        OrganizePassives(testPassiveList);
    }
    public StatDatabase passiveTiming;
    public List<string> startTurnPassives;
    public List<string> endTurnPassives;
    public List<string> attackingPassives;
    public List<string> defendingPassives;
    public List<string> movingPassives;

    protected void ClearLists()
    {
        startTurnPassives.Clear();
        endTurnPassives.Clear();
        attackingPassives.Clear();
        defendingPassives.Clear();
        movingPassives.Clear();
    }

    public void OrganizePassives(List<string> passives)
    {
        ClearLists();
        for (int i = 0; i < passives.Count; i++)
        {
            SortPassive(passives[i], passiveTiming.ReturnValue(passives[i]));
        }
    }

    protected void SortPassive(string passive, string timing)
    {
        switch (timing)
        {
            case "Moving":
            movingPassives.Add(passive);
            break;
            case "Start":
            startTurnPassives.Add(passive);
            break;
            case "End":
            endTurnPassives.Add(passive);
            break;
            case "Attacking":
            attackingPassives.Add(passive);
            break;
            case "Defending":
            defendingPassives.Add(passive);
            break;
        }
    }
}
