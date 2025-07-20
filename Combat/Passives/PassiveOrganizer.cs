using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveOrganizer", menuName = "ScriptableObjects/BattleLogic/PassiveOrganizer", order = 1)]
public class PassiveOrganizer : ScriptableObject
{
    public List<string> testPassiveList;
    public List<string> testPassiveLevels;
    public MultiKeyStatDatabase passiveNameLevels;
    public StatDatabase passiveNames;
    public StatDatabase passiveTiming;
    public List<string> startBattlePassives;
    public List<string> startTurnPassives;
    public List<string> endTurnPassives;
    public List<string> attackingPassives;
    public List<string> defendingPassives;
    public List<string> takeDamagePassives;
    public List<string> movingPassives;
    public List<string> deathPassives;
    public List<string> outOfCombatPassives;

    protected void ClearLists()
    {
        startBattlePassives.Clear();
        startTurnPassives.Clear();
        endTurnPassives.Clear();
        attackingPassives.Clear();
        defendingPassives.Clear();
        takeDamagePassives.Clear();
        movingPassives.Clear();
        deathPassives.Clear();
        outOfCombatPassives.Clear();
    }

    protected void OrganizePassivesList(List<string> passives, List<string> passiveLevels)
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

    public void AddSortedPassive(TacticActor actor, string passiveName)
    {
        string timing = passiveTiming.ReturnValue(passiveName);
        actor.AddSortedPassive(passiveName, timing);
    }

    public void RemoveSortedPassive(TacticActor actor, string passiveName)
    {
        string timing = passiveTiming.ReturnValue(passiveName);
        actor.RemoveSortedPassive(passiveName, timing);
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
            case "Death":
                deathPassives.Add(passive);
                break;
            case "OOC":
                outOfCombatPassives.Add(passive);
                break;
        }
    }

    public void OrganizeActorPassives(TacticActor actor)
    {
        OrganizePassivesList(actor.GetPassiveSkills(), actor.GetPassiveLevels());
        actor.SetStartBattlePassives(startBattlePassives);
        actor.SetStartTurnPassives(startTurnPassives);
        actor.SetEndTurnPassives(endTurnPassives);
        actor.SetAttackingPassives(attackingPassives);
        actor.SetDefendingPassives(defendingPassives);
        actor.SetTakeDamagePassives(takeDamagePassives);
        actor.SetMovingPassives(movingPassives);
        actor.SetDeathPassives(deathPassives);
        actor.SetOOCPassives(outOfCombatPassives);
    }
}
