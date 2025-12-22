using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveOrganizer", menuName = "ScriptableObjects/BattleLogic/PassiveOrganizer", order = 1)]
public class PassiveOrganizer : ScriptableObject
{
    public List<string> testPassiveList;
    public List<string> testPassiveLevels;
    public MultiKeyStatDatabase passiveNameLevels;
    public StatDatabase allPassives;
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
        string passiveData = allPassives.ReturnValue(passiveName);
        actor.AddSortedPassive(passiveData, timing);
    }

    public void RemoveSortedPassive(TacticActor actor, string passiveName)
    {
        string timing = passiveTiming.ReturnValue(passiveName);
        string passiveData = allPassives.ReturnValue(passiveName);
        actor.RemoveSortedPassive(passiveData, timing);
    }

    protected void SortPassive(string passive, string timing, bool data = false)
    {
        string passiveDetails = "";
        if (data)
        {
            passiveDetails = passive;
        }
        else
        {
            passiveDetails = allPassives.ReturnValue(passive);
        }
        switch (timing)
        {
            case "Moving":
                movingPassives.Add(passiveDetails);
                break;
            case "Start":
                startTurnPassives.Add(passiveDetails);
                break;
            case "End":
                endTurnPassives.Add(passiveDetails);
                break;
            case "Attacking":
                attackingPassives.Add(passiveDetails);
                break;
            case "Defending":
                defendingPassives.Add(passiveDetails);
                break;
            case "BS":
                startBattlePassives.Add(passiveDetails);
                break;
            case "TakeDamage":
                takeDamagePassives.Add(passiveDetails);
                break;
            case "Death":
                deathPassives.Add(passiveDetails);
                break;
            case "OOC":
                outOfCombatPassives.Add(passiveDetails);
                break;
        }
    }

    protected void OrganizeCustomPassives(TacticActor actor)
    {
        List<string> customPassives = actor.GetCustomPassives();
        for (int i = 0; i < customPassives.Count; i++)
        {
            List<string> customData = customPassives[i].Split("|").ToList();
            if (customData.Count < 5){continue;}
            SortPassive(customPassives[i], customData[0], true);
        }
    }

    public void OrganizeActorPassives(TacticActor actor)
    {
        OrganizePassivesList(actor.GetPassiveSkills(), actor.GetPassiveLevels());
        OrganizeCustomPassives(actor);
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
