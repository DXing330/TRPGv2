using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public bool debug = false;
    public BattleStats battleStats;
    public DisplayTurnOrder turnOrder;
    public GameObject playerChoicesPanel;
    public ActiveSelectList activeSelectList;

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
