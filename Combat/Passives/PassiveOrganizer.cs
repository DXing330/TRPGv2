using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveOrganizer : MonoBehaviour
{
    public List<string> testPassiveList;
    [ContextMenu("Test Sorting")]
    public void TestSorting()
    {
        OrganizePassivesList(testPassiveList);
    }
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

    public void OrganizePassivesList(List<string> passives)
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
        OrganizePassivesList(actor.passiveSkills);
        actor.SetStartBattlePassives(startBattlePassives);
        actor.SetStartTurnPassives(startTurnPassives);
        actor.SetEndTurnPassives(endTurnPassives);
        actor.SetAttackingPassives(attackingPassives);
        actor.SetDefendingPassives(defendingPassives);
        actor.SetTakeDamagePassives(takeDamagePassives);
        actor.SetMovingPassives(movingPassives);
    }

    public void OrganizePassives(ActorPassives passives)
    {
        OrganizePassivesList(passives.passiveSkills);
        passives.SetStartBattlePassives(startBattlePassives);
        passives.SetStartTurnPassives(startTurnPassives);
        passives.SetEndTurnPassives(endTurnPassives);
        passives.SetAttackingPassives(attackingPassives);
        passives.SetDefendingPassives(defendingPassives);
        passives.SetTakeDamagePassives(takeDamagePassives);
        passives.SetMovingPassives(movingPassives);
    }
}
