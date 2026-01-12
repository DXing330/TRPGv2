using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public bool debug = false;
    public GeneralUtility utility;
    public BattleManager battleManager;
    public int state;
    public void SetState(int newState)
    {
        state = Mathf.Max(0, newState);
        utility.DisableGameObjects(stateObjects);
        stateObjects[state].SetActive(true);
    }
    public List<GameObject> stateObjects;
    // Prestate.
    public GameObject adjustStartingPositionsPanel;
    public void AdjustStartingPositions()
    {
        adjustStartingPositionsPanel.SetActive(true);
        utility.DisableGameObjects(playerChoiceActions);
    }
    public void FinishSettingStartingPositions()
    {
        adjustStartingPositionsPanel.SetActive(false);
        utility.EnableGameObjects(playerChoiceActions);
        SetState(0);
    }
    // State 0 - Player Stats + Actions.
    public BattleStats battleStats;
    public GameObject playerChoicesPanel;
    public List<GameObject> playerChoiceActions;
    public ActiveSelectList activeSelectList;
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
    public void PlayerTurn()
    {
        playerChoicesPanel.SetActive(true);
        utility.EnableGameObjects(playerChoiceActions);
    }
    public void ResetActiveSelectList()
    {
        activeSelectList.ResetState();
    }
    public void UpdateStatSheet(TacticActor actor)
    {

    }
    // State 1 - UI Choices
    // State 2 - Battle Log
    // State 3 - Turn Order
    public DisplayTurnOrder turnOrder;

    public void UpdateTurnOrder(BattleManager manager)
    {
        turnOrder.UpdateTurnOrder(manager.map.battlingActors, manager.GetTurnIndex());
    }
    // State 4 - Map Details.
    public int mapDetailState = 4;
    public bool ViewingDetails()
    {
        return mapDetailState == state;
    }
    public PopUpMessage mapPassives;
    public StatusDetailViewer statusDetails;
    public void ViewMapPassives(BattleMap map, int tileNumber)
    {
        string mPassives = "";
        mPassives += statusDetails.MapTilePassives(map, tileNumber);
        mPassives += "\n";
        mPassives += statusDetails.MapTEffectPassives(map, tileNumber);
        mapPassives.SetMessage(mPassives);
    }
}
