using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveSelectList : SelectList
{
    public BattleManager battle;
    public ActiveManager activeManager;
    public int state;
    public List<GameObject> statePanels;
    protected void ResetPanels()
    {
        utility.DisableGameObjects(statePanels);
    }

    public void SetState(int newState)
    {
        ResetPanels();
        state = newState;
        statePanels[state].SetActive(true);
    }

    public override void Select(int index)
    {
        base.Select(index);
        IncrementState();
        ShowSelected();
        activeManager.SetSkillFromName(selected);
        activeManager.GetTargetableTiles(battle.GetTurnActor().GetLocation(), battle.moveManager.actorPathfinder);
        battle.map.UpdateHighlights(activeManager.ReturnTargetableTiles());
    }

    public void Deselect()
    {
        DecrementState();
        StartingPage();
    }

    protected void DecrementState()
    {
        int newState = state - 1;
        SetState(newState);
    }

    protected void IncrementState()
    {
        int newState = state + 1;
        SetState(newState);
    }

    public void ResetState(){SetState(0);}

    public void StartSelecting()
    {
        if (battle.GetTurnActor().ActiveSkillCount() <= 0){return;}
        IncrementState();
        SetSelectables(battle.GetTurnActor().GetActiveSkills());
        activeManager.SetSkillUser(battle.GetTurnActor());
        StartingPage();
    }
}