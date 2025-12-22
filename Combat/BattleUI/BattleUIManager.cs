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
    public GameObject adjustStartingPositionsPanel;
    public List<GameObject> playerChoiceActions;
    public ActiveSelectList activeSelectList;
    public BattleManager battleManager;
    public SelectStatTextList statusSelect;
    public void ViewActorStatuses()
    {
        TacticActor viewedActor = battleManager.GetSelectedActor();
        if (viewedActor == null){return;}
        statusSelect.SetStatsAndData(viewedActor.GetUniqueStatusAndBuffs(), viewedActor.GetUnqiueSBDurations());
    }
    public SelectStatTextList passiveSelect;
    public PassiveDetailViewer passiveViewer;
    public void ViewActorPassives()
    {
        TacticActor viewedActor = battleManager.GetSelectedActor();
        if (viewedActor == null){return;}
        passiveSelect.SetStatsAndData(viewedActor.GetPassiveSkills(), viewedActor.GetPassiveLevels());
    }
    public void ViewActorCustomPassives()
    {
        TacticActor viewedActor = battleManager.GetSelectedActor();
        if (viewedActor == null){return;}
        passiveViewer.ViewCustomPassives(viewedActor);
    }

    public void NPCTurn()
    {
        if (debug)
        {
            return;
        }
        utility.DisableGameObjects(playerChoiceActions);
    }

    public void AdjustStartingPositions()
    {
        adjustStartingPositionsPanel.SetActive(true);
        utility.DisableGameObjects(playerChoiceActions);
    }

    public void FinishSettingStartingPositions()
    {
        adjustStartingPositionsPanel.SetActive(false);
        utility.EnableGameObjects(playerChoiceActions);
    }

    public void PlayerTurn()
    {
        playerChoicesPanel.SetActive(true);
        utility.EnableGameObjects(playerChoiceActions);
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
