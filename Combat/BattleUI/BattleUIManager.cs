using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public bool debug = false;
    public GeneralUtility utility;
    public BattleStats battleStats;
    public DisplayTurnOrder turnOrder;
    public GameObject playerChoicesPanel;
    public ActiveSelectList activeSelectList;
    public BattleManager battleManager;
    public SelectStatTextList statusSelect;
    public void ViewActorStatuses()
    {
        TacticActor viewedActor = battleManager.GetSelectedActor();
        statusSelect.SetStatsAndData(viewedActor.GetUniqueStatuses(), viewedActor.GetUnqiueStatusStacks());
    }
    public SelectStatTextList passiveSelect;
    public void ViewActorPassives()
    {
        TacticActor viewedActor = battleManager.GetSelectedActor();
        passiveSelect.SetStatsAndData(viewedActor.GetPassiveSkills(), viewedActor.GetPassiveLevels());
    }

    public void NPCTurn()
    {
        if (debug)
        {
            return;
        }
        playerChoicesPanel.SetActive(false);
    }

    public void PlayerTurn()
    {
        playerChoicesPanel.SetActive(true);
    }

    public void UpdateStatSheet(TacticActor actor)
    {

    }

    public void UpdateTurnOrder(BattleManager manager)
    {
        turnOrder.UpdateTurnOrder(manager.map.battlingActors, manager.GetTurnIndex());
    }

    public void ResetActiveSelectList()
    {
        activeSelectList.ResetState();
    }
}
