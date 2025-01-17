using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveOrganizer : MonoBehaviour
{
    public List<string> testPassiveList;
    public List<string> testPassiveLevels;
    [ContextMenu("Test Sorting")]
    public void TestSorting()
    {
        OrganizePassivesList(testPassiveList, testPassiveLevels);
    }
    public MultiKeyStatDatabase passiveNameLevels;
    public StatDatabase passiveTiming;
    public List<string> startBattlePassives;
    public List<string> startTurnPassives;
    public List<string> endTurnPassives;
    public List<string> attackingPassives;
    public List<string> defendingPassives;
    public List<string> takeDamagePassives;
    public List<string> movingPassives;

    protected void ClearLists()
    {
        startBattlePassives.Clear();
        startTurnPassives.Clear();
        endTurnPassives.Clear();
        attackingPassives.Clear();
        defendingPassives.Clear();
        takeDamagePassives.Clear();
        movingPassives.Clear();
    }

    public void OrganizePassivesList(List<string> passives, List<string> passiveLevels)
    {
        ClearLists();
        string passiveName = "";
        for (int i = 0; i < passives.Count; i++)
        {
            for (int j = 1; j <= int.Parse(passiveLevels[i]); j++)
            {
                passiveName = passiveNameLevels.GetMultiKeyValue(passives[i], j.ToString());
                SortPassive(passiveName, passiveTiming.ReturnValue(passiveName));
            }
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
            case "BattleStart":
            startBattlePassives.Add(passive);
            break;
            case "TakeDamage":
            takeDamagePassives.Add(passive);
            break;
        }
    }

    public void OrganizeActorPassives(TacticActor actor)
    {
        OrganizePassivesList(actor.passiveSkills, actor.passiveLevels);
        actor.SetStartBattlePassives(startBattlePassives);
        actor.SetStartTurnPassives(startTurnPassives);
        actor.SetEndTurnPassives(endTurnPassives);
        actor.SetAttackingPassives(attackingPassives);
        actor.SetDefendingPassives(defendingPassives);
        actor.SetTakeDamagePassives(takeDamagePassives);
        actor.SetMovingPassives(movingPassives);
    }
}
